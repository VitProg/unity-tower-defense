using System;
using Leopotam.EcsProto;
using td.features.eventBus;
using td.features.eventBus.types;
using td.features.impactKernel.bus;
using td.utils.ecs;
using UnityEngine;

namespace td.features.impactKernel
{
    public class ImpactKernel_Module : IProtoModuleWithEvents
    {
        public void Init(IProtoSystems systems)
        {
            // Debug.Log($"{GetType().Name} Init");
            systems
                .AddSystem(new KernalChangeLivesSystem())
                //
                .AddService(new ImpactKernel_Service(), true)
                ;
        }

        public IProtoAspect[] Aspects()
        {
            return null;
        }

        public IProtoModule[] Modules()
        {
            return null;
        }

        public Type[] Events() => Ev.E<
            Command_Kernel_Heal,
            Command_Kernel_Damage
        >();
    }
}