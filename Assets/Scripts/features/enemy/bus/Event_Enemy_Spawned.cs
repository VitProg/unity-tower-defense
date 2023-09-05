using Leopotam.EcsProto.QoL;
using td.features.eventBus.types;

namespace td.features.enemy.bus
{
    public struct Event_Enemy_Spawned: IGlobalEvent
    {
        public ProtoPackedEntityWithWorld Entity { get; set; }
    }
}