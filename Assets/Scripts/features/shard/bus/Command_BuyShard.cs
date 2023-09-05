using td.features.eventBus.types;
using td.features.shard.components;

namespace td.features.shard.bus
{
    public struct Command_BuyShard : IGlobalEvent
    {
        public Shard shard;
    }
}