using Leopotam.EcsLite.Di;
using td.features._common;
using td.features._common.components;
using td.features.tower.components;
using td.features.tower.mb;

namespace td.features.tower
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class Tower_Pools
    {
        public readonly EcsPoolInject<Tower> towerPool = default;
        public readonly EcsPoolInject<ShardTower> shardTowerPool = default;
        public readonly EcsPoolInject<TowerTarget> towerTargetPool = default;
        public readonly EcsPoolInject<Ref<TowerMonoBehaviour>> refTowerMB = default;
        public readonly EcsPoolInject<Ref<ShardTowerMonoBehaviour>> refShardTowerMB = default;
    }
}