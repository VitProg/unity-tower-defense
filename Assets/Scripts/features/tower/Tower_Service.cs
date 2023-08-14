using System;
using Leopotam.EcsProto.QoL;
using td.features._common.components;
using td.features.tower.components;
using td.features.tower.mb;
using td.utils.ecs;

namespace td.features.tower
{
    public class Tower_Service
    {
        [DI] private Tower_Aspect aspect;

        public bool HasTower(ProtoPackedEntity packedEntity) => packedEntity.Unpack(aspect.World(), out var entity) && HasTower(entity);
        public bool HasTower(ProtoPackedEntity packedEntity, out int towerEntity) => packedEntity.Unpack(aspect.World(), out towerEntity) && HasTower(towerEntity);
        public bool HasTower(ProtoPackedEntityWithWorld packedEntity, out int towerEntity) => packedEntity.Unpack(out var w, out towerEntity) && aspect.World().Equals(w) && HasTower(towerEntity);
        public bool HasTower(int entity) => aspect.towerPool.Has(entity);
        public ref Tower GetTower(ProtoPackedEntity packedEntity, out int towerEntity)
        {
            if (!packedEntity.Unpack(aspect.World(), out towerEntity)) throw new Exception("Can't unpack Tower entity");
            return ref GetTower(towerEntity);
        }
        public ref Tower GetTower(int entity) => ref aspect.towerPool.GetOrAdd(entity);

        public bool HasShardTower(ProtoPackedEntityWithWorld packedEntity) => packedEntity.Unpack(out var w, out var towerEntity) && aspect.World().Equals(w) && HasShardTower(towerEntity);
        public bool HasShardTower(ProtoPackedEntity packedEntity) => packedEntity.Unpack(aspect.World(), out var towerEntity) && HasShardTower(towerEntity);
        public bool HasShardTower(int entity) => aspect.shardTowerPool.Has(entity);
        public ref ShardTower GetShardTower(ProtoPackedEntity packedEntity)
        {
            if (!packedEntity.Unpack(aspect.World(), out var entity)) throw new NullReferenceException();
            return ref GetShardTower(entity);
        }

        public ref ShardTower GetShardTower(int entity) => ref aspect.shardTowerPool.GetOrAdd(entity);
        
        public ref TowerTarget GetTowerTarget(int entity) => ref aspect.towerTargetPool.GetOrAdd(entity);
        public void RemoveTowerTarget(int entity) => aspect.towerTargetPool.Del(entity);
        public bool HasTowerTarget(int entity) => aspect.towerTargetPool.Has(entity);
        
        public ref Ref<ShardTowerMonoBehaviour> GetShardTowerMBRef(int enemyEntity) => ref aspect.refShardTowerMB.GetOrAdd(enemyEntity);
        public ShardTowerMonoBehaviour GetShardTowerMB(int enemyEntity) => GetShardTowerMBRef(enemyEntity).reference!;

        public ref Ref<TowerMonoBehaviour> GetTowerMBRef(int enemyEntity) => ref aspect.refTowerMB.GetOrAdd(enemyEntity);
        public TowerMonoBehaviour GetTowerMB(int enemyEntity) => GetTowerMBRef(enemyEntity).reference!;
    }
}