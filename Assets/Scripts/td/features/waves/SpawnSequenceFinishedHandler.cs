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
        [EcsWorld(Constants.Worlds.Outer)] private EcsWorld outerWorld;
        
        private readonly EcsFilterInject<Inc<SpawnSequenceFinishedOuterEvent>> eventEntities = Constants.Worlds.Outer;


        public void Run(IEcsSystems systems)
        {
            if (eventEntities.Value.GetEntitiesCount() == 0) return;
            systems.CleanupOuter(eventEntities);            
            
            // Debug.Log("SpawnSequenceFinishedHandler RUN...");
        
            //Todo Check this!
            var numberOfActiveSpawn = outerWorld.GetEntitiesCount<SpawnEnemyOuterCommand>();

            if (numberOfActiveSpawn <= 0)
            {
                systems.SendOuter<AllEnemiesAreOverOuterWait>();
            }
        
            // Debug.Log("SpawnSequenceFinishedHandler FIN");
        }
    }
}