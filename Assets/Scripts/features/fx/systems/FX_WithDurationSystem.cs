using System;
using Leopotam.EcsLite;
using td.features.state;
using td.utils.ecs;

namespace td.features.fx.systems
{
    public class FX_WithDurationSystem : EcsIntervalableRunSystem
    {
        private readonly EcsInject<FX_Pools> pools;
        private readonly EcsInject<FX_Service> fxService;
        private readonly EcsInject<State> state;

        public override void IntervalRun(IEcsSystems systems, float dt)
        {
            foreach (var fxEntity in pools.Value.withDurationFilter.Value)
            {
                ref var d = ref pools.Value.withDurationFilter.Pools.Inc1.Get(fxEntity);
                if (!d.withDuration || pools.Value.needRemovePool.Value.Has(fxEntity)) continue;
                d.remainingTime -= dt * state.Value.GameSpeed;
                if (d.remainingTime <= 0f)
                {
                    pools.Value.needRemovePool.Value.SafeAdd(fxEntity);
                }
            }
        }

        public FX_WithDurationSystem(float interval, float timeShift, Func<float> getDeltaTime) : base(interval, timeShift, getDeltaTime)
        {
        }
    }
}