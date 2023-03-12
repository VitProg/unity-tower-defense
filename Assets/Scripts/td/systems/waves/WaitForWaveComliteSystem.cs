using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.components.flags;
using td.components.waves;
using td.services;
using td.utils.ecs;
using UnityEngine;

namespace td.systems.waves
{
    public class WaitForWaveComliteSystem : IEcsRunSystem
    {
        private readonly EcsCustomInject<LevelData> levelData = default;
        private readonly EcsFilterInject<Inc<IsEnemy>> enemyEntities = default;
        private readonly EcsFilterInject<Inc<WaitForAllEnemiesDead>> eventEntities = Constants.Ecs.EventWorldName;
        private readonly EcsFilterInject<Inc<SpawnSequence>> spawnSequenceEntities = Constants.Ecs.EventWorldName;

        public void Run(IEcsSystems systems)
        {
            var eventEntity = EcsEventUtils.FirstEntity(eventEntities);

            if (eventEntity == null) return;

            var spawnSequenceCount = spawnSequenceEntities.Value.GetEntitiesCount();
            var enemiesCount = enemyEntities.Value.GetEntitiesCount();

            if (spawnSequenceCount <= 0 &&
                enemiesCount <= 0)
            {
                Debug.Log("WaitForAllEnemiesDeadSystem RUN...");
                
                EcsEventUtils.CleanupEvent(systems, eventEntities);
                
                if (levelData.Value.IsLastWave)
                {
                    EcsEventUtils.SendSingle<LevelFinishedEvent>(systems);
                }
                else
                {
                    var countdown = levelData.Value.waveNumber <= 0
                        ? levelData.Value.LevelConfig?.delayBeforeFirstWave
                        : levelData.Value.LevelConfig?.delayBetweenWaves;

                    EcsEventUtils.SendSingle(systems, new NextWaveCountdownTimer()
                    {
                        countdown = countdown ?? 0,
                    });
                }
                Debug.Log("WaitForAllEnemiesDeadSystem FIN");
            }
        }
    }
}