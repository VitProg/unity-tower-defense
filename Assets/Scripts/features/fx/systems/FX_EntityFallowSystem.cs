using System;
using Leopotam.EcsProto.QoL;
using td.features.destroy;
using td.features.movement;
using td.features.state;
using td.utils.ecs;

namespace td.features.fx.systems
{
    public class FX_EntityFallowSystem : ProtoIntervalableRunSystem
    {
        [DI(Constants.Worlds.FX)] private FX_Aspect aspect;
        [DI] private State state;
        [DI] private FX_Service fxService;
        [DI] private Movement_Service movementService;
        [DI] private Destroy_Service destroyService;

        public override void IntervalRun(float deltaTime)
        {
            if (!state.GetGameSimulationEnabled()) return;
            
            foreach (var fxEntity in aspect.itEentityFallow)
            {
                ref var target = ref aspect.withTargetEntityPool.Get(fxEntity);
                ref var transform = ref aspect.withTransformPool.Get(fxEntity);
                
                if (
                    !target.entity.Unpack(out var targetEntityWorld, out var targetEntity) ||
                    destroyService.IsDestroyed(targetEntity)
                )
                {
                    aspect.needRemovePool.GetOrAdd(fxEntity);
                    continue;
                }
                
                ref var targetTransform = ref movementService.GetTransform(targetEntity);

                transform.SetPosition(targetTransform.position);
            }
        }

        public FX_EntityFallowSystem(float interval, float timeShift, Func<float> getDeltaTime) : base(interval, timeShift, getDeltaTime)
        {
            
        }
    }
}