using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.components.flags;
using td.components.waves;
using td.services;
using td.utils.ecs;
using UnityEngine;

namespace td.systems.waves
{
    public class LevelFinishedHandler : IEcsRunSystem
    {
        private readonly EcsWorldInject world = default;
        private readonly EcsCustomInject<LevelData> levelData = default;
        private readonly EcsWorldInject eventsWorld = Constants.Ecs.EventWorldName;
        private readonly EcsFilterInject<Inc<IsEnemy>> enemyEntities = default;
        private readonly EcsFilterInject<Inc<SpawnSequence>> spawnSequenceEntities = Constants.Ecs.EventWorldName;

        private readonly EcsFilterInject<Inc<LevelFinishedEvent>> entities = Constants.Ecs.EventWorldName;
        
        public void Run(IEcsSystems systems)
        {
            var entity = EcsEventUtils.FirstEntity(entities);

            if (entity == null) return;

            var spawnSequenceCount = spawnSequenceEntities.Value.GetEntitiesCount();
            var enemiesCount = enemyEntities.Value.GetEntitiesCount();
            
            if (levelData.Value.IsLastWave &&
                spawnSequenceCount <= 0 &&
                enemiesCount <= 0)
            {
                EcsEventUtils.CleanupEvent(eventsWorld.Value, entities);
                
                Debug.Log("LEVEL COMPLETE!!!");
            }
        }
    }
}