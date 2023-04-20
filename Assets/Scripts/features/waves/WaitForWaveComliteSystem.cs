using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.components.flags;
using td.features.enemies;
using td.features.levels;
using td.services;
using td.states;
using td.utils.ecs;
using UnityEngine;

namespace td.features.waves
{
    public class WaitForWaveComliteSystem : IEcsRunSystem
    {
        [EcsInject] private LevelState levelState;
        [EcsInject] private LevelMap levelMap;
        
        private readonly EcsFilterInject<Inc<Enemy>> enemyEntities = default;
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
                
                if (levelState.IsLastWave)
                {
                    systems.SendSingleOuter<LevelFinishedOuterEvent>();
                }
                else
                {
                    var countdown = levelState.WaveNumber <= 0
                        ? levelMap.LevelConfig?.delayBeforeFirstWave
                        : levelMap.LevelConfig?.delayBetweenWaves;

                    systems.SendSingleOuter(new NextWaveCountdownOuter()
                    {
                        countdown = countdown ?? 0,
                    });
                }
                // Debug.Log("WaitForAllEnemiesDeadSystem FIN");
            }
        }
    }
}