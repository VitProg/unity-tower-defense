using Leopotam.EcsProto.QoL;
using td.features.eventBus.types;

namespace td.features.tower.towerMenu.bus
{
    public struct Command_HideTowerMenu : IGlobalEvent
    {
        public ProtoPackedEntityWithWorld towerEntity;
    }
}