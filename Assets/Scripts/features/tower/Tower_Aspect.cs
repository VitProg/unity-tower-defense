using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using td.features._common.components;
using td.features.destroy.flags;
using td.features.tower.components;
using td.features.tower.mb;

namespace td.features.tower
{
    public class Tower_Aspect: ProtoAspectInject
    {
        // public ProtoPool<Tower> towerPool;
        public ProtoPool<ShardTower> shardTowerPool;
        // public ProtoPool<ShardTowerWithShard> shardTowerWithShardPool;
        public ProtoPool<TowerTarget> towerTargetPool;
        // public ProtoPool<Ref<TowerMonoBehaviour>> refTowerMB;
        public ProtoPool<Ref<ShardTowerMonoBehaviour>> refShardTowerMB;

        // public readonly ProtoItExc itTower = new ProtoItExc(
            // It.Inc<Tower>(),
            // It.Exc<IsDestroyed, IsDisabled>()
        // );

        public readonly ProtoItExc itShardTowerWithTarget = new (
            It.Inc<ShardTower, TowerTarget>(),
            It.Exc<IsDestroyed, IsDisabled>()
        );
        
        public readonly ProtoItExc itShardTower = new (
            It.Inc<ShardTower>(),
            It.Exc<IsDestroyed, IsDisabled>()
        );
    }
}