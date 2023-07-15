using Leopotam.EcsLite;
using td.common;
using td.components.commands;
using td.components.flags;
using td.features.shards.commands;
using td.features.state;
using td.features.windows;
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
        [Inject] private State state;
        [Inject] private PrefabService prefabService;
        [Inject] private WindowsService windowsService;

        public void PreInit(IEcsSystems systems)
        {
            // Debug.Log("SturtupInitSystem RUN...");
            
            LoadEnemiesData();

            // systems.Outer<LoadLevelOuterCommand>().levelNumber = state.LevelNumber;
            // systems.Outer<IsLoadingOuter>();
            // systems.Outer<UIHideShardStoreOuterCommand>();
            
            // Debug.Log("SturtupInitSystem FIN");

            // todo make service for switch screens
            // var mainMenu = prefabService.GetPrefab(PrefabCategory.UI, "MainMenu");
            // var mainMenuGO = Object.Instantiate(mainMenu, sharedData.canvas.transform);
            // mainMenuGO.SetActive(true);
            var show = windowsService.Open(WindowsService.Type.MainMenu, true);
        }
        
        private void LoadEnemiesData()
        {
            var col = ResourcesUtils.LoadJson<EnemyConfigCollection>("Configs/enemies");
            sharedData.enemyConfigs = col.enemies;

            for (var index = 0; index < sharedData.enemyConfigs.Length; index++)
            {
                sharedData.enemyConfigs[index].prefab = 
                    (GameObject)Resources.Load(
                        $"Prefabs/enemies/{sharedData.enemyConfigs[index].prefabPath}",
                        typeof(GameObject)
                    );
            }
        }
    }
}