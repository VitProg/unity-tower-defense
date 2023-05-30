using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.features.state;
using td.services;
using td.utils.ecs;

namespace td.features.waves
{
    public class StartWaveExecutor : IEcsRunSystem
    {
        [Inject] private State state;
        [Inject] private LevelMap levelMap;

        [InjectWorld] private EcsWorld world;        
        [InjectWorld(Constants.Worlds.Outer)] private EcsWorld outerWorld;

        private readonly EcsFilterInject<Inc<StartWaveOuterCommand>> eventEntities = Constants.Worlds.Outer;

        public void Run(IEcsSystems systems)
        {
            if (eventEntities.Value.GetEntitiesCount() == 0) return;
            systems.CleanupOuter(eventEntities);
            
            // Debug.Log("LogStartWaveExecutor RUN...");

            var waveNumber = state.WaveNumber;

            var waveConfig = levelMap.LevelConfig?.waves[waveNumber - 1];

            if (waveConfig != null)
            {
                foreach (var spawn in waveConfig.Value.spawns)
                {
                    var entity = world.NewEntity();
                    // Debug.Log($">> NewEntity {entity} - StartWaveExecutor - Run Spawn Sequence");
                    ref var spawnSequence = ref world.GetComponent<SpawnSequence>(entity);
                    spawnSequence.config = spawn;
                    spawnSequence.enemyCounter = 0;
                    spawnSequence.delayBeforeCountdown = spawn.delayBefore;
                    spawnSequence.delayBetweenCountdown = 0;
                    spawnSequence.lastSpawner = -1;
                }
            }
            
            // Debug.Log("StartWaveExecutor FIN");
        }
    }
}