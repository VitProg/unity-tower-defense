using Leopotam.EcsProto.QoL;
using td.features.eventBus.types;

namespace td.features.tower.towerMenu.bus
{
    public struct Command_ShowTowerMenu : IGlobalEvent
    {
        public ProtoPackedEntityWithWorld towerEntity;
    }
}