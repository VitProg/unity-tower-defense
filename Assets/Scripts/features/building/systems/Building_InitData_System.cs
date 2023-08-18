using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using td.features.building.data;
using td.features.prefab;
using td.utils;
using td.utils.di;

namespace td.features.building.systems
{
    public class Building_InitData_System : IProtoPreInitSystem
    {
        [DI] private Prefab_Service prefabService;
        
        public void PreInit(IProtoSystems systems)
        {
            var buildingsConfigs = ResourcesUtils.LoadJson<Building_Config_Collection>("Configs/buildings").buildings;
            
            for (var idx = 0; idx < buildingsConfigs.Length; idx++)
            {
                buildingsConfigs[idx].prefab = prefabService.GetPrefab(PrefabCategory.Buildings, buildingsConfigs[idx].prefabName);
            }
            ServiceContainer.Set(buildingsConfigs);
        }
    }
}