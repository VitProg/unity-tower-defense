using System;
using Leopotam.EcsProto.QoL;
using td.features.eventBus.types;

namespace td.features.destroy.bus
{
    [Serializable]
    public struct Command_Remove : IGlobalEvent
    {
        public ProtoPackedEntityWithWorld Entity { get; set; }
    }
}