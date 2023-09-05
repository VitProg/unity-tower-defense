using System;
using System.Runtime.CompilerServices;
using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using td.features._common.components;
using td.features.prefab;
using td.features.tower.components;
using td.features.tower.mb;
using td.utils;
using td.utils.di;
using td.utils.ecs;
using Unity.Mathematics;
using UnityEngine;
using Object = UnityEngine.Object;

namespace td.features.tower
{
    public class Tower_Service
    {
        [DI] private Tower_Aspect aspect;
        [DI] private Tower_Converter converter;

        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        // public bool HasTower(ProtoPackedEntityWithWorld packedEntity, out ProtoWorld world, out int towerEntity) => packedEntity.Unpack(out world, out towerEntity) && aspect.World().Equals(world) && HasTower(towerEntity);
        //
        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        // public bool HasTower(ProtoPackedEntityWithWorld packedEntity) => HasTower(packedEntity, out var w, out var e);
        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        // public bool HasTower(int entity) => aspect.towerPool.Has(entity);

//         [MethodImpl(MethodImplOptions.AggressiveInlining)]
//         public ref Tower GetTower(ProtoPackedEntityWithWorld packedEntity, out int towerEntity)
//         {
//             var check = packedEntity.Unpack(out var w, out towerEntity);
// #if UNITY_EDITOR
//             if (!check) throw new Exception("Can't unpack Tower entity");
// #endif
//             return ref GetTower(towerEntity);
//         }
        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        // public ref Tower GetTower(int entity) => ref aspect.towerPool.GetOrAdd(entity);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasShardTower(ProtoPackedEntityWithWorld packedEntity) => packedEntity.Unpack(out var w, out var towerEntity) && aspect.World().Equals(w) && HasShardTower(towerEntity);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasShardTower(int entity) => aspect.shardTowerPool.Has(entity);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref ShardTower GetShardTower(int entity) => ref aspect.shardTowerPool.GetOrAdd(entity);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref ShardTower GetShardTower(ProtoPackedEntityWithWorld packedEntity, out int towerEntity)
        {
            var check = packedEntity.Unpack(out var w, out towerEntity);
#if UNITY_EDITOR
            if (!check) throw new Exception("Can't unpack Tower entity");
#endif
            return ref GetShardTower(towerEntity);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref TowerTarget GetTowerTarget(int entity) => ref aspect.towerTargetPool.GetOrAdd(entity);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveTowerTarget(int entity) => aspect.towerTargetPool.Del(entity);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasTowerTarget(int entity) => aspect.towerTargetPool.Has(entity);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref Ref<ShardTowerMonoBehaviour> GetShardTowerMBRef(int enemyEntity) => ref aspect.refShardTowerMB.GetOrAdd(enemyEntity);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ShardTowerMonoBehaviour GetShardTowerMB(int enemyEntity) => GetShardTowerMBRef(enemyEntity).reference!;

        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        // public ref Ref<TowerMonoBehaviour> GetTowerMBRef(int enemyEntity) => ref aspect.refTowerMB.GetOrAdd(enemyEntity);
        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        // public TowerMonoBehaviour GetTowerMB(int enemyEntity) => GetTowerMBRef(enemyEntity).reference!;
        
        //

        private GameObject prefab;
        private GameObject container;
        
        public int SpawnShardTower(int2 cellCoords, uint? buildTime = 0)
        {
            Debug.Log("Tower_Service:SpawnShardTower");
            
            if (prefab == null)
            {
                Debug.Log("Tower_Service:SpawnShardTower - load prefab");
                var prefabService = ServiceContainer.Get<Prefab_Service>();
                prefab = prefabService.GetPrefab(PrefabCategory.Buildings, "shard_tower");
            }

            if (container == null || container.gameObject == null)
            {
                Debug.Log("Tower_Service:SpawnShardTower - find container");
                container = GameObject.FindGameObjectWithTag(Constants.Tags.BuildingsContainer);
                if (container == null)
                {
                    container = new GameObject
                    {
                        tag = Constants.Tags.BuildingsContainer
                    };
                }
            }

            var position = HexGridUtils.CellToPosition(cellCoords);

            var go = Object.Instantiate(prefab, position, Quaternion.identity, container.transform);
            var entity = converter.GetEntity(go) ?? aspect.World().NewEntity();
            Debug.Log("Tower_Service:SpawnShardTower - entity:" + entity);
            converter.Convert(go, entity);
            
            // todo usebuild time
            
            Debug.Log("Tower_Service:SpawnShardTower - FIN");
            
            return entity;
        }

    }
}