using Leopotam.EcsProto.QoL;
using td.features.eventBus.types;

namespace td.features.infoPanel.bus
{
    public struct Command_HideTowerInfo : IGlobalEvent
    {
        public ProtoPackedEntityWithWorld towerEntity;
    }
}