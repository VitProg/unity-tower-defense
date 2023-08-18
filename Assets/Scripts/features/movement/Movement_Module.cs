using System;
using Leopotam.EcsProto;
using td.features.movement.flags;
using td.features.movement.systems;
using td.utils.ecs;

namespace td.features.movement
{
    public class Movement_Module : IProtoModule
    {
        private readonly Func<float> getDeltaTime;
        
        public Movement_Module(Func<float> getDeltaTime)
        {
            // Debug.Log($"{GetType().Name} Init");
            this.getDeltaTime = getDeltaTime;
        }
        
        public void Init(IProtoSystems systems)
        {
            systems
                .AddService(new Movement_Service(), true)
                //
                .AddSystem(new DefaultMovementSystem(1/20f, 0f, getDeltaTime))
                .AddSystem(new ApplyObjectTransformSystem(1 / 60f, 0f, getDeltaTime))
                .AddSystem(new SmoothRotateSystem(1/30f, 0.3f, getDeltaTime))
                //
                .AddSystem(new PostRunDelSystem<IsTargetReached>(systems.World()))
                ;
        }

        public IProtoAspect[] Aspects()
        {
            return new IProtoAspect[]
            {
                new Movement_Aspect()
            };
        }

        public IProtoModule[] Modules()
        {
            return null;
        }
    }
}