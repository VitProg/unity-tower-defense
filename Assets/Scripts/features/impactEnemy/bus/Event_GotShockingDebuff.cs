using Leopotam.EcsProto.QoL;
using td.features.eventBus.types;

namespace td.features.impactEnemy.bus
{
    public struct Event_GotShockingDebuff : IGlobalEvent
    {
        public ProtoPackedEntityWithWorld entity;
        public float duration;
    }
}