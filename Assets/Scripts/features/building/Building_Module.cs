using Leopotam.EcsProto;
using td.features.building.buildingShop;
using td.features.building.data;
using td.features.building.systems;
using td.utils.ecs;
using UnityEngine;

namespace td.features.building
{
    public class Building_Module : IProtoModule
    {
        private readonly Buildings_Config_SO buildingsConfigsSO;

        public Building_Module()
        {
            buildingsConfigsSO = Resources.Load<Buildings_Config_SO>("Configs/Buildings Config");
        }
        
        public void Init(IProtoSystems systems)
        {
            systems
                .AddSystem(new Building_InitData_System())
                // .AddSystem(new Building_BuyHandler_System())
                //
                .AddService(new Building_Service(), true)
                .AddService(buildingsConfigsSO, true)
                ;
        }

        public IProtoAspect[] Aspects()
        {
            return new IProtoAspect[]
            {
                new Building_Aspect(),
            };
        }

        public IProtoModule[] Modules()
        {
            return new IProtoModule[]
            {
                new BuildingShop_Module()
            };
        }
    }
}