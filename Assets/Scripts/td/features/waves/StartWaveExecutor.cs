using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.common;
using td.services;
using td.utils.ecs;
using UnityEngine;

namespace td.features.waves
{
    public class StartWaveExecutor : IEcsRunSystem
    {
        private readonly EcsCustomInject<LevelData> levelData = default;
        private readonly EcsSharedInject<SharedData> shared = default;
        private readonly EcsWorldInject eventsWorld = Constants.Ecs.EventsWorldName;
        private readonly EcsWorldInject world = default;

        private readonly EcsFilterInject<Inc<StartWaveCommand>> entities = Constants.Ecs.EventsWorldName;

        public void Run(IEcsSystems systems)
        {
            if (EcsEventUtils.FirstEntity(entities) == null) return;
            
            Debug.Log("StartWaveExecutor RUN...");

            var waveNumber = levelData.Value.waveNumber;

            var waveConfig = levelData.Value.LevelConfig?.waves[waveNumber];

            if (waveConfig != null)
            {
                foreach (var spawn in waveConfig.Value.spawns)
                {
                    EntityUtils.AddComponent(
                        world.Value,
                        world.Value.NewEntity(),
                        new SpawnSequence()
                        {
                            Config = spawn,
                            EnemyCounter = 0,
                            DelayBeforeCountdown = spawn.delayBefore,
                            DelayBetweenCountdown = 0,
                        }
                    );
                }
            }


            EcsEventUtils.CleanupEvent(eventsWorld.Value, entities);
            
            Debug.Log("StartWaveExecutor FIN");
        }
    }
}