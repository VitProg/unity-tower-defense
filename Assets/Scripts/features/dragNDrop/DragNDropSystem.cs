using System;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.common;
using td.features._common;
using td.features.dragNDrop.events;
using td.features.dragNDrop.flags;
using td.utils;
using td.utils.ecs;
using UnityEngine;

namespace td.features.dragNDrop
{
    public class DragNDropSystem : IEcsRunSystem
    {
        private readonly EcsInject<DragNDrop_Pools> pools;
        private readonly EcsInject<SharedData> shared;
        private readonly EcsInject<DragNDrop_Service> dndService;
        private readonly EcsInject<Common_Service> common;
        
        private readonly EcsWorldInject world;

        public void Run(IEcsSystems systems)
        {
            var cursorPositionOnWorld = CameraUtils.ToWorldPoint(shared.Value.mainCamera, Input.mousePosition);
            
            var cursorPositionOnScreen = Input.mousePosition;
            var cursorPositionOnCanvasCamera = shared.Value.canvasCamera.ScreenToWorldPoint(cursorPositionOnScreen);
            
            var currentTime = Time.timeSinceLevelLoadAsDouble;
            
            foreach (var entity in pools.Value.dndFilter.Value)
            {
                ref var isDragging = ref dndService.Value.GetIsDragging(entity);
                var inWorld = isDragging.mode == DragMode.World;
                var inCamera = isDragging.mode == DragMode.Camera;
                
                // if (isDragging.mode != DragMode.World) continue;
                
                ref var draggingStartedData = ref pools.Value.dndFilter.Pools.Inc2.Get(entity);
                var gameObject = common.Value.GetGameObject(entity)!;

                Vector2 position = inWorld
                    ? (isDragging.isGridSnapping
                        ? HexGridUtils.SnapToGrid(cursorPositionOnWorld)
                        : cursorPositionOnWorld)
                    : cursorPositionOnCanvasCamera;

                var isSmooth = dndService.Value.IsSmooth(entity);

                if (isSmooth && common.Value.HasMovement(entity))
                {
                    var distance = (position - (Vector2)gameObject.transform.position).magnitude;
                    ref var movement = ref common.Value.GetMovement(entity);
                    movement.target = position;
                    movement.speed = distance * Constants.UI.DragNDrop.SmoothSpeed;
                    if (!inCamera) movement.resetAnchoredPositionZ = true;
                }
                else
                {
                    gameObject.transform.position = position;
                    if (inCamera)
                    {
                        var rectTransform = ((RectTransform)gameObject.transform);
                        var ap = rectTransform.anchoredPosition3D;
                        rectTransform.anchoredPosition3D = new Vector3(ap.x, ap.y, 0.0f);

                    }
                }

                var canDrop = dndService.Value.CanDrop(entity);
                
                var removeIsDraging = false;
                var isRollback = false;
                
                switch (isDragging.state)
                {
                    case IsDraggingState.None:
                        if (Input.GetMouseButtonUp(0))
                        {
                            var deltaTime = currentTime - draggingStartedData.startedTime;
                            if (deltaTime < Constants.UI.DragNDrop.TimeForAwaitDown)
                            {
                                isDragging.state = IsDraggingState.Down;
                            }
                            else
                            {
                                //todo
                                if (canDrop)
                                {
                                    removeIsDraging = true;
                                }
                            }
                        }
                        break;
                    
                    case IsDraggingState.Down:
                        if (Input.GetMouseButtonDown(0) && canDrop) {
                            isDragging.state = IsDraggingState.Up;
                        }
                        break;

                    case IsDraggingState.Up:
                        if (Input.GetMouseButtonUp(0))
                        {
                            if (canDrop)
                            {
                                removeIsDraging = true;
                            }
                            else
                            {
                                isDragging.state = IsDraggingState.Down;
                            }
                        }
                        break;
                    
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                if (Input.GetMouseButtonUp(1) || Input.GetKeyUp(KeyCode.Escape))
                {
                    //rollback
                    removeIsDraging = true;
                    isRollback = true;
                }

                if (removeIsDraging)
                {
                    pools.Value.dragEndEventPool.Value.GetOrAdd(entity).mode = isDragging.mode;
                    if (isSmooth && dndService.Value.IsSmooth(entity))
                    {
                        ref var smooth = ref dndService.Value.GetIsSmooth(entity);
                        if (smooth.removeLinearMovementWhenFinished)
                        {
                            common.Value.RemoveMovement(entity);
                        }
                    }

                    gameObject.transform.position = position;
                    
                    var sortingLayerChangeable = gameObject.GetComponent<ISortingLayerChangeable>();
                    if (sortingLayerChangeable != null && sortingLayerChangeable.sortigLayer != draggingStartedData.startedLayer)
                    {
                        sortingLayerChangeable.sortigLayer = draggingStartedData.startedLayer;
                    }
                    
                    dndService.Value.RemoveIsSmooth(entity);
                    dndService.Value.RemoveIsDragging(entity);

                    if (isRollback)
                    {
                        dndService.Value.SetIsRollback(entity, true, isDragging.mode);
                        ref var movement = ref common.Value.GetMovement(entity);
                        movement.from = gameObject.transform.position;
                        movement.target = draggingStartedData.startedPosition;
                        /* todo: recalc for world space \/ */
                        movement.speed = Mathf.Max(Screen.width, Screen.height) * Constants.UI.DragNDrop.RollbackSpeed;
                        if (inCamera) movement.resetAnchoredPositionZ = true;
                    }
                }
            }
            
            foreach (var entity in pools.Value.rollbackFinishedFilter.Value)
            {
                if (dndService.Value.GetIsRollback(entity).mode != DragMode.Camera) continue;
                
                dndService.Value.Clear(entity);
            }
        }
    }
}