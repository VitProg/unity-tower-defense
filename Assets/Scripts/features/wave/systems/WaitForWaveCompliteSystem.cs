using System;
using Leopotam.EcsLite;
using td.features.level;
using td.features.level.bus;
using td.features.state;
using td.features.wave.bus;
using td.utils.ecs;

namespace td.features.wave.systems
{
    public class WaitForWaveCompliteSystem  : EcsIntervalableRunSystem
    {
        private readonly EcsInject<IState> state;
        private readonly EcsInject<LevelMap> levelMap;
        private readonly EcsInject<IEventBus> events;

        public override void IntervalRun(IEcsSystems _, float deltaTime)
        {
            if (!events.Value.Unique.Has<Wait_AllEnemiesAreOver>()) return;
            
            var spawnSequenceCount = state.Value.ActiveSpawnCount;
            var enemiesCount = state.Value.EnemiesCount;

            if (spawnSequenceCount > 0 || enemiesCount > 0) return;
            
            if (state.Value.WaveNumber + 1 >= state.Value.WaveCount)
            {
                events.Value.Unique.Add<Event_LevelFinished>();
            }
            else
            {
                var countdown = state.Value.WaveNumber <= 0
                    ? levelMap.Value.LevelConfig?.delayBeforeFirstWave
                    : levelMap.Value.LevelConfig?.delayBetweenWaves;

                events.Value.Unique.Add<Wave_NextCountdown>().countdown = countdown ?? 0;
            }
                
            events.Value.Unique.Del<Wait_AllEnemiesAreOver>();;
        }

        public WaitForWaveCompliteSystem(float interval, float timeShift, Func<float> getDeltaTime) : base(interval, timeShift, getDeltaTime)
        {
        }
    }
}