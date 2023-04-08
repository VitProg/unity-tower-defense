using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.common;
using td.services;
using td.states;
using td.utils.ecs;
using UnityEngine;

namespace td.features.waves
{
    public class StartWaveExecutor : IEcsRunSystem
    {
        [EcsInject] private LevelState levelState;
        [EcsInject] private LevelMap levelMap;
        
        [EcsWorld] private EcsWorld world;        
        [EcsWorld(Constants.Worlds.Outer)] private EcsWorld outerWorld;

        private readonly EcsFilterInject<Inc<StartWaveOuterCommand>> eventEntities = Constants.Worlds.Outer;

        public void Run(IEcsSystems systems)
        {
            if (eventEntities.Value.GetEntitiesCount() == 0) return;
            systems.CleanupOuter(eventEntities);
            
            // Debug.Log("LogStartWaveExecutor RUN...");

            var waveNumber = levelState.WaveNumber;

            var waveConfig = levelMap.LevelConfig?.waves[waveNumber - 1];

            if (waveConfig != null)
            {
                foreach (var spawn in waveConfig.Value.spawns)
                {
                    world.AddComponent(
                        world.NewEntity(),
                        new SpawnSequence()
                        {
                            config = spawn,
                            enemyCounter = 0,
                            delayBeforeCountdown = spawn.delayBefore,
                            delayBetweenCountdown = 0,
                        }
                    );
                }
            }
            
            // Debug.Log("StartWaveExecutor FIN");
        }
    }
}