using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.components.flags;
using td.features.ui;
using td.features.waves;
using td.services;
using td.states;
using td.utils.ecs;
using UnityEngine;

namespace td.features.levels
{
    // todo rewrite to load level service
    public class LevelLoadedHandler : IEcsRunSystem
    {
        [EcsInject] private LevelMap levelMap;
        [EcsInject] private LevelState levelState;
        
        private readonly EcsFilterInject<Inc<LevelLoadedOuterEvent>> eventEntities = Constants.Worlds.Outer;

        public void Run(IEcsSystems systems)
        {
            if (eventEntities.Value.GetEntitiesCount() == 0) return;
            systems.CleanupOuter(eventEntities);

            // Debug.Log("LevelLoadedHandler RUN...");

            systems.CleanupOuter<IsLoadingOuter>();
            systems.SendSingleOuter(new UpdateUIOuterCommand
            {
                Lives = levelState.Lives,
                MaxLives = levelState.MaxLives,
                Money = levelState.Money,
                LevelNumber = levelState.LevelNumber,
                EnemiesCount = levelState.EnemiesCount,
                IsLastWave = levelState.IsLastWave,
                NextWaveCountdown = levelState.NextWaveCountdown,
                wave = new[] { levelState.WaveNumber, levelState.WaveCount },
            });


            var countdown = levelState.WaveNumber <= 0
                ? levelMap.LevelConfig?.delayBeforeFirstWave
                : levelMap.LevelConfig?.delayBetweenWaves;

            systems.SendSingleOuter(new NextWaveCountdownOuter()
            {
                countdown = countdown ?? 0,
            });


            // Debug.Log("LevelLoadedHandler FIN");
        }
    }
}