using System;
using Leopotam.EcsProto.QoL;
using td.features.state;
using td.utils;
using td.utils.ecs;

namespace td.features.fx.systems
{
    public class FX_WithDurationSystem : ProtoIntervalableRunSystem
    {
        [DI(Constants.Worlds.FX)] private FX_Aspect aspect;
        [DI] private FX_Service fxService;
        [DI] private State state;

        public override void IntervalRun(float deltaTime)
        {
            if (!state.GetSimulationEnabled()) return;
            
            foreach (var fxEntity in aspect.itWithDuration)
            {
                ref var d = ref aspect.withDurationPool.Get(fxEntity);
                if (!d.withDuration || aspect.needRemovePool.Has(fxEntity)) continue;
                d.remainingTime -= deltaTime * state.GetGameSpeed();
                if (d.remainingTime <= 0f)
                {
                    aspect.needRemovePool.GetOrAdd(fxEntity);
                }
            }
        }

        public FX_WithDurationSystem(float interval, float timeShift, Func<float> getDeltaTime) : base(interval, timeShift, getDeltaTime)
        {
        }
    }
}