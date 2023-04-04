using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.services;
using td.utils.ecs;
using UnityEngine;

namespace td.features.waves
{
    public class NextWaveCountdownTimerSystem : IEcsRunSystem
    {
        private readonly EcsWorldInject eventsWorld = Constants.Ecs.EventsWorldName;
        private readonly EcsFilterInject<Inc<NextWaveCountdownTimer>> entities = Constants.Ecs.EventsWorldName;
        private readonly EcsCustomInject<UI> ui = default;

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
                ui.Value.UpdateWaveCountdown((int)current + 1);
            }

            if (countdown.countdown < Constants.ZeroFloat)
            {
                EcsEventUtils.CleanupEvent(eventsWorld.Value, entities);
                EcsEventUtils.SendSingle<IncreaseWaveCommand>(eventsWorld.Value);
                ui.Value.UpdateWaveCountdown(0);
            }
        }
    }
}