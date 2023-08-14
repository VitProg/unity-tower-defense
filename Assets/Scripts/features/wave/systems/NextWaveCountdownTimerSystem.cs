using System;
using Leopotam.EcsProto.QoL;
using td.features.eventBus;
using td.features.state;
using td.features.wave.bus;
using td.utils.ecs;

namespace td.features.wave.systems
{
    public class NextWaveCountdownTimerSystem : ProtoIntervalableRunSystem
    {
        [DI] private readonly State state;
        [DI] private readonly EventBus events;
        [DI] private readonly Wave_Service waveService;
        
        public override void IntervalRun(float deltaTime)
        {
            if (!events.unique.Has<Wave_NextCountdown>()) return;
            
            ref var countdown = ref events.unique.GetOrAdd<Wave_NextCountdown>();
            
            var last = countdown.countdown;
            var current = last - deltaTime * state.GetGameSpeed();

            countdown.countdown = current;

            state.SetNextWaveCountdown(current);

            if (countdown.countdown < Constants.ZeroFloat)
            {
                events.unique.Del<Wave_NextCountdown>();
                events.unique.GetOrAdd<Command_Wave_Increase>();
                state.SetNextWaveCountdown(0);
            }
        }

        public NextWaveCountdownTimerSystem(float interval, float timeShift, Func<float> getDeltaTime) : base(interval, timeShift, getDeltaTime)
        {
        }
    }
}