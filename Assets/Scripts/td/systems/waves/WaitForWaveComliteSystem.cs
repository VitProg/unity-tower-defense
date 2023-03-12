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
            var eventEntity = EcsEventUtils.Single(eventEntities);

            if (eventEntity == null) return;

            var spawnSequenceCount = spawnSequenceEntities.Value.GetEntitiesCount();
            var enemiesCount = enemyEntities.Value.GetEntitiesCount();

            Debug.Log("WaitForAllEnemiesDeadSystem RUN...");

            if (spawnSequenceCount <= 0 &&
                enemiesCount <= 0)
            {
                EcsEventUtils.CleanupEvent(systems, eventEntities);
                EcsEventUtils.Send<IncreaseWaveCommand>(systems);
            }

            Debug.Log("WaitForAllEnemiesDeadSystem FIN");
        }
    }
}