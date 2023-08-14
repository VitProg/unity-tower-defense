using System;
using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using td.features.movement.systems;
using td.utils.ecs;

namespace td.features.enemy.systems
{
    public class Enemy_Movement_System : ProtoIntervalableRunSystem, IProtoPreInitSystem
    {
        [DI] private Enemy_Aspect aspect;
        private MovementSubSystem movement;

        public void PreInit(IProtoSystems systems)
        {
            movement = new MovementSubSystem(aspect);
            TotalAutoInjectModule.Inject(movement, systems, systems.Services());
        }

        public override void IntervalRun(float deltaTime)
        {
            movement.Run(deltaTime);
        }
        
        public Enemy_Movement_System(float interval, float timeShift, Func<float> getDeltaTime) : base(interval, timeShift, getDeltaTime)
        {
        }
    }
}