using System;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.components.behaviors;
using td.components.events;
using td.components.flags;
using td.components.refs;
using td.services;
using td.utils;
using td.utils.ecs;
using UnityEngine;

namespace td.features.input
{
    public class DragNDropSystem : IEcsRunSystem
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
                ref var isDraggeing = ref entities.Pools.Inc1.Get(entity);
                ref var reGameObject = ref entities.Pools.Inc2.Get(entity);
                var gameObject = reGameObject.reference;

                var position = isDraggeing.isGridSnapping
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
                
                switch (isDraggeing.mode)
                {
                    case IsDraggingMode.None:
                        if (Input.GetMouseButtonUp(0))
                        {
                            var deltaTime = currentTime - isDraggeing.startedTime;
                            // Debug.Log($"> DnD: mode=NONE; mb=UP; dt:{deltaTime:0.000s}; isUnableToDrop:{(isUnableToDrop ? "+" : "-")}");
                            if (deltaTime < Constants.UI.DragNDrop.TimeForAwaitDown)
                            {
                                // Debug.Log($"> ...delta time is small switch mode to DOWN");
                                isDraggeing.mode = IsDraggingMode.Down;
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
                    
                    case IsDraggingMode.Down:
                        if (Input.GetMouseButtonDown(0))
                        {
                            // Debug.Log($"> DnD: mode=DOWN; mb=DOWN; isUnableToDrop:{(isUnableToDrop ? "+" : "-")}");
                            if (!isUnableToDrop)
                            {
                                // Debug.Log($"> ...switch mode to UP");
                                isDraggeing.mode = IsDraggingMode.Up;
                            }
                        }
                        break;

                    case IsDraggingMode.Up:
                        if (Input.GetMouseButtonUp(0))
                        {
                            // Debug.Log($"> DnD: mode=UP; mb=UP; isUnableToDrop:{(isUnableToDrop ? "+" : "-")}");
                            if (isUnableToDrop)
                            {
                                // Debug.Log($"> ...switch mode to DOWN");
                                isDraggeing.mode = IsDraggingMode.Down;
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
                    world.GetComponent<DragEndEvent>(entity);
                    if (isSmooth && world.HasComponent<IsSmoothDragging>(entity))
                    {
                        ref var smooth = ref world.GetComponent<IsSmoothDragging>(entity);
                        if (smooth.removeLinearMovementWhenFinished)
                        {
                            world.DelComponent<LinearMovementToTarget>(entity);
                        }
                    }
                    world.DelComponent<IsSmoothDragging>(entity);
                    world.DelComponent<IsDragging>(entity);

                    gameObject.transform.position = position;
                }
            }
        }

        public static void BeginDrag(
            IEcsSystems systems, 
            int entityWithGameObjectRef,
            bool snapToGrid = false,
            bool smoothDragging = false,
            float smoothSpeed = Constants.UI.DragNDrop.SmoothSpeed
        ) {
            var world = systems.GetWorld();
            
            var entity = entityWithGameObjectRef;

            if (!world.HasComponent<Ref<GameObject>>(entity)) return;
            ref var refGameObject = ref world.GetComponent<Ref<GameObject>>(entity);

            ref var isDragging = ref world.GetComponent<IsDragging>(entity);
            isDragging.startedTime = Time.timeSinceLevelLoadAsDouble;
            isDragging.isGridSnapping = snapToGrid;
            isDragging.mode = Input.GetMouseButtonDown(0) ? IsDraggingMode.None : IsDraggingMode.Down;

            world.GetComponent<DragStartEvent>(entity);
            
            // Debug.Log($"> DnD: mode={isDragging.mode.ToString()}; mb={(Input.GetMouseButtonDown(0) ? "DOWN" : "UP")}");

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
    }
}