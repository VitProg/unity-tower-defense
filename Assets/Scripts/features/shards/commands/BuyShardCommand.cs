using Leopotam.EcsLite;

namespace td.features.shards.commands
{
    public struct BuyShardCommand
    {
        public EcsPackedEntity shardPackedEntity;
        public int cost;
    }
}