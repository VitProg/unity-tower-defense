using System;
using Leopotam.EcsProto;
using td.features.eventBus;
using td.features.impactEnemy.bus;
using td.features.impactEnemy.components;
using td.features.impactEnemy.systems;
using td.utils.ecs;

namespace td.features.impactEnemy
{
    public class ImpactEnemy_Module : IProtoModuleWithEvents
    {
        public void Init(IProtoSystems systems)
        {
            // Debug.Log($"{GetType().Name} Init");
            systems
                .AddSystem(new PoisonDebuffSystem())
                .AddSystem(new ShockingDebuffSystem())
                .AddSystem(new SpeedDebuffSystem())
                .AddSystem(new TakeDamageSystem())
                .AddSystem(new ImpactEnemy_ClearSystem())
                //
                .AddService(new ImpactEnemy_Service(), true);
        }

        public IProtoAspect[] Aspects()
        {
            return new IProtoAspect[]
            {
                new ImpactEnemy_Aspect(),
            };
        }

        public IProtoModule[] Modules()
        {
            return null;
        }

        public Type[] Events() => Ev.E<
            TakeDamage,
            Event_GotPoisonDebuff,
            Event_GotShockingDebuff,
            Event_GotSpeedDebuff
        >();
    }
}