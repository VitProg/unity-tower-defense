using System;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.common;
using td.components.behaviors;
using td.components.flags;
using td.components.refs;
using td.features.dragNDrop;
using td.features.shards.commands;
using td.features.shards.events;
using td.features.shards.flags;
using td.features.shards.mb;
using td.features.towers;
using td.monoBehaviours;
using td.services;
using td.services.ecsConverter;
using td.utils;
using td.utils.ecs;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace td.features.shards
{
    public class ShardDragNDropSystem : IEcsRunSystem, IEcsInitSystem
    {
        [Inject] private EntityConverters converters;
        [Inject] private LevelMap levelMap;
        [InjectShared] private SharedData shared;
        [InjectWorld] private EcsWorld world;
        [InjectWorld(Constants.Worlds.Outer)] private EcsWorld outerWorld;

        private readonly EcsFilterInject<Inc<UIShardDownEvent>> downEventEntities = Constants.Worlds.Outer;

        private readonly EcsFilterInject<Inc<Shard, DragEndEvent, DraggingStartedData, Ref<GameObject>>, Exc<IsDestroyed, IsRollbackDragging>> dragEndEventEntities = default;
        
        private readonly EcsFilterInject<Inc<Shard, DragRollbackEvent>, Exc<IsDestroyed, IsRollbackDragging>> rollbackEntities = default;

        private readonly EcsFilterInject<Inc<Shard, IsDragging, DraggingStartedData, Ref<GameObject>>, Exc<IsDestroyed>> draggableEntities = default;

        private readonly EcsFilterInject<Inc<Shard, ShardIsHovered, Ref<GameObject>>, Exc<IsDisabled, IsHidden, IsDragging, DragEndEvent>> hoveredShardEntities = default;
        
        private readonly EcsFilterInject<Inc<Shard, ShardInCollection, IsHidden, Ref<GameObject>>, Exc<IsDestroyed, IsDisabled>> hiddenEntities = default;

        private GameObject buildingsContainer;
        private GameObject shardPrefab;

        public void Run(IEcsSystems systems)
        {
            if (buildingsContainer == null)
            {
                buildingsContainer = GameObject.FindGameObjectWithTag(Constants.Tags.BuildingsContainer);
            }

            DragStart(systems);
            Draging(systems);
            DragEnd(systems);
            RollbackEnd(systems);
        }


        private void DragStart(IEcsSystems systems)
        {
            foreach (var downEventEntity in downEventEntities.Value)
            {
                ref var downEvent = ref downEventEntities.Pools.Inc1.Get(downEventEntity);

                if (downEvent.packedEntity.Unpack(world, out var shardEntity) &&
                    shared.draggableShardPackedEntity.Unpack(world, out var draggableShardEntity))
                {
                    ref var shard = ref world.GetComponent<Shard>(shardEntity);
                    var shardGO = world.GetComponent<Ref<GameObject>>(shardEntity).reference;
                    
                    ref var draggableShard = ref world.GetComponent<Shard>(draggableShardEntity);
                    var draggableShardGO = world.GetComponent<Ref<GameObject>>(draggableShardEntity).reference;
                    
                    ShardUtils.Copy(ref draggableShard, ref shard);
                    draggableShardGO.SetActive(true);
                    draggableShardGO.GetComponent<ShardMonoBehaviour>().UpdateFromEntity();
                    draggableShardGO.transform.position = shardGO.transform.position;
                    
                    shardGO.SetActive(false);
                    world.GetComponent<IsHidden>(shardEntity);

                    DragNDropCameraSystem.BeginDrag(
                        systems,
                        draggableShardEntity,
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
                var position = draggableEntities.Pools.Inc4.Get(draggableEntity).reference.transform.position;

                // ToDo add check with hovered shard!!
                
                var canDrop = false;

                var cell = levelMap.GetCell(
                    CameraUtils.ToWorldPoint(position),
                    CellTypes.CanBuild
                );

                if (cell != null && cell.TryGetBuilding<ShardTower>(world, out var towerEntity, out var shardTower))
                {
                    canDrop = !ShardTowerUtils.HasShard(world, ref shardTower);
                }

                if (shared.hightlightGrid)
                {
                    shared.hightlightGrid.state = canDrop ? GridHightlightState.Fine : GridHightlightState.Error;
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
                ref var draggingStartedData = ref dragEndEventEntities.Pools.Inc3.Get(draggableEntity);
                ref var shardRefGameObject = ref dragEndEventEntities.Pools.Inc4.Get(draggableEntity);

                var shardScreenPosition = shardRefGameObject.reference.transform.position;
                var shardPosition = CameraUtils.ToWorldPoint(shardScreenPosition);

                var dropped = false;

                // +todo is can't drop, need to move back to collection cell
                // todo is droped success - neeed to move dragged shard back to collection and reset those state, and reset collection cell state (set hasShard = false)
                // todo after need to refresh collection panel

                Debug.Log("HOVERED: " + hoveredShardEntities.Value.GetEntitiesCount());
                // foreach (var a in hoveredShardEntities.Value)
                // {
                    // var o = new object[10];
                    // Debug.Log("HOVERED:" + a);
                    // Debug.Log(world.GetComponents(a, ref o));
                    // foreach (var o1 in o)
                    // {
                        // Debug.Log(o1?.GetType());   
                    // }
                // }
                // Debug.Log("======");

                // if we droped shardPackedEntity to another shardPackedEntity
                // todo
                // if (hoveredShardEntities.Value.GetEntitiesCount() == 1)
                // {
                //     foreach (var hoveredShardEntity in hoveredShardEntities.Value)
                //     {
                //         ref var hoveredShard = ref hoveredShardEntities.Pools.Inc1.Get(hoveredShardEntity);
                //         dropped = DropToShard(ref hoveredShard, ref sourceShard, hoveredShardEntity);
                //         break;
                //     }
                // }
                // else
                // {
                    var cell = levelMap.GetCell(shardPosition, CellTypes.CanBuild);
                    dropped = DropToCell(ref sourceShard, cell);
                // }

                if (dropped)
                {
                    shared.draggableShard.gameObject.SetActive(false);
                    systems.OuterSingle<ShardCollectionRemoveHiddenOuterCommand>();
                }
                else
                {
                    world.GetComponent<IsRollbackDragging>(draggableEntity);
                    ref var target = ref world.GetComponent<LinearMovementToTarget>(draggableEntity);
                    target.from = shardRefGameObject.reference.transform.position;
                    target.target = draggingStartedData.startedPosition;
                    target.speed = Constants.UI.DragNDrop.RollbackSpeed;
                }
            }
        }
        
        private void RollbackEnd(IEcsSystems systems)
        {
            foreach (var rollbackEntity in rollbackEntities.Value)
            {
                shared.draggableShard.gameObject.SetActive(false);

                foreach (var entity in hiddenEntities.Value)
                {
                    hiddenEntities.Pools.Inc4.Get(entity).reference.SetActive(true);
                    world.DelComponent<IsHidden>(entity);
                }
                break;
            }
        }

        private bool DropToCell(ref Shard sourceShard, Cell cell)
        {
            if (
                cell == null ||
                !cell.TryGetBuildngEntity(world, out var towerEntity) ||
                !world.HasComponent<Tower>(towerEntity) ||
                !world.HasComponent<ShardTower>(towerEntity) ||
                !world.HasComponent<Ref<GameObject>>(towerEntity)
            )
            {
                return false;
            }
            
            ref var tower = ref world.GetComponent<Tower>(towerEntity);
            ref var shardTower = ref world.GetComponent<ShardTower>(towerEntity);
            ref var towerGORef = ref world.GetComponent<Ref<GameObject>>(towerEntity);
            
            if (ShardTowerUtils.HasShard(world, ref shardTower))
            {
                ref var targetShard = ref ShardTowerUtils.GetShard(world, ref shardTower, out var targetShardEntity);
                DropToShard(ref targetShard, ref sourceShard, targetShardEntity);
                return true;
            }
                
            // todo add animation and FX
            // todo add delay to activate shard in tower

            var shardPosition = towerGORef.reference.transform.position + (Vector3)tower.barrel;
            shardPosition.z = -0.01f;
            
            // todo make new shard based on droped shard and pass it into tower !!!
                
            var shardGO = Object.Instantiate(shardPrefab, shardPosition, Quaternion.identity, towerGORef.reference.transform);
            if (!converters.Convert<Shard>(shardGO, out var shardEntity))
            {
                throw new NullReferenceException($"Failed to convert GameObject {shardGO.name}");
            }
                
            var scale = shardGO.transform.localScale;
            shardGO.transform.localScale = new Vector2(scale.x, scale.y * 0.85f);
                
            ref var shard = ref world.GetComponent<Shard>(shardEntity);
            ShardUtils.Copy(ref shard, ref sourceShard);
            var shardMonoBehaviour = shardGO.GetComponent<ShardMonoBehaviour>();
            shardMonoBehaviour.UpdateFromEntity();

            world.GetComponent<ShardInTower>(shardEntity).towerPackedEntity = world.PackEntity(towerEntity);

            shardTower.shardPackedEntity = world.PackEntity(shardEntity);

            return true;
        }

        private bool DropToShard(ref Shard targetShard, ref Shard sourceShard, int targetEntity)
        {
            ShardUtils.MixShards(ref targetShard, ref sourceShard);

            // todo add animation and FX

            ref var targetGameObjectRef = ref world.GetComponent<Ref<GameObject>>(targetEntity);
            var targetMb = targetGameObjectRef.reference.GetComponent<ShardMonoBehaviour>();
            targetMb.UpdateFromEntity();

            return true;
        }

        public void Init(IEcsSystems systems)
        {
            shardPrefab = Resources.Load<GameObject>("Prefabs/Shard");
        }
    }
}