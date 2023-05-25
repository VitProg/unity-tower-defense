using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.services;
using td.utils;
using td.utils.ecs;
using UnityEngine;

namespace td.features.waves
{
    public class NextWaveCountdownTimerSystem : IEcsRunSystem
    {
        [InjectWorld(Constants.Worlds.Outer)] private EcsWorld outerWorld;
        [Inject] private LevelState levelSatate;
        
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

            // var iLast = (int)(last * 100);
            // var iCurrent = (int)(current * 100);
            
            if (Mathf.Abs(current - last) > 0.01f)
            {
                // Debug.Log($"COUNTDOWN - {iCurrent}");
                levelSatate.NextWaveCountdown = current;
            }

            if (countdown.countdown < Constants.ZeroFloat)
            {
                systems.CleanupOuter(eventEntities);
                systems.OuterSingle<IncreaseWaveOuterCommand>();
                levelSatate.NextWaveCountdown = 0;
            }
        }
    }
}