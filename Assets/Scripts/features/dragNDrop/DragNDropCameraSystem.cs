using System;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.common;
using td.components.behaviors;
using td.components.events;
using td.components.flags;
using td.components.refs;
using td.features.state;
using td.services;
using td.utils.ecs;
using UnityEngine;

namespace td.features.dragNDrop
{
    public class DragNDropCameraSystem : IEcsRunSystem, IEcsInitSystem
    {
        [Inject] private State state;
        [Inject] private LevelMap levelMap;
        
        [InjectWorld] private EcsWorld world;
        [InjectWorld(Constants.Worlds.Outer)] private EcsWorld outerWorld;

        private readonly EcsFilterInject<Inc<IsDragging, DraggingStartedData, Ref<GameObject>>, Exc<IsDestroyed>> entities = default;
        private readonly EcsFilterInject<Inc<ReachingTargetEvent, IsRollbackDragging, DraggingStartedData, Ref<GameObject>>, Exc<IsDestroyed>> entitiesRollbackFinished = default;
        
        private static GameObject canvasDragLayer;

        public void Run(IEcsSystems systems)
        {
            var cursorPosition = Input.mousePosition;
            var currentTime = Time.timeSinceLevelLoadAsDouble;
            
            foreach (var entity in entities.Value)
            {
                ref var isDragging = ref entities.Pools.Inc1.Get(entity);
                
                if (isDragging.mode != DragMode.Camera) continue;
                
                ref var draggingStartedData = ref entities.Pools.Inc2.Get(entity);
                ref var refGameObject = ref entities.Pools.Inc3.Get(entity);
                var gameObject = refGameObject.reference;

                var position = (Vector2)cursorPosition;

                var isSmooth = world.HasComponent<IsSmoothDragging>(entity);

                if (isSmooth && world.HasComponent<LinearMovementToTarget>(entity))
                {
                    ref var movement = ref world.GetComponent<LinearMovementToTarget>(entity);
                    movement.target = position;

                    var distance = (position - (Vector2)gameObject.transform.position).magnitude;
                    movement.speed = distance * Constants.UI.DragNDrop.SmoothSpeed;
                }
                else
                {
                    gameObject.transform.position = position;
                }

                var isUnableToDrop = world.HasComponent<IsUnableToDrop>(entity);
                
                var removeIsDraging = false;
                var isRollback = false;
                
                switch (isDragging.state)
                {
                    case IsDraggingState.None:
                        if (Input.GetMouseButtonUp(0))
                        {
                            var deltaTime = currentTime - draggingStartedData.startedTime;
                            // Debug.Log($"> DnD: state=NONE; mb=UP; dt:{deltaTime:0.000s}; isUnableToDrop:{(isUnableToDrop ? "+" : "-")}");
                            if (deltaTime < Constants.UI.DragNDrop.TimeForAwaitDown)
                            {
                                // Debug.Log($"> ...delta time is small switch state to DOWN");
                                isDragging.state = IsDraggingState.Down;
                            }
                            else
                            {
                                //todo
                                if (!isUnableToDrop)
                                {
                                    // Debug.Log($"> ...REMOVE IsDraging !!!");
                                    removeIsDraging = true;
                                }
                            }
                        }
                        break;
                    
                    case IsDraggingState.Down:
                        if (Input.GetMouseButtonDown(0))
                        {
                            // Debug.Log($"> DnD: state=DOWN; mb=DOWN; isUnableToDrop:{(isUnableToDrop ? "+" : "-")}");
                            if (!isUnableToDrop)
                            {
                                // Debug.Log($"> ...switch state to UP");
                                isDragging.state = IsDraggingState.Up;
                            }
                        }
                        break;

                    case IsDraggingState.Up:
                        if (Input.GetMouseButtonUp(0))
                        {
                            // Debug.Log($"> DnD: state=UP; mb=UP; isUnableToDrop:{(isUnableToDrop ? "+" : "-")}");
                            if (isUnableToDrop)
                            {
                                // Debug.Log($"> ...switch state to DOWN");
                                isDragging.state = IsDraggingState.Down;
                            }
                            else
                            {
                                // Debug.Log($"> ...REMOVE IsDraging !!!");
                                removeIsDraging = true;
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
                    world.GetComponent<DragEndEvent>(entity).mode = DragMode.Camera;
                    if (isSmooth && world.HasComponent<IsSmoothDragging>(entity))
                    {
                        ref var smooth = ref world.GetComponent<IsSmoothDragging>(entity);
                        if (smooth.removeLinearMovementWhenFinished)
                        {
                            world.DelComponent<LinearMovementToTarget>(entity);
                        }
                    }

                    gameObject.transform.position = position;
                    
                    //todo check
                    var sortingLayerChangeable = gameObject.GetComponent<ISortingLayerChangeable>();
                    if (sortingLayerChangeable != null && sortingLayerChangeable.sortigLayer != draggingStartedData.startedLayer)
                    {
                        sortingLayerChangeable.sortigLayer = draggingStartedData.startedLayer;
                    }
                    
                    world.DelComponent<IsSmoothDragging>(entity);
                    world.DelComponent<IsDragging>(entity);

                    if (isRollback)
                    {
                        world.GetComponent<IsRollbackDragging>(entity).mode = DragMode.Camera;
                        ref var target = ref world.GetComponent<LinearMovementToTarget>(entity);
                        target.from = refGameObject.reference.transform.position;
                        target.target = draggingStartedData.startedPosition;
                        target.speed = Mathf.Max(Screen.width, Screen.height) * Constants.UI.DragNDrop.RollbackSpeed;
                    }
                }
            }
            
            foreach (var entityRollback in entitiesRollbackFinished.Value)
            {
                if (entitiesRollbackFinished.Pools.Inc2.Get(entityRollback).mode != DragMode.Camera) continue;
                
                ref var draggingStartedData = ref entitiesRollbackFinished.Pools.Inc3.Get(entityRollback);
                ref var refGameObject = ref entitiesRollbackFinished.Pools.Inc4.Get(entityRollback);

                refGameObject.reference.transform.position = draggingStartedData.startedPosition;
                refGameObject.reference.transform.SetParent(draggingStartedData.parentContainer);
                
                world.DelComponent<DraggingStartedData>(entityRollback);
                
                world.DelComponent<IsRollbackDragging>(entityRollback);
                world.DelComponent<LinearMovementToTarget>(entityRollback);
                world.DelComponent<IsDragging>(entityRollback);

                world.GetComponent<DragRollbackEvent>(entityRollback);
            }
        }

        public static void BeginDrag(
            IEcsSystems systems, 
            int entity,
            bool smoothDragging = false,
            float smoothSpeed = Constants.UI.DragNDrop.SmoothSpeed
        ) {
            var world = systems.GetWorld();

            if (!world.HasComponent<Ref<GameObject>>(entity)) return;
            ref var refGameObject = ref world.GetComponent<Ref<GameObject>>(entity);
            
            ref var isDragging = ref world.GetComponent<IsDragging>(entity);
            isDragging.mode = DragMode.Camera;
            isDragging.isGridSnapping = false;
            isDragging.state = Input.GetMouseButtonDown(0) ? IsDraggingState.None : IsDraggingState.Down;

            ref var draggingStartedData = ref world.GetComponent<DraggingStartedData>(entity);
            draggingStartedData.startedPosition = refGameObject.reference.transform.position;
            draggingStartedData.startedTime = Time.timeSinceLevelLoadAsDouble;
            draggingStartedData.parentContainer = refGameObject.reference.transform.parent.transform;

            world.GetComponent<DragStartEvent>(entity).mode = DragMode.Camera;
            
            refGameObject.reference.transform.position = Input.mousePosition;
            refGameObject.reference.transform.SetParent(canvasDragLayer.transform);
            
            if (smoothDragging)
            {
                var hasLinearMovement = world.HasComponent<LinearMovementToTarget>(entity);

                ref var isSmoothDragging = ref world.GetComponent<IsSmoothDragging>(entity);
                isSmoothDragging.speed = smoothSpeed;
                isSmoothDragging.removeLinearMovementWhenFinished = !hasLinearMovement;

                ref var linearMovementToTarget = ref world.GetComponent<LinearMovementToTarget>(entity);
                linearMovementToTarget.target = refGameObject.reference.transform.position;
                linearMovementToTarget.gap = Constants.DefaultGap;
                linearMovementToTarget.speed = Constants.UI.DragNDrop.SmoothSpeed;
            }
        }

        public void Init(IEcsSystems systems)
        {
            canvasDragLayer = GameObject.FindGameObjectWithTag(Constants.Tags.CanvasDragLayer);
        }
    }
}