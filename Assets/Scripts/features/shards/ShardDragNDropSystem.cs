using System;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.common;
using td.components.commands;
using td.components.flags;
using td.components.refs;
using td.features.dragNDrop;
using td.features.shards.mb;
using td.features.shards.ui;
using td.features.towers;
using td.monoBehaviours;
using td.services;
using td.services.ecsConverter;
using td.utils;
using td.utils.ecs;
using UnityEngine;
using Object = UnityEngine.Object;

namespace td.features.shards
{
    public class ShardDragNDropSystem : IEcsRunSystem
    {
        [Inject] private EntityConverters converters;
        [Inject] private LevelMap levelMap;
        [InjectShared] private SharedData sharedData;
        [InjectWorld] private EcsWorld world;
        [InjectWorld(Constants.Worlds.Outer)] private EcsWorld outerWorld;

        private readonly EcsFilterInject<Inc<ShardUIDownEvent>> downEventEntities = Constants.Worlds.Outer;

        private readonly EcsFilterInject<Inc<Shard, ShardUICell, DragEndEvent, Ref<GameObject>>, Exc<IsDestroyed>>
            dragEndEventEntities = default;

        private readonly EcsFilterInject<Inc<Shard, IsDragging, Ref<GameObject>>, Exc<IsDestroyed>> draggableEntities =
            default;

        private readonly EcsFilterInject<Inc<Shard, ShardUIIsHovered, Ref<GameObject>>, Exc<IsDestroyed, IsDragging, DragEndEvent>>
            hoveredShardEntities = default;

        private GameObject buildingsContainer;

        public void Run(IEcsSystems systems)
        {
            if (buildingsContainer == null)
            {
                buildingsContainer = GameObject.FindGameObjectWithTag(Constants.Tags.BuildingsContainer);
            }

            DragStart(systems);
            Draging(systems);
            DragEnd(systems);
        }

        private void DragStart(IEcsSystems systems)
        {
            foreach (var downEventEntity in downEventEntities.Value)
            {
                ref var downEvent = ref downEventEntities.Pools.Inc1.Get(downEventEntity);

                if (downEvent.packedEntity.Unpack(world, out var entity))
                {
                    ref var gameObjectRef = ref world.GetComponent<Ref<GameObject>>(entity);

                    if (!converters.ConvertForEntity<Shard>(gameObjectRef.reference, entity))
                    {
                        throw new NullReferenceException(
                            $"Failed to convert GameObject {gameObjectRef.reference.name}");
                    }
                    
                    world.GetComponent<ShardUICell>(entity).cell = gameObjectRef.reference.transform.parent.gameObject;

                    DragNDropCameraSystem.BeginDrag(
                        systems,
                        entity,
                        Constants.UI.DragNDrop.Smooth
                    );
                }

                Debug.Log("Shard Start DrugNDrop");
            }

            systems.CleanupOuter(downEventEntities);
        }

        private void Draging(IEcsSystems systems)
        {
            foreach (var draggableEntity in draggableEntities.Value)
            {
                ref var refGameObject = ref draggableEntities.Pools.Inc3.Get(draggableEntity);

                var screenPosition = refGameObject.reference.transform.position;

                var canDrop = hoveredShardEntities.Value.GetEntitiesCount() == 1;

                if (!canDrop)
                {
                    var position = CameraUtils.ToWorldPoint(screenPosition);
                    var cell = levelMap.GetCell(position, CellTypes.CanBuild);
                    canDrop = cell && cell.buildings[0].HasValue;

                    if (canDrop && cell.buildings[0].Value.Unpack(world, out var buildingEntity))
                    {
                        if (!world.HasComponent<ShardTower>(buildingEntity))
                        {
                            canDrop = false;
                        }
                        //todo check if tower alredy have shard
                    }
                }

                if (sharedData.HightlightGrid)
                {
                    sharedData.HightlightGrid.state = canDrop ? GridHightlightState.Fine : GridHightlightState.Error;
                }

                if (canDrop)
                {
                    world.DelComponent<IsUnableToDrop>(draggableEntity);
                }
                else
                {
                    world.GetComponent<IsUnableToDrop>(draggableEntity);
                }
            }
        }

        private void DragEnd(IEcsSystems systems)
        {
            foreach (var draggableEntity in dragEndEventEntities.Value)
            {
                ref var sourceShard = ref dragEndEventEntities.Pools.Inc1.Get(draggableEntity);
                ref var sourceShardCell = ref dragEndEventEntities.Pools.Inc2.Get(draggableEntity);
                ref var shardRefGameObject = ref dragEndEventEntities.Pools.Inc4.Get(draggableEntity);

                var shardScreenPosition = shardRefGameObject.reference.transform.position;
                var shardPosition = CameraUtils.ToWorldPoint(shardScreenPosition);

                world.GetComponent<IsDisabled>(draggableEntity);
                world.GetComponent<RemoveGameObjectCommand>(draggableEntity);
                
                if (sourceShardCell.cell)
                {
                    var cellEntity = world.NewEntity();
                    world.GetComponent<Ref<GameObject>>(cellEntity).reference = sourceShardCell.cell;
                    world.GetComponent<RemoveGameObjectCommand>(cellEntity);
                }

                // todo если не получилось скинуть, то надо плавно вернуть на место

                // if we droped shard to another shard
                if (hoveredShardEntities.Value.GetEntitiesCount() == 1)
                {
                    foreach (var hoveredShardEntity in hoveredShardEntities.Value)
                    {
                        ref var hoveredShard = ref hoveredShardEntities.Pools.Inc1.Get(hoveredShardEntity);
                        DropToShard(ref hoveredShard, ref sourceShard, hoveredShardEntity);
                        break;
                    }
                }
                else
                {
                    var cell = levelMap.GetCell(shardPosition, CellTypes.CanBuild);
                    DropToCell(ref sourceShard, cell);
                }
            }
        }

        private void DropToCell(ref Shard sourceShard, Cell cell)
        {
            if (!cell ||
                !cell.buildings[0].HasValue || !cell.buildings[0].Value.Unpack(world, out var buildingEntity) ||
                !world.HasComponent<Tower>(buildingEntity))
            {
                return;
            }

            // todo check if tower already have shard
            // todo add animation and FX

            ref var tower = ref world.GetComponent<Tower>(buildingEntity);
            ref var shardTower = ref world.GetComponent<ShardTower>(buildingEntity);
            ref var towerGORef = ref world.GetComponent<Ref<GameObject>>(buildingEntity);

            ////

            var shardPosition = towerGORef.reference.transform.position + (Vector3)tower.barrel;
            shardPosition.z = -0.01f;

            var shardPrefab = Resources.Load<GameObject>("Prefabs/Shard");
            var shardGO = Object.Instantiate(shardPrefab, shardPosition, Quaternion.identity,
                towerGORef.reference.transform);
            if (!converters.Convert<Shard>(shardGO, out var shardEntity))
            {
                throw new NullReferenceException($"Failed to convert GameObject {shardGO.name}");
            }

            var scale = shardGO.transform.localScale;
            shardGO.transform.localScale = new Vector2(scale.x, scale.y * 0.85f);

            ref var shardInTower = ref world.GetComponent<Shard>(shardEntity);
            shardInTower = sourceShard;
            var shardMonoBehaviour = shardGO.GetComponent<ShardMonoBehaviour>();
            shardMonoBehaviour.UpdateFromEntity();

            shardTower.shard = world.PackEntity(shardEntity);
        }

        private void DropToShard(ref Shard targetShard, ref Shard sourceShard, int targetEntity)
        {
            ShardUtils.MixShards(ref targetShard, ref sourceShard);
            
            // todo add animation and FX
            
            ref var targetGameObjectRef = ref world.GetComponent<Ref<GameObject>>(targetEntity);
            var targetMb = targetGameObjectRef.reference.GetComponent<ShardMonoBehaviour>();
            targetMb.UpdateFromEntity();
        }
    }
}