using Leopotam.EcsLite;
using td.features.fx.types;

namespace td.features.fx.events
{
    public struct FX_Event_Screen_Spawned<T> : IEventEntity where T : struct, IScreenFX
    {
        public EcsPackedEntityWithWorld Entity { get; set; }
    }
}