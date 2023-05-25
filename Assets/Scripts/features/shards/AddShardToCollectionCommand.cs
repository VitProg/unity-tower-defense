using Leopotam.EcsLite;

namespace td.features.shards
{
    public struct AddShardToCollectionCommand
    {
        public EcsPackedEntity shardPackedEntity;
        public uint cost;
    }
}