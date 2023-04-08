using System;
using System.Diagnostics;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.common.decorators;
using td.common.level;
using td.components.commands;
using td.services;
using td.utils;
using td.utils.ecs;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace td.features.levels
{
    // todo rewrite to load level service
    public class LoadLevelExecutor: IEcsRunSystem
    {
        [EcsInject] private LevelMap levelMap;
        
        private readonly EcsFilterInject<Inc<LoadLevelOuterCommand>> eventEntities = Constants.Worlds.Outer;

        public void Run(IEcsSystems systems)
        {
            foreach (var eventEntity in eventEntities.Value)
            {
                RunInternal(systems, eventEntity);
                
                systems.CleanupOuter(eventEntities);
                break;
            }
        }

        private void RunInternal(IEcsSystems systems, int entity)
        {
            // Debug.Log("LoadLevelExecutor RUN...");

            var levelNumber = eventEntities.Pools.Inc1.Get(entity).levelNumber;
            try
            {
                var levelConfig = ResourcesUtils.LoadJson<LevelConfig>($"Levels/{levelNumber}");
                levelMap.LevelConfig = levelConfig;
                // Debug.Log(levelConfig);

                var levelPrefab = Resources.Load<GameObject>($"Levels/{levelNumber}");
                Object.Instantiate(levelPrefab);

                // todo load prefab with level
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                systems.SendOuter<LevelInitOuterCommand>();
                
                systems.CleanupOuter<LoadLevelOuterCommand>();

                // Debug.Log("LoadLevelExecutor FIN");
            }
        }
    }
}