using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.components.commands;
using td.components.flags;
using td.features.enemies.components;
using td.features.state;
using td.features.waves;
using td.utils.ecs;
using UnityEngine;

namespace td.features.levels
{
    public class LevelFinishedHandler : IEcsRunSystem
    {
        [Inject] private State state;
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

                if (state.WaveNumber + 1 >= state.WaveCount &&
                    spawnSequenceCount <= 0 &&
                    enemiesCount <= 0)
                {
                    Debug.Log("LEVEL COMPLETE!!!");
                    //todo show vickoty screen
                    systems.Outer<LoadLevelOuterCommand>().levelNumber = state.LevelNumber + 1;
                }
            }
            systems.CleanupOuter(eventEntities);
        }
    }
}