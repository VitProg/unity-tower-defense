using System;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.common;
using td.components.commands;
using td.components.events;
using td.components.flags;
using td.components.refs;
using td.features.dragNDrop;
using td.features.state;
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
    public class TowerBuySystem : IEcsRunSystem
    {
        [Inject] private State state;
        [Inject] private LevelMap levelMap;
        [Inject] private EntityConverters converters;
        [Inject] private PrefabService prefabService;
        [InjectShared] private SharedData shared;

        [InjectWorld] private EcsWorld world;
        [InjectWorld(Constants.Worlds.Outer)] private EcsWorld outerWorld;

        [InjectSystems] private IEcsSystems systems;

        // [EcsUguiNamed(Constants.UI.Components.AddTowerButton)]
        // private GameObject addTowerButton;

        private readonly EcsFilterInject<Inc<Tower, DragEndEvent, Ref<GameObject>>, Exc<IsDestroyed>> dragEndEventEntities = default;
        private readonly EcsFilterInject<Inc<Tower, IsDragging, Ref<GameObject>>, Exc<IsDestroyed>> draggableEntities = default;

        private GameObject buildingsContainer;

        public void Run(IEcsSystems systems)
        {
            foreach (var draggableEntity in draggableEntities.Value)
            {
                ref var refGameObject = ref draggableEntities.Pools.Inc3.Get(draggableEntity);

                var position = refGameObject.reference.transform.position;
                var cell = levelMap.GetCell(position, CellTypes.CanBuild);
                var canBuild = cell && cell.HasBuilding() == false;
                
                if (shared.hightlightGrid)
                {
                    shared.hightlightGrid.state = canBuild ? GridHightlightState.Fine : GridHightlightState.Error;
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
                if (cell && !cell.HasBuilding() && state.Money - tower.cost >= 0)
                {
                    state.Money -= tower.cost;
                    // todo
                    cell.buildingPackedEntity = world.PackEntity(dragEndEntity);
                }
                else
                {
                    world.GetComponent<IsDisabled>(dragEndEntity);
                    world.GetComponent<RemoveGameObjectCommand>(dragEndEntity);
                }
                
                world.DelComponent<DragEndEvent>(dragEndEntity);
            }
        }

        // todo
        // [Preserve]
        // [EcsUguiDownEvent(Constants.UI.Components.AddTowerButton, Constants.Worlds.UI)]
        // private void OnBuyTowerDown(in EcsUguiDownEvent e)
        // {
        //     Debug.Log("OnBuyTowerDown");
        //     if (buildingsContainer == null)
        //     {
        //         buildingsContainer = GameObject.FindGameObjectWithTag(Constants.Tags.BuildingsContainer);
        //     }
        //     
        //     // todo
        //     var position = CameraUtils.ToWorldPoint(shared.canvasCamera, e.Position);
        //     var prefab = prefabService.GetPrefab(PrefabCategory.Buildings, "shard_tower");
        //     var gameObject = Object.Instantiate(prefab, position, Quaternion.identity, buildingsContainer.transform);
        //
        //     if (!converters.Convert<Tower>(gameObject, out var entity))
        //     {
        //         throw new NullReferenceException($"Failed to convert GameObject {gameObject.name}");
        //     }
        //     
        //     ref var tower = ref world.GetComponent<Tower>(entity);
        //     
        //     // todo
        //     tower.cost = 5;
        //     
        //     gameObject.transform.localScale = Vector3.one;
        //     
        //     DragNDropWorldSystem.BeginDrag(
        //         systems,
        //         entity,
        //         true,
        //         Constants.UI.DragNDrop.Smooth
        //     );
        // }
    }
}