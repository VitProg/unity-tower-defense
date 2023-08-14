using Leopotam.EcsProto.QoL;
using td.features.eventBus.types;
using td.features.fx.types;

namespace td.features.fx.events
{
    public struct FX_Event_EnemyFallow_Spawned<T> : IGlobalEvent where T : struct, IEntityFallowFX
    {
        public ProtoPackedEntityWithWorld Entity { get; set; }
    }
}