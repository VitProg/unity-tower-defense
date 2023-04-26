using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.components.commands;
using td.components.flags;
using td.features.enemies;
using td.features.enemies.components;
using td.features.waves;
using td.services;
using td.utils.ecs;
using UnityEngine;

namespace td.features.levels
{
    public class LevelFinishedHandler : IEcsRunSystem
    {
        [Inject] private LevelState levelState;
        [InjectWorld(Constants.Worlds.Outer)] private EcsWorld outerWorld;
        
        private readonly EcsFilterInject<Inc<Enemy>, Exc<IsDestroyed>> enemyEntities = default;
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
                    systems.Outer<LoadLevelOuterCommand>().levelNumber = levelState.LevelNumber + 1;
                }
            }
            systems.CleanupOuter(eventEntities);
        }
    }
}