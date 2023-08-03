using Leopotam.EcsLite;
using td.features.fx.types;

namespace td.features.fx.events
{
    public struct FX_Event_Position_Spawned<T> : IEventEntity where T : struct, IPositionFX
    {
        public EcsPackedEntityWithWorld Entity { get; set; }
    }
}