using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.features.state;
using td.utils.ecs;
using UnityEngine;

namespace td.features.waves
{
    public class NextWaveCountdownTimerSystem : IEcsRunSystem
    {
        [InjectWorld(Constants.Worlds.Outer)] private EcsWorld outerWorld;
        [Inject] private State state;
        
        private readonly EcsFilterInject<Inc<NextWaveCountdownOuter>> eventEntities = Constants.Worlds.Outer;

        public void Run(IEcsSystems systems)
        {
            foreach (var eventEntity in eventEntities.Value)
            {
                RunInternal(systems, eventEntity);
            }
        }
        
        private void RunInternal(IEcsSystems systems, int eventEntity)
        {
            ref var countdown = ref eventEntities.Pools.Inc1.Get((int)eventEntity);

            var last = countdown.countdown;
            var current = last - Time.deltaTime;
                
            countdown.countdown = current;

            var iLast = (int)(last * 30);
            var iCurrent = (int)(current * 30);
            
            // if (Mathf.Abs(current - last) > 0.01f)
            if (iLast != iCurrent)
            {
                // Debug.Log($"COUNTDOWN - {iCurrent}");
                state.NextWaveCountdown = current;
            }

            if (countdown.countdown < Constants.ZeroFloat)
            {
                systems.CleanupOuter(eventEntities);
                systems.OuterSingle<IncreaseWaveOuterCommand>();
                state.NextWaveCountdown = 0;
            }
        }
    }
}