using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using td.features.enemy.data;
using td.utils;
using td.utils.di;
using UnityEngine;

namespace td.features.enemy.systems
{
    public class Enemy_InitData_System : IProtoPreInitSystem
    {
        public void PreInit(IProtoSystems systems)
        {
            var enemyConfigs = ResourcesUtils.LoadJson<EnemyConfigCollection>("Configs/enemies").enemies;
            
            for (var index = 0; index < enemyConfigs.Length; index++)
            {
                enemyConfigs[index].prefab = 
                    (GameObject)Resources.Load(
                        $"Prefabs/enemies/{enemyConfigs[index].prefabPath}",
                        typeof(GameObject)
                    );
            }

            ServiceContainer.Set(enemyConfigs);
        }
    }
}