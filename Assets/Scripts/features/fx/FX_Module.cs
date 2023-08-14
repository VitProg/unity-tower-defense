using System;
using Leopotam.EcsProto;
using td.features.eventBus;
using td.features.fx.effects;
using td.features.fx.effects.@base;
using td.features.fx.events;
using td.features.fx.listeners;
using td.features.fx.subServices;
using td.features.fx.systems;
using td.utils.ecs;
using UnityEngine;

namespace td.features.fx
{
    public class FX_Module : IProtoModuleWithEvents
    {
        private readonly Func<float> getDeltaTime;
        
        public FX_Module(Func<float> getDeltaTime)
        {
            this.getDeltaTime = getDeltaTime;
        }
        
        public void Init(IProtoSystems systems)
        {
            // Debug.Log($"{GetType().Name} Init");
            systems
                .AddSystem(new FX_EntityFallowSystem(1 / 30f, 0f, getDeltaTime), Constants.EcsPoints.FX)
                .AddSystem(new FX_ApplyTransformSystem(1 / 10f, 0f, getDeltaTime), Constants.EcsPoints.FX)
                .AddSystem(new FX_WithDurationSystem(1 / 45f, 0f, getDeltaTime), Constants.EcsPoints.FX)
                //effects
                .AddSystem(new BlinkFX_System(1 / 15f, 0f, getDeltaTime), Constants.EcsPoints.FX)
                .AddSystem(new HitFX_System(), Constants.EcsPoints.FX)
                .AddSystem(new WithSpriteAnimatorFX_System<ColdStatusFX>(), Constants.EcsPoints.FX)
                .AddSystem(new WithSpriteAnimatorFX_System<PoisonStatusFX>(), Constants.EcsPoints.FX)
                .AddSystem(new WithSpriteAnimatorFX_System<ElectroStatusFX>(), Constants.EcsPoints.FX)
                .AddSystem(new WithSpriteAnimatorFX_System<FireStatusFX>(), Constants.EcsPoints.FX)
                //listeners
                .AddSystem(new FX_TakeDamage_ListenerSystem(), Constants.EcsPoints.FX)
                .AddSystem(new FX_GotDebuff_ListenerSystem(), Constants.EcsPoints.FX)
                //clean
                .AddSystem(new FX_AutoRemoveSystem(), Constants.EcsPoints.FX)
                //services
                .AddService(new FX_Service(), true)
                .AddService(new FX_EntityModifier_SubService())
                .AddService(new FX_EntityFallow_SubService())
                .AddService(new FX_Screen_SubService())
                .AddService(new FX_Position_SubService())
                ;
        }
        
        public IProtoAspect[] Aspects()
        {
            return new IProtoAspect[]
            {
                new FX_Aspect()
            };
        }

        public IProtoModule[] Modules()
        {
            return null;
        }

        public Type[] Events() =>
            Ev.E<
                FX_Event_EnemyModifier_Spawned<BlinkFX>,
                FX_Event_EnemyFallow_Spawned<ColdStatusFX>,
                FX_Event_EnemyFallow_Spawned<ElectroStatusFX>,
                FX_Event_EnemyFallow_Spawned<FireStatusFX>,
                FX_Event_EnemyFallow_Spawned<PoisonStatusFX>,
                FX_Event_EnemyFallow_Spawned<HitFX>
            >();
    }
}