using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Leopotam.EcsLite.Unity.Ugui;
using td.common.level;
using td.components;
using td.components.behaviors;
using td.components.events;
using td.components.flags;
using td.features.towers;
using td.services;
using td.states;
using td.utils;
using td.utils.ecs;
using UnityEngine;
using UnityEngine.Scripting;
using Object = UnityEngine.Object;

namespace td.features.input
{
    public class TowerBuySystem: EcsUguiCallbackSystem, IEcsInitSystem
    {
        [EcsInject] private LevelState levelState;
        [EcsInject] private LevelMap levelMap;

        [EcsWorld] private EcsWorld world;
        [EcsWorld(Constants.Worlds.Outer)] private EcsWorld outerWorld;

        [EcsUguiNamed(Constants.UI.Components.AddTowerButton)] private GameObject addTowerButton;

        private readonly EcsFilterInject<Inc<Tower, IsDragging, Ref<GameObject>>> entities = default;
        
        private IEcsSystems systems;
        private GameObject buildingsContainer;

        public override void Run(IEcsSystems systems)
        {
            base.Run(systems);
            
            var cursorPosition = InputToWorldPosition(UnityEngine.Input.mousePosition);
            var currentTime = Time.timeSinceLevelLoadAsDouble;
            
            foreach (var entity in entities.Value)
            {
                ref var isDraggeing = ref entities.Pools.Inc2.Get(entity);
                ref var reGameObject = ref entities.Pools.Inc3.Get(entity);
                var gameObject = reGameObject.reference;
                var position = GridUtils.SnapToGrid(cursorPosition, levelMap.CellType, levelMap.CellSize);

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
        
        [Preserve]
        [EcsUguiDownEvent(Constants.UI.Components.AddTowerButton, Constants.Worlds.UI)]
        private void OnBuyTowerDown(in EcsUguiDownEvent e)
        {
            if (buildingsContainer == null)
            {
                buildingsContainer = GameObject.FindGameObjectWithTag(Constants.Tags.BuildingsContainer);
            }
            
            var position = InputToWorldPosition(e.Position);
            var prefab = Resources.Load<GameObject>("Prefabs/buildings/tower_v1");
            var gameObject = Object.Instantiate(prefab, position, Quaternion.identity, buildingsContainer.transform);
            var entity = world.ConvertToEntity(gameObject);
            world.AddComponent<IsDragging>(entity).startedTime = Time.timeSinceLevelLoadAsDouble;
            systems.SendOuter<DragStartEvent>();

            gameObject.transform.localScale = Vector3.one * levelMap.CellSize;

            if (levelMap.CellType == LevelCellType.Hex)
            {
                var sprite = gameObject.transform.Find("sprite");
                if (sprite != null)
                {
                    sprite.localScale = Vector3.one * .55f;
                }
            }

            if (Constants.UI.DragNDrop.Smooth)
            {
                world.AddComponent(entity, new LinearMovementToTarget()
                {
                    gap = Constants.DefaultGap,
                    speed = Constants.UI.DragNDrop.SmoothSpeed,
                    target = position
                });
            }
        }

        private static Vector3 InputToWorldPosition(Vector2 inputPos) {
            Vector3 pos = new Vector3(inputPos.x, inputPos.y, 
                -Camera.main.transform.position.z);
            return Camera.main.ScreenToWorldPoint(pos);
        }

        public void Init(IEcsSystems systems)
        {
            this.systems = systems;
        }
    }
}