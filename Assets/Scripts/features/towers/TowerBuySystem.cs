using System;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Leopotam.EcsLite.Unity.Ugui;
using td.common;
using td.components.commands;
using td.components.events;
using td.components.flags;
using td.components.refs;
using td.features.dragNDrop;
using td.monoBehaviours;
using td.services;
using td.services.ecsConverter;
using td.utils;
using td.utils.ecs;
using UnityEngine;
using UnityEngine.Scripting;
using Object = UnityEngine.Object;

namespace td.features.towers
{
    public class TowerBuySystem : EcsUguiCallbackSystem
    {
        [Inject] private LevelState levelState;
        [Inject] private LevelMap levelMap;
        [Inject] private EntityConverters converters;
        [InjectShared] private SharedData sharedData;

        [InjectWorld] private EcsWorld world;
        [InjectWorld(Constants.Worlds.Outer)] private EcsWorld outerWorld;

        [InjectSystems] private IEcsSystems systems;

        [EcsUguiNamed(Constants.UI.Components.AddTowerButton)]
        private GameObject addTowerButton;

        private readonly EcsFilterInject<Inc<Tower, DragEndEvent, Ref<GameObject>>, Exc<IsDestroyed>> dragEndEventEntities = default;
        private readonly EcsFilterInject<Inc<Tower, IsDragging, Ref<GameObject>>, Exc<IsDestroyed>> draggableEntities = default;

        private GameObject buildingsContainer;

        public override void Run(IEcsSystems systems)
        {
            base.Run(systems);
            
            foreach (var draggableEntity in draggableEntities.Value)
            {
                ref var refGameObject = ref draggableEntities.Pools.Inc3.Get(draggableEntity);

                var position = refGameObject.reference.transform.position;
                var cell = levelMap.GetCell(position, CellTypes.CanBuild);
                var canBuild = cell && cell.HasBuilding == false;
                
                if (sharedData.HightlightGrid)
                {
                    sharedData.HightlightGrid.state = canBuild ? GridHightlightState.Fine : GridHightlightState.Error;
                }
                
                if (canBuild)
                {
                    world.DelComponent<IsUnableToDrop>(draggableEntity);
                }
                else
                {
                    world.GetComponent<IsUnableToDrop>(draggableEntity);
                }
            }
            
            //////////////////

            foreach (var dragEndEntity in dragEndEventEntities.Value)
            {
                ref var tower = ref draggableEntities.Pools.Inc1.Get(dragEndEntity);
                ref var refGameObject = ref draggableEntities.Pools.Inc3.Get(dragEndEntity);

                var position = refGameObject.reference.transform.position;
                var cell = levelMap.GetCell(position, CellTypes.CanBuild);

                // todo надо бы проверять до того как начали "тянуть" башню
                if (cell is { HasBuilding: false } && levelState.Money - tower.cost >= 0)
                {
                    levelState.Money -= tower.cost;
                    // todo
                    cell.buildings[0] = world.PackEntity(dragEndEntity);
                }
                else
                {
                    world.GetComponent<IsDisabled>(dragEndEntity);
                    world.GetComponent<RemoveGameObjectCommand>(dragEndEntity);
                }
                
                world.DelComponent<DragEndEvent>(dragEndEntity);
            }
        }

        [Preserve]
        [EcsUguiDownEvent(Constants.UI.Components.AddTowerButton, Constants.Worlds.UI)]
        private void OnBuyTowerDown(in EcsUguiDownEvent e)
        {
            Debug.Log("OnBuyTowerDown");
            if (buildingsContainer == null)
            {
                buildingsContainer = GameObject.FindGameObjectWithTag(Constants.Tags.BuildingsContainer);
            }
            
            // todo
            var position = CameraUtils.ToWorldPoint(e.Position);
            var prefab = Resources.Load<GameObject>("Prefabs/buildings/shard_tower");
            var gameObject = Object.Instantiate(prefab, position, Quaternion.identity, buildingsContainer.transform);

            if (!converters.Convert<Tower>(gameObject, out var entity))
            {
                throw new NullReferenceException($"Failed to convert GameObject {gameObject.name}");
            }
            
            ref var tower = ref world.GetComponent<Tower>(entity);
            
            // todo
            tower.cost = 5;
            
            gameObject.transform.localScale = Vector3.one;
            
            DragNDropWorldSystem.BeginDrag(
                systems,
                entity,
                true,
                Constants.UI.DragNDrop.Smooth
            );
        }
    }
}