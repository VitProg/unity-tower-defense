using Leopotam.EcsProto.QoL;
using td.features.eventBus.types;

namespace td.features.tower.towerRadius.bus {
    public struct Command_ShowTowerRadius : IGlobalEvent {
        public ProtoPackedEntityWithWorld Tower;
    }
}
