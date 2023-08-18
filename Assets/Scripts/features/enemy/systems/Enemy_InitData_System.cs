using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using td.features.enemy.data;
using td.features.prefab;
using td.utils;
using td.utils.di;

namespace td.features.enemy.systems
{
    public class Enemy_InitData_System : IProtoPreInitSystem
    {
        [DI] private Prefab_Service prefabService;
        
        public void PreInit(IProtoSystems systems)
        {
            var enemyConfigs = ResourcesUtils.LoadJson<EnemyConfigCollection>("Configs/enemies").enemies;
            for (var index = 0; index < enemyConfigs.Length; index++)
            {
                enemyConfigs[index].prefab = prefabService.GetPrefab(PrefabCategory.Enemies, enemyConfigs[index].prefabName);
            }
            ServiceContainer.Set(enemyConfigs);
        }
    }
}