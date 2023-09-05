using System;
using Leopotam.EcsProto.QoL;
using td.features.eventBus.types;

namespace td.features.tower.bus
{
    [Serializable]
    public struct Event_Tower_UnHovered : IGlobalEvent
    {
        public ProtoPackedEntityWithWorld Tower;
    }
}