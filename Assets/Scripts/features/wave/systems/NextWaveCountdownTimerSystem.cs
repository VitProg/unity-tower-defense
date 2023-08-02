using System;
using Leopotam.EcsLite;
using td.features.state;
using td.features.wave.bus;
using td.utils.ecs;

namespace td.features.wave.systems
{
    public class NextWaveCountdownTimerSystem : EcsIntervalableRunSystem
    {
        private readonly EcsInject<IState> state;
        private readonly EcsInject<IEventBus> events;
        private readonly EcsInject<Wave_Service> waveService;
        
        public override void IntervalRun(IEcsSystems systems, float deltaTime)
        {
            if (!events.Value.Unique.Has<Wave_NextCountdown>()) return;
            
            ref var countdown = ref events.Value.Unique.Get<Wave_NextCountdown>();
            
            var last = countdown.countdown;
            var current = last - deltaTime * state.Value.GameSpeed;

            countdown.countdown = current;

            state.Value.NextWaveCountdown = current;

            if (countdown.countdown < Constants.ZeroFloat)
            {
                events.Value.Unique.Del<Wave_NextCountdown>();
                events.Value.Unique.Add<Command_Wave_Increase>();
                state.Value.NextWaveCountdown = 0;
            }
        }

        public NextWaveCountdownTimerSystem(float interval, float timeShift, Func<float> getDeltaTime) : base(interval, timeShift, getDeltaTime)
        {
        }

    }
}