using System;
using Leopotam.EcsLite;

namespace td.features._common.bus
{
    [Serializable]
    public struct Command_Remove : IEventEntity
    {
        public EcsPackedEntityWithWorld Entity { get; set; }
    }
}