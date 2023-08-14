using Leopotam.EcsProto.QoL;
using td.features.eventBus;
using td.features.level;
using td.features.state;
using td.features.wave.bus;

namespace td.features.wave
{
    public class Wave_Service
    {
        [DI] private State state;
        [DI] private LevelMap levelMap;
        [DI] private EventBus events;

        public void StartWave(int waveNumber)
        {
            state.SetWaveNumber(waveNumber);

            var waveConfig = levelMap.LevelConfig?.waves[waveNumber - 1];

            if (waveConfig == null) return;
            
            foreach (var spawn in waveConfig.Value.spawns)
            {
                ref var spawnSequence = ref events.global.Add<Wave_SpawnSequence>();
                spawnSequence.config = spawn;
                spawnSequence.enemyCounter = 0;
                spawnSequence.delayBeforeCountdown = spawn.delayBefore;
                spawnSequence.delayBetweenCountdown = 0;
                spawnSequence.lastSpawner = -1;

                state.SetActiveSpawnCount(state.GetActiveSpawnCount() + 1);
            }
        }        
    }
}