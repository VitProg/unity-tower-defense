using Leopotam.EcsProto.QoL;
using td.features.eventBus.types;
using Unity.Mathematics;

namespace td.features.shard.bus
{
    public struct Command_InsertShard_InBuilding : IGlobalEvent
    {
        public int sourceIndex;
        public ProtoPackedEntityWithWorld targetBuilding;
        public uint cost;
        public uint time;
    }
}