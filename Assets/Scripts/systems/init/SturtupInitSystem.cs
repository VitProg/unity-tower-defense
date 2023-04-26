using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.common;
using td.components.commands;
using td.components.flags;
using td.services;
using td.utils;
using td.utils.ecs;
using UnityEngine;

namespace td.systems.init
{
    public class SturtupInitSystem : IEcsPreInitSystem
    {
        [InjectWorld] private EcsWorld world;
        [InjectShared] private SharedData sharedData;
        [Inject] private LevelState levelState;

        public void PreInit(IEcsSystems systems)
        {
            // Debug.Log("SturtupInitSystem RUN...");
            
            LoadEnemiesData();

            systems.Outer<LoadLevelOuterCommand>().levelNumber = levelState.LevelNumber;
            systems.Outer<IsLoadingOuter>();
            
            // Debug.Log("SturtupInitSystem FIN");
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
        }
    }
}