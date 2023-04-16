using System;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.components.commands;
using td.components.flags;
using td.features.ui;
using td.features.waves;
using td.services;
using td.states;
using td.utils.ecs;
using UnityEngine;

namespace td.features.levels
{
    public class LoadLevelExecutor : IEcsRunSystem
    {
        [EcsWorld] private EcsWorld world;
        
        [EcsInject] private LevelMap levelMap;
        [EcsInject] private LevelState levelState;
        [EcsInject] private LevelLoader levelLoader;
        [EcsInject] private PathService pathService;

        private readonly EcsFilterInject<Inc<LoadLevelOuterCommand>> loadCommandEntities = Constants.Worlds.Outer;
        private readonly EcsFilterInject<Inc<LevelLoadedOuterEvent>> loadedEventEntities = Constants.Worlds.Outer;

        public void Run(IEcsSystems systems)
        {
            try
            {
                foreach (var entity in loadCommandEntities.Value)
                {
                    Load(systems, entity);
                    systems.CleanupOuter<LoadLevelOuterCommand>();
                    break;
                }

                foreach (var entity in loadedEventEntities.Value)
                {
                    levelLoader.InitBuildings(world);
                    systems.CleanupOuter<LevelLoadedOuterEvent>();
                    break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private void Load(IEcsSystems systems, int entity)
        {
            var levelNumber = loadCommandEntities.Pools.Inc1.Get(entity).levelNumber;
            levelState.LevelNumber = levelNumber;

            if (levelLoader.HasLevel())
            {
                levelLoader.LoadLevel(systems);
                pathService.InitPath();

                systems.CleanupOuter<IsLoadingOuter>();
                systems.SendSingleOuter(UpdateUIOuterCommand.FromLevelState(levelState));

                var countdown = levelState.WaveNumber <= 0
                    ? levelMap.LevelConfig?.delayBeforeFirstWave
                    : levelMap.LevelConfig?.delayBetweenWaves;

                systems.SendSingleOuter(new NextWaveCountdownOuter()
                {
                    countdown = countdown ?? 0,
                });

                systems.SendOuter<LevelLoadedOuterEvent>();
            }
            else
            {
                Debug.Log("ALL LEVELS ARE FINISHED!");
            }
        }
    }
}