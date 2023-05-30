using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.components.flags;
using td.features.enemies.components;
using td.features.levels;
using td.features.state;
using td.services;
using td.utils.ecs;

namespace td.features.waves
{
    public class WaitForWaveComliteSystem : IEcsRunSystem
    {
        [Inject] private State state;
        [Inject] private LevelMap levelMap;
        
        private readonly EcsFilterInject<Inc<Enemy>, Exc<IsDestroyed>> enemyEntities = default;
        private readonly EcsFilterInject<Inc<AllEnemiesAreOverOuterWait>> outerEntities = Constants.Worlds.Outer;
        private readonly EcsFilterInject<Inc<SpawnSequence>> spawnSequenceEntities = default;

        public void Run(IEcsSystems systems)
        {
            if (outerEntities.Value.GetEntitiesCount() == 0) return;
            
            var spawnSequenceCount = spawnSequenceEntities.Value.GetEntitiesCount();
            var enemiesCount = enemyEntities.Value.GetEntitiesCount();

            if (spawnSequenceCount <= 0 && enemiesCount <= 0)
            {
                // Debug.Log("WaitForAllEnemiesDeadSystem RUN...");
                
                systems.CleanupOuter(outerEntities);
                
                if (state.WaveNumber + 1 >= state.WaveCount)
                {
                    systems.OuterSingle<LevelFinishedOuterEvent>();
                }
                else
                {
                    var countdown = state.WaveNumber <= 0
                        ? levelMap.LevelConfig?.delayBeforeFirstWave
                        : levelMap.LevelConfig?.delayBetweenWaves;

                    systems.OuterSingle<NextWaveCountdownOuter>().countdown = countdown ?? 0;
                }
                // Debug.Log("WaitForAllEnemiesDeadSystem FIN");
            }
        }
    }
}