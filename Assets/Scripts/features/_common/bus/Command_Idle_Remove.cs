using System;
using Leopotam.EcsLite;

namespace td.features._common.bus
{
    [Serializable]
    //todo IEventEntity + IEventPersist not working
    public struct Command_Idle_Remove : IEventEntity, IEventPersist
    {
        public EcsPackedEntityWithWorld Entity { get; set; }
        public float remainingTime;
    }
}