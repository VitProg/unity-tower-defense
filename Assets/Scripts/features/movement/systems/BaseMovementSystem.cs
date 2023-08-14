using System;
using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using td.utils.ecs;

namespace td.features.movement.systems
{
    public class BaseMovementSystem : ProtoIntervalableRunSystem, IProtoPreInitSystem
    {
        [DI] private Movement_Aspect aspect;
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

        public BaseMovementSystem(float interval, float timeShift, Func<float> getDeltaTime) : base(interval, timeShift, getDeltaTime)
        {
            
        }
    }
}