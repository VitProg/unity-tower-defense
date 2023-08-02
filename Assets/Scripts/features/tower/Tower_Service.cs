using System;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.features._common.components;
using td.features.shard;
using td.features.tower.components;
using td.features.tower.mb;
using td.utils.ecs;

namespace td.features.tower
{
    public class Tower_Service
    {
        private readonly EcsInject<Tower_Pools> pools;
        
        private readonly EcsWorldInject world;

        public bool HasTower(EcsPackedEntity packedEntity) => packedEntity.Unpack(world.Value, out var entity) && HasTower(entity);
        public bool HasTower(EcsPackedEntity packedEntity, out int towerEntity) => packedEntity.Unpack(world.Value, out towerEntity) && HasTower(towerEntity);
        public bool HasTower(int entity) => pools.Value.towerPool.Value.Has(entity);
        public ref Tower GetTower(EcsPackedEntity packedEntity, out int towerEntity)
        {
            if (!packedEntity.Unpack(world.Value, out towerEntity)) throw new Exception("Can't unpack Tower entity");
            return ref GetTower(towerEntity);
        }
        public ref Tower GetTower(int entity) => ref pools.Value.towerPool.Value.GetOrAdd(entity);

        public bool HasShardTower(EcsPackedEntity packedEntity) => packedEntity.Unpack(world.Value, out var towerEntity) && HasShardTower(towerEntity);
        public bool HasShardTower(int entity) => pools.Value.shardTowerPool.Value.Has(entity);
        public ref ShardTower GetShardTower(EcsPackedEntity packedEntity)
        {
            if (!packedEntity.Unpack(world.Value, out var entity)) throw new NullReferenceException();
            return ref GetShardTower(entity);
        }

        public ref ShardTower GetShardTower(int entity) => ref pools.Value.shardTowerPool.Value.GetOrAdd(entity);
        
        public ref TowerTarget GetTowerTarget(int entity) => ref pools.Value.towerTargetPool.Value.GetOrAdd(entity);
        public void RemoveTowerTarget(int entity) => pools.Value.towerTargetPool.Value.SafeDel(entity);
        public bool HasTowerTarget(int entity) => pools.Value.towerTargetPool.Value.Has(entity);
        
        public ref Ref<ShardTowerMonoBehaviour> GetShardTowerMBRef(int enemyEntity) => ref pools.Value.refShardTowerMB.Value.GetOrAdd(enemyEntity);
        public ShardTowerMonoBehaviour GetShardTowerMB(int enemyEntity) => GetShardTowerMBRef(enemyEntity).reference!;

        public ref Ref<TowerMonoBehaviour> GetTowerMBRef(int enemyEntity) => ref pools.Value.refTowerMB.Value.GetOrAdd(enemyEntity);
        public TowerMonoBehaviour GetTowerMB(int enemyEntity) => GetTowerMBRef(enemyEntity).reference!;
    }
}