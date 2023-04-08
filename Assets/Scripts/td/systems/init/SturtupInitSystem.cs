using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.common;
using td.components.commands;
using td.components.flags;
using td.services;
using td.states;
using td.utils;
using td.utils.ecs;
using UnityEngine;

namespace td.systems.init
{
    public class SturtupInitSystem : IEcsPreInitSystem
    {
        [EcsWorld] private EcsWorld world;
        [EcsShared] private SharedData sharedData;
        [EcsInject] private LevelState levelState;

        public void PreInit(IEcsSystems systems)
        {
            // Debug.Log("SturtupInitSystem RUN...");
            
            MakeGlobalEntity();
            LoadEnemiesData();

            systems.SendOuter(new LoadLevelOuterCommand()
            {
                levelNumber = levelState.LevelNumber
            });
            systems.SendOuter<IsLoadingOuter>();
            
            // Debug.Log("SturtupInitSystem FIN");
        }

        private void MakeGlobalEntity()
        {
            var globalEntity = world.NewEntity();
            world.GetPool<IsGlobal>().Add(globalEntity);
        }

        private void LoadEnemiesData()
        {
            var col = ResourcesUtils.LoadJson<EnemyConfigCollection>("Configs/enemies");
            sharedData.EnemyConfigs = col.enemies;

            for (var index = 0; index < sharedData.EnemyConfigs.Length; index++)
            {
                sharedData.EnemyConfigs[index].prefab = 
                    (GameObject)Resources.Load(
                        $@"Prefabs/enemies/{sharedData.EnemyConfigs[index].prefabPath}",
                        typeof(GameObject)
                    );
            }

            Debug.Log(sharedData.EnemyConfigs);
        }
    }
}