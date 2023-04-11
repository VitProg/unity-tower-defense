using System;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.components;
using td.components.behaviors;
using td.components.events;
using td.components.flags;
using td.features.towers;
using td.utils;
using td.utils.ecs;
using UnityEngine;

namespace td.features.input
{
    public class TowerDragAndDropSystem: IEcsRunSystem
    {
        [EcsWorld] private EcsWorld world;

        private readonly EcsFilterInject<Inc<Tower, IsDragging, Ref<GameObject>>> entities = default;

        public void Run(IEcsSystems systems)
        {
            var cursorPosition = UIInputSystem.InputToWorldPosition(UnityEngine.Input.mousePosition);
            var currentTime = Time.timeSinceLevelLoadAsDouble;
            
            foreach (var entity in entities.Value)
            {
                ref var isDraggeing = ref entities.Pools.Inc2.Get(entity);
                ref var reGameObject = ref entities.Pools.Inc3.Get(entity);
                var gameObject = reGameObject.reference;
                var position = GridUtils.SnapToGrid(cursorPosition);

                if (world.HasComponent<LinearMovementToTarget>(entity))
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
                
                // todo ограничить перемещение башни крайними координатами уровня
                // todo добавить проверку можно/или нет ставить на клетку
                
                // todo отслеживания мыши и клавиатуры вынести в систему?

                var removeIsDraging = false;
                
                switch (isDraggeing.mode)
                {
                    case IsDraggingMode.None:
                        if (UnityEngine.Input.GetMouseButtonUp(0))
                        {
                            var deltaTime = currentTime - isDraggeing.startedTime;
                            if (deltaTime < Constants.UI.DragNDrop.TimeForAwaitDown)
                            {
                                isDraggeing.mode = IsDraggingMode.Down;
                            }
                            else
                            {
                                removeIsDraging = true;
                            }
                        }
                        break;
                    
                    case IsDraggingMode.Down:
                        if (UnityEngine.Input.GetMouseButtonDown(0))
                        {
                            isDraggeing.mode = IsDraggingMode.Up;
                        }
                        break;

                    case IsDraggingMode.Up:
                        removeIsDraging = true;
                        break;
                }

                if (removeIsDraging)
                {
                    world.DelComponent<IsDragging>(entity);
                    world.DelComponent<LinearMovementToTarget>(entity);
                    systems.SendOuter<DragEndEvent>();

                    gameObject.transform.position = position;
                }
            }
        }
    }
}