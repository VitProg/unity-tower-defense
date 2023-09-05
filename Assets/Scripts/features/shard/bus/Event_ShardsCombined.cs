using System;
using Leopotam.EcsProto.QoL;
using td.features.eventBus.types;

namespace td.features.shard.bus
{
    [Serializable]
    public struct Event_ShardsCombined : IGlobalEvent
    {
        public ProtoPackedEntityWithWorld ShardEntity;
        public ProtoPackedEntityWithWorld BuildingEntity;
    }
}