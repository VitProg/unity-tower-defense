using Leopotam.EcsLite;
using td.features.level;
using td.features.state;
using td.features.wave.bus;

namespace td.features.wave
{
    public class Wave_Service
    {
        private readonly EcsInject<IState> state;
        private readonly EcsInject<LevelMap> levelMap;
        private readonly EcsInject<IEventBus> events;

        public void StartWave(int waveNumber)
        {
            state.Value.WaveNumber = waveNumber;

            var waveConfig = levelMap.Value.LevelConfig?.waves[waveNumber - 1];

            if (waveConfig == null) return;
            
            foreach (var spawn in waveConfig.Value.spawns)
            {
                ref var spawnSequence = ref events.Value.Global.Add<Wave_SpawnSequence>();
                spawnSequence.config = spawn;
                spawnSequence.enemyCounter = 0;
                spawnSequence.delayBeforeCountdown = spawn.delayBefore;
                spawnSequence.delayBetweenCountdown = 0;
                spawnSequence.lastSpawner = -1;

                state.Value.ActiveSpawnCount++;
            }
        }        
    }
}