using td.features.eventBus.types;
using Unity.Mathematics;

namespace td.features.shard.bus
{
    public struct Command_DropShard_OnMap : IGlobalEvent
    {
        public int sourceIndex;
        public float2 position;
        public uint cost;
    }
}