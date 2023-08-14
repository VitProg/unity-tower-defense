using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using td.features._common.components;
using td.features.shard.components;
using td.features.shard.mb;
using td.features.tower.components;

namespace td.features.shard
{
    public class Shard_Aspect : ProtoAspectInject
    {
        public ProtoPool<Shard> shardPool;
        public ProtoPool<Ref<ShardMonoBehaviour>> shardRefMBPool;
        public ProtoPool<ShardTowerWithShard> shardTowerWithShardPool;
    }
}