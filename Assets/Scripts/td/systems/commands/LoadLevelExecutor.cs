using System;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.common.level;
using td.components.commands;
using td.services;
using td.utils;
using td.utils.ecs;
using UnityEngine;

namespace td.systems.commands
{
    public class LoadLevelExecutor: IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<LoadLevelCommand>> entities = Constants.Ecs.EventWorldName;
        private readonly EcsCustomInject<LevelData> levelData = default;
        
        public void Run(IEcsSystems systems)
        {
            var entity = EcsEventUtils.Single(entities);
            
            if (entity == null) return;
            
            Debug.Log("LoadLevelExecutor RUN...");
            
            var levelNumber = entities.Pools.Inc1.Get((int)entity).LevelNumber;
            try
            {
                var levelConfig = ResourcesUtils.LoadJson<LevelConfig>($@"Levels/{levelNumber}");
            
                Debug.Log(levelConfig);
            
                // todo load prefab with level
                
                levelData.Value.levelConfig = levelConfig;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                EcsEventUtils.CleanupEvent(systems, entities);
                
                EcsEventUtils.Send<LevelInitCommand>(systems);
                
                Debug.Log("LoadLevelExecutor FIN");
            }
        }
    }
}