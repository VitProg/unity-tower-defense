using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Leopotam.EcsLite.Unity.Ugui;
using td.common.cells.interfaces;
using td.common.level;
using td.components;
using td.components.commands;
using td.components.events;
using td.components.flags;
using td.features.input;
using td.monoBehaviours;
using td.services;
using td.states;
using td.utils;
using td.utils.ecs;
using UnityEngine;
using UnityEngine.Scripting;
using Object = UnityEngine.Object;

namespace td.features.towers
{
    public class TowerBuySystem : EcsUguiCallbackSystem, IEcsInitSystem
    {
        [EcsInject] private LevelState levelState;
        [EcsInject] private LevelMap levelMap;

        [EcsWorld] private EcsWorld world;
        [EcsWorld(Constants.Worlds.Outer)] private EcsWorld outerWorld;

        [EcsUguiNamed(Constants.UI.Components.AddTowerButton)]
        private GameObject addTowerButton;

        private readonly EcsFilterInject<Inc<Tower, DragEndEvent, Ref<GameObject>>> dragEndEventEntities = default;
        private readonly EcsFilterInject<Inc<Tower, IsDragging, Ref<GameObject>>> draggableEntities = default;

        private IEcsSystems systems;
        private GameObject buildingsContainer;

        public override void Run(IEcsSystems systems)
        {
            base.Run(systems);

            foreach (var draggableEntity in draggableEntities.Value)
            {
                ref var refGameObject = ref draggableEntities.Pools.Inc3.Get(draggableEntity);

                var position = refGameObject.reference.transform.position;
                var cell = levelMap.GetCell<ICellCanBuild>(position);
                var canBuild = cell is { HasBuilding: false };

                //todo
                // var c = refGameObject.reference.GetComponentInChildren(typeof(SpriteRenderer));
                // if (c is SpriteRenderer spriteRenderer)
                // {
                //     spriteRenderer.color = cell == null ? Color.red : Color.white;
                // }

                if (levelMap.GridRenderer)
                {
                    levelMap.GridRenderer.State = canBuild ? GridHightlightState.Fine : GridHightlightState.Error;
                }
                
                if (canBuild)
                {
                    world.DelComponent<IsUnableToDrop>(draggableEntity);
                }
                else
                {
                    world.AddComponent<IsUnableToDrop>(draggableEntity);
                }
            }
            
            //////////////////

            foreach (var draggableEntity in dragEndEventEntities.Value)
            {
                ref var tower = ref draggableEntities.Pools.Inc1.Get(draggableEntity);
                ref var refGameObject = ref draggableEntities.Pools.Inc3.Get(draggableEntity);

                var position = refGameObject.reference.transform.position;
                var cell = levelMap.GetCell<ICellCanBuild>(position);

                // todo надо бы проверять до того как начали "тянуть" башню
                if (cell is { HasBuilding: false } && levelState.Money - tower.cost >= 0)
                {
                    levelState.Money -= tower.cost;
                    cell.BuildingPackedEntity = world.PackEntity(draggableEntity);
                }
                else
                {
                    world.AddComponent<IsDisabled>(draggableEntity);
                    world.AddComponent<RemoveGameObjectCommand>(draggableEntity);
                }

                world.DelComponent<DragEndEvent>(draggableEntity);
                world.DelComponent<IsUnableToDrop>(draggableEntity);
                world.DelComponent<IsDragging>(draggableEntity);
                world.DelComponent<IsSmoothDragging>(draggableEntity);
                world.DelComponent<IsDisabled>(draggableEntity);
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

            var position = CameraUtils.ToWorldPoint(e.Position);
            var prefab = Resources.Load<GameObject>("Prefabs/buildings/tower_v1");
            var gameObject = Object.Instantiate(prefab, position, Quaternion.identity, buildingsContainer.transform);
            var entity = world.ConvertToEntity(gameObject);
            ref var tower = ref world.GetComponent<Tower>(entity);

            // todo
            tower.cost = 5;
            
            var radiusTransform = gameObject.transform.Find("radius");
            if (radiusTransform != null)
            {
                tower.radiusGameObject = radiusTransform.gameObject;
                tower.radiusGameObject.SetActive(false);
            }
            
            gameObject.transform.localScale = Vector3.one * levelMap.CellSize;

            if (levelMap.CellType == LevelCellType.Hex)
            {
                var sprite = gameObject.transform.Find("sprite");
                if (sprite != null)
                {
                    sprite.localScale = Vector3.one * .55f;
                }
            }

            DragNDropSystem.BeginDrag(
                systems,
                entity,
                true,
                Constants.UI.DragNDrop.Smooth
            );
        }


        public void Init(IEcsSystems systems)
        {
            this.systems = systems;
        }
    }
}