using td.features.eventBus.types;

namespace td.features.shard.bus
{
    public struct Command_CombineShards_InCollection : IGlobalEvent
    {
        public int sourceIndex;
        public int targetIndex;
        public uint cost;
        public uint time;
    }
}