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
        private IEcsSystems systems;
        private EcsWorld world;
        private SharedData sharedData;

        private readonly EcsCustomInject<LevelData> levelData = default;

        public void PreInit(IEcsSystems systems)
        {
            Debug.Log("SturtupInitSystem RUN...");
            
            this.systems = systems;
            world = this.systems.GetWorld();
            sharedData = systems.GetShared<SharedData>();

            MakeGlobalEntity();
            LoadEnemiesData();

            EcsEventUtils.Send(systems, new LoadLevelCommand()
            {
                LevelNumber = 1
            });
            GlobalEntityUtils.AddComponent<IsLoading>(systems);
            
            Debug.Log("SturtupInitSystem FIN");
        }

        private void MakeGlobalEntity()
        {
            var globalEntity = world.NewEntity();
            world.GetPool<IsGlobal>().Add(globalEntity);
            sharedData.GlobalEntity = world.PackEntity(globalEntity);
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