using Leopotam.EcsProto.QoL;
using td.features.eventBus.types;
using Unity.Mathematics;

namespace td.features.shard.bus
{
    public struct Command_CombineShards_InBuilding : IGlobalEvent
    {
        public int sourceIndex;
        public ProtoPackedEntityWithWorld targetBuilding;
        public float2 position;
        public uint cost;
        public uint time;
    }
}