using Leopotam.EcsLite;

namespace td.features.enemy.bus
{
    public struct Event_Enemy_ChangeHealth : IEventEntity
    {
        public EcsPackedEntityWithWorld Entity { get; set; }
    }
}