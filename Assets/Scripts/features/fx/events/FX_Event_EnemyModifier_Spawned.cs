using Leopotam.EcsLite;
using td.features.fx.types;

namespace td.features.fx.events
{
    public struct FX_Event_EnemyModifier_Spawned<T> : IEventEntity where T : struct, IEntityFallowFX
    {
        public EcsPackedEntityWithWorld Entity { get; set; }
    }
}