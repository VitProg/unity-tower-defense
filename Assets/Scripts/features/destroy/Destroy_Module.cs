using System;
using Leopotam.EcsProto;
using td.features.destroy.bus;
using td.features.destroy.systems;
using td.features.eventBus;
using td.utils.ecs;

namespace td.features.destroy
{
    public class Destroy_Module : IProtoModuleWithEvents
    {
        public void Init(IProtoSystems systems)
        {
            // Debug.Log($"{GetType().Name} Init");
            
            systems
                .AddService(new Destroy_Service(), true)
                .AddSystem(new RemoveSystem())
                ;
        }

        public IProtoAspect[] Aspects()
        {
            return new IProtoAspect[]
            {
                new Destroy_Aspect(),
            };
        }

        public IProtoModule[] Modules()
        {
            return null;
        }

        public Type[] Events() => Ev.E<
            Command_Remove,
            Command_Idle_Remove
        >();
    }
}