using System;
using Leopotam.EcsProto.QoL;
using td.features.eventBus.types;
using Unity.Mathematics;

namespace td.features.shard.bus
{
    [Serializable]
    public struct Event_ShardInserted_InBuilding : IGlobalEvent
    {
        public ProtoPackedEntityWithWorld BuildingEntity;
        public ProtoPackedEntityWithWorld ShardEntity;
    }
}