using Leopotam.EcsProto.QoL;
using td.features.eventBus.types;

namespace td.features.tower.towerRadius.bus
{
    public struct Command_Tower_ShowRadius : IGlobalEvent
    {
        public ProtoPackedEntityWithWorld towerEntity;
    }
}