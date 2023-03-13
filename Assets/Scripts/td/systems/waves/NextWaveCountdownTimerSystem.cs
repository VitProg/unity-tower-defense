using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.components.waves;
using td.utils.ecs;
using UnityEngine;

namespace td.systems.waves
{
    public class NextWaveCountdownTimerSystem : IEcsRunSystem
    {
        private readonly EcsWorldInject eventsWorld = Constants.Ecs.EventWorldName;
        private readonly EcsFilterInject<Inc<NextWaveCountdownTimer>> entities = Constants.Ecs.EventWorldName;

        public void Run(IEcsSystems systems)
        {
            var entity = EcsEventUtils.FirstEntity(entities);

            if (entity == null) return;
            
            ref var countdown = ref entities.Pools.Inc1.Get((int)entity);

            var last = countdown.countdown;
            var current = last - Time.deltaTime;
                
            countdown.countdown = current;

            if ((int)last != (int)current)
            {
                Debug.Log($"COUNTDOWN - {(int)current}");
            }

            if (countdown.countdown < 0.000001f)
            {
                EcsEventUtils.CleanupEvent(eventsWorld.Value, entities);
                EcsEventUtils.SendSingle<IncreaseWaveCommand>(eventsWorld.Value);
            }
        }
    }
}