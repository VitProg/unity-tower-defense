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
    // todo rewrite to load level service

    public class LoadLevelExecutor : IEcsRunSystem
    {
        [EcsInject] private LevelMap levelMap;
        [EcsInject] private LevelState levelState;
        [EcsInject] private LevelLoader levelLoader;
        [EcsInject] private PathService pathService;

        private readonly EcsFilterInject<Inc<LoadLevelOuterCommand>> eventEntities = Constants.Worlds.Outer;

        public void Run(IEcsSystems systems)
        {
            foreach (var eventEntity in eventEntities.Value)
            {
                try
                {
                    RunInternal(systems, eventEntity);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }

                systems.CleanupOuter(eventEntities);
                break;
            }
        }

        private void RunInternal(IEcsSystems systems, int entity)
        {
            var levelNumber = eventEntities.Pools.Inc1.Get(entity).levelNumber;
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