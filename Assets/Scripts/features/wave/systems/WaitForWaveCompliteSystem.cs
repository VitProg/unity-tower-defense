using System;
using Leopotam.EcsProto.QoL;
using td.features.eventBus;
using td.features.level;
using td.features.level.bus;
using td.features.state;
using td.features.wave.bus;
using td.utils.ecs;

namespace td.features.wave.systems
{
    public class WaitForWaveCompliteSystem : ProtoIntervalableRunSystem
    {
        [DI] private State state;
        [DI] private LevelMap levelMap;
        [DI] private EventBus events;

        public override void IntervalRun(float deltaTime)
        {
            if (!events.unique.Has<Wait_AllEnemiesAreOver>()) return;
            
            var spawnSequenceCount = state.GetActiveSpawnCount();
            var enemiesCount = state.GetEnemiesCount();

            if (spawnSequenceCount > 0 || enemiesCount > 0) return;
            
            if (state.GetWaveNumber() + 1 >= state.GetWaveCount())
            {
                events.unique.GetOrAdd<Event_LevelFinished>();
            }
            else
            {
                var countdown = state.GetWaveNumber() <= 0
                    ? levelMap.LevelConfig?.delayBeforeFirstWave
                    : levelMap.LevelConfig?.delayBetweenWaves;

                events.unique.GetOrAdd<Wave_NextCountdown>().countdown = countdown ?? 0;
            }
                
            events.unique.Del<Wait_AllEnemiesAreOver>();
        }

        public WaitForWaveCompliteSystem(float interval, float timeShift, Func<float> getDeltaTime) : base(interval, timeShift, getDeltaTime)
        {
        }
    }
}