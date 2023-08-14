using System;
using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using td.features.movement.systems;
using td.utils.ecs;

namespace td.features.projectile.systems
{
    public class ProjectileMovementSystem : ProtoIntervalableRunSystem, IProtoPreInitSystem
    {
        [DI] private Projectile_Aspect aspect;
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

        public ProjectileMovementSystem(float interval, float timeShift, Func<float> getDeltaTime) : base(interval, timeShift, getDeltaTime)
        {
        }

    }
}