using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.features.enemies;
using td.features.waves;
using td.services;
using td.states;
using td.utils.ecs;
using UnityEngine;

namespace td.features.levels
{
    public class LevelFinishedHandler : IEcsRunSystem
    {
        [EcsInject] private LevelState levelState;
        [EcsWorld(Constants.Worlds.Outer)] private EcsWorld outerWorld;
        
        private readonly EcsFilterInject<Inc<IsEnemy>> enemyEntities = default;
        private readonly EcsFilterInject<Inc<SpawnSequence>> spawnSequenceEntities = default;

        private readonly EcsFilterInject<Inc<LevelFinishedOuterEvent>> eventEntities = Constants.Worlds.Outer;
        
        public void Run(IEcsSystems systems)
        {
            if (eventEntities.Value.GetEntitiesCount() > 0)
            {
                var spawnSequenceCount = spawnSequenceEntities.Value.GetEntitiesCount();
                var enemiesCount = enemyEntities.Value.GetEntitiesCount();

                if (levelState.IsLastWave &&
                    spawnSequenceCount <= 0 &&
                    enemiesCount <= 0)
                {
                    Debug.Log("LEVEL COMPLETE!!!");
                }
            }
            systems.CleanupOuter(eventEntities);
        }
    }
}