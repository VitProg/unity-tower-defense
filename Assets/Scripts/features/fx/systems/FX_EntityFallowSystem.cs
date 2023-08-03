using System;
using Leopotam.EcsLite;
using td.features._common;
using td.utils.ecs;

namespace td.features.fx.systems
{
    public class FX_EntityFallowSystem : EcsIntervalableRunSystem
    {
        private readonly EcsInject<FX_Service> fxService;
        private readonly EcsInject<FX_Pools> pools;
        private readonly EcsInject<Common_Service> common;

        public override void IntervalRun(IEcsSystems systems, float dt)
        {
            foreach (var fxEntity in pools.Value.entityFallowFilter.Value)
            {
                ref var target = ref pools.Value.entityFallowFilter.Pools.Inc2.Get(fxEntity);
                ref var transform = ref pools.Value.entityFallowFilter.Pools.Inc4.Get(fxEntity);
                
                if (
                    !target.entity.Unpack(out var targetEntityWorldm, out var targetEntity) ||
                    common.Value.IsDestroyed(targetEntity)
                )
                {
                    pools.Value.needRemovePool.Value.SafeAdd(fxEntity);
                    continue;
                }
                
                ref var targetTransform = ref common.Value.GetTransform(targetEntity);

                transform.SetPosition(targetTransform.position);
            }
        }

        public FX_EntityFallowSystem(float interval, float timeShift, Func<float> getDeltaTime) : base(interval, timeShift, getDeltaTime)
        {
            
        }
    }
}