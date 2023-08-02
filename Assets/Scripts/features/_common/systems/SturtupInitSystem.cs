using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.features.enemy.data;
using td.features.state;
using td.features.window;
using td.utils;
using UnityEngine;

namespace td.features._common.systems
{
    public class SturtupInitSystem : IEcsPreInitSystem
    {
        private readonly EcsWorldInject world;
        private readonly EcsInject<SharedData> sharedData;
        private readonly EcsInject<IState> state;
        private readonly EcsInject<Prefab_Service> prefabService;
        private readonly EcsInject<Windows_Service> windowsService;

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
            var show = windowsService.Value.Open(Windows_Service.Type.MainMenu, true);
        }
        
        private void LoadEnemiesData()
        {
            var col = ResourcesUtils.LoadJson<EnemyConfigCollection>("Configs/enemies");
            sharedData.Value.enemyConfigs = col.enemies;

            for (var index = 0; index < sharedData.Value.enemyConfigs.Length; index++)
            {
                sharedData.Value.enemyConfigs[index].prefab = 
                    (GameObject)Resources.Load(
                        $"Prefabs/enemies/{sharedData.Value.enemyConfigs[index].prefabPath}",
                        typeof(GameObject)
                    );
            }
        }
    }
}