using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.common;
using td.features.enemies;
using td.services;
using td.utils.ecs;
using UnityEngine;

namespace td.features.waves
{
    public class SpawnSequenceFinishedHandler : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<SpawnSequenceFinishedEvent>> entities = Constants.Ecs.EventsWorldName;
        
        private readonly EcsCustomInject<LevelData> levelData = default;
        private readonly EcsSharedInject<SharedData> shared = default;
        private readonly EcsWorldInject world = default;
        private readonly EcsWorldInject eventsWorld = Constants.Ecs.EventsWorldName;


        public void Run(IEcsSystems systems)
        {
            if (EcsEventUtils.FirstEntity(entities) == null) return;
            
            Debug.Log("SpawnSequenceFinishedHandler RUN...");
        
            var numberOfActiveSpawn = eventsWorld.Value.Filter<SpawnEnemyCommand>().End().GetEntitiesCount();

            if (numberOfActiveSpawn <= 0)
            {
                EcsEventUtils.Send<WaitForAllEnemiesDead>(eventsWorld.Value);
            }
        
            EcsEventUtils.CleanupEvent(eventsWorld.Value, entities);
            
            Debug.Log("SpawnSequenceFinishedHandler FIN");
        }
    }
}