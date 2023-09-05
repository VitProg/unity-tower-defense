using System;
using Leopotam.EcsProto.QoL;
using td.features.eventBus.types;
using td.features.shard.components;
using Unity.Mathematics;

namespace td.features.shard.bus
{
    [Serializable]
    public struct Event_ShardDropped_OnMap : IGlobalEvent
    {
        public Shard shard;
        public float2 position;
    }
}