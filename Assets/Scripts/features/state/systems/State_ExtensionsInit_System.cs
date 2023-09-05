using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using td.utils.ecs;

namespace td.features.state.systems
{
    public class State_ExtensionsInit_System : IProtoPreInitSystem
    {
        [DI] private State_Aspect aspect;
        
        public void PreInit(IProtoSystems systems)
        {
            for (var idx = 0; idx < aspect.extensions.Len(); idx++)
            {
                TotalAutoInjectModule.Inject(aspect.extensions.Get(idx), systems, systems.Services());
            }
        }
    }
}