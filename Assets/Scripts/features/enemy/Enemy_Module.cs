using System;
using Leopotam.EcsProto;
using td.features.enemy.bus;
using td.features.enemy.systems;
using td.features.eventBus;
using td.utils.ecs;
using UnityEngine;

namespace td.features.enemy
{
    public class Enemy_Module : IProtoModuleWithEvents
    {
        private readonly Func<float> getDeltaTime;
        
        public Enemy_Module(Func<float> getDeltaTime)
        {
            // Debug.Log($"{GetType().Name} Init");
            this.getDeltaTime = getDeltaTime;
        }

        public void Init(IProtoSystems systems)
        {
            systems
                .AddSystem(new Enemy_AnimationSpeed_MB_System())
                .AddSystem(new Enemy_CalcDistanceToKernel_System(1/15f, 0f, getDeltaTime))
                .AddSystem(new Enemy_Died_System())
                .AddSystem(new Enemy_HP_System())
                .AddSystem(new Enemy_InitData_System())
                .AddSystem(new Enemy_Movement_System(1/30f, 0f, getDeltaTime))
                .AddSystem(new Enemy_ReachCell_System())
                .AddSystem(new Enemy_ReachKernel_System())
                //
                .AddService(new Enemy_Service(), true)
                .AddService(new Enemy_Converter(), true)
                .AddService(new Enemy_Path_Service(), true)
                ;
        }

        public IProtoAspect[] Aspects()
        {
            return new IProtoAspect[]
            {
                new Enemy_Aspect(),
            };
        }

        public IProtoModule[] Modules()
        {
            return null;
        }

        public Type[] Events() => Ev.E<
            Event_Enemy_ChangeHealth,
            Event_Enemy_ReachKernel
        >();
    }
}