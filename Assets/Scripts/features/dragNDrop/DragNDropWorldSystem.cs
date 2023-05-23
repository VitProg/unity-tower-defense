using System;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.common;
using td.components.behaviors;
using td.components.flags;
using td.components.refs;
using td.services;
using td.utils;
using td.utils.ecs;
using UnityEngine;

namespace td.features.dragNDrop
{
    public class DragNDropWorldSystem : IEcsRunSystem
    {
        [Inject] private LevelState levelState;
        [Inject] private LevelMap levelMap;
        
        [InjectWorld] private EcsWorld world;
        [InjectWorld(Constants.Worlds.Outer)] private EcsWorld outerWorld;

        private readonly EcsFilterInject<Inc<IsDragging, Ref<GameObject>>, Exc<IsDestroyed>> entities = default;
        
        public void Run(IEcsSystems systems)
        {
            var cursorPosition = CameraUtils.ToWorldPoint(Input.mousePosition);
            var currentTime = Time.timeSinceLevelLoadAsDouble;
            
            foreach (var entity in entities.Value)
            {
                ref var isDragging = ref entities.Pools.Inc1.Get(entity);
                
                if (isDragging.mode != DragMode.World) continue;
                
                ref var reGameObject = ref entities.Pools.Inc2.Get(entity);
                var gameObject = reGameObject.reference;

                var position = isDragging.isGridSnapping
                    ? HexGridUtils.SnapToGrid(cursorPosition)
                    : (Vector2)cursorPosition;

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
                
                switch (isDragging.state)
                {
                    case IsDraggingState.None:
                        if (Input.GetMouseButtonUp(0))
                        {
                            var deltaTime = currentTime - isDragging.startedTime;
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

                if (removeIsDraging)
                {
                    world.GetComponent<DragEndEvent>(entity).mode = DragMode.World;
                    if (isSmooth && world.HasComponent<IsSmoothDragging>(entity))
                    {
                        ref var smooth = ref world.GetComponent<IsSmoothDragging>(entity);
                        if (smooth.removeLinearMovementWhenFinished)
                        {
                            world.DelComponent<LinearMovementToTarget>(entity);
                        }
                    }

                    gameObject.transform.position = position;
                    
                    var sortingLayerChangeable = gameObject.GetComponent<ISortingLayerChangeable>();
                    if (sortingLayerChangeable != null && sortingLayerChangeable.sortigLayer != isDragging.startedLayer)
                    {
                        sortingLayerChangeable.sortigLayer = isDragging.startedLayer;
                    }
                    
                    world.DelComponent<IsSmoothDragging>(entity);
                    world.DelComponent<IsDragging>(entity);
                }
            }
        }

        public static void BeginDrag(
            IEcsSystems systems, 
            int entity,
            bool snapToGrid = false,
            bool smoothDragging = false,
            float smoothSpeed = Constants.UI.DragNDrop.SmoothSpeed,
            bool moveToUpperLayer = true
        ) {
            var world = systems.GetWorld();

            if (!world.HasComponent<Ref<GameObject>>(entity)) return;
            ref var refGameObject = ref world.GetComponent<Ref<GameObject>>(entity);

            var sortingLayerChangeable = refGameObject.reference.GetComponent<ISortingLayerChangeable>();

            ref var isDragging = ref world.GetComponent<IsDragging>(entity);
            isDragging.mode = DragMode.World;
            isDragging.startedPosition = refGameObject.reference.transform.position;
            isDragging.startedTime = Time.timeSinceLevelLoadAsDouble;
            isDragging.isGridSnapping = snapToGrid;
            isDragging.state = Input.GetMouseButtonDown(0) ? IsDraggingState.None : IsDraggingState.Down;
            isDragging.startedLayer = sortingLayerChangeable?.sortigLayer ?? Constants.Layers.L3_Buildings;

            world.GetComponent<DragStartEvent>(entity).mode = DragMode.World;

            if (moveToUpperLayer && sortingLayerChangeable != null)
            {
                sortingLayerChangeable.sortigLayer = Constants.Layers.L11_Draggable;
            }
            
            // Debug.Log($"> DnD: state={isDragging.state.ToString()}; mb={(Input.GetMouseButtonDown(0) ? "DOWN" : "UP")}");

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

        //ToDO
        public static void RollbackDrag(
            IEcsSystems systems,
            int entity
        )
        {
            var world = systems.GetWorld();

            if (!world.HasComponent<Ref<GameObject>>(entity)) return;
            ref var refGameObject = ref world.GetComponent<Ref<GameObject>>(entity);

            //todo move to isDragging.startedPosition
        }
    }
}