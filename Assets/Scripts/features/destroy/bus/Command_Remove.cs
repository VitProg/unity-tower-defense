using System;
using Leopotam.EcsProto.QoL;

namespace td.features.destroy.bus
{
    [Serializable]
    public struct Command_Remove
    {
        public ProtoPackedEntityWithWorld Entity { get; set; }
    }
}