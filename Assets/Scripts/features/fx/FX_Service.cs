using Leopotam.EcsLite.Di;
using td.features.fx.subServices;
using td.utils.di;

namespace td.features.fx
{
    public class FX_Service
    {
        [field: AutoResolve] public FX_EntityModifier_SubService EntityModifier { get; } = new();

        [field: AutoResolve]
        public FX_EntityFallow_SubService EntityFallow { get; } = new();
        
        [field: AutoResolve]
        public FX_Position_SubService Position { get; } = new();
        
        [field: AutoResolve]
        public FX_Screen_SubService Screen { get; } = new();

        public readonly EcsWorldInject fxWorld = Constants.Worlds.FX;
    }
}