using System;
using Leopotam.EcsProto;
using td.features.building.buildingShop.bus;
using td.features.building.buildingShop.state;
using td.features.building.buildingShop.systems;
using td.features.eventBus;
using td.features.state;
using td.features.state.interfaces;
using td.utils.ecs;

namespace td.features.building.buildingShop
{
    public class BuildingShop_Module : IProtoModuleWithStateEx, IProtoModuleWithEvents
    {
        public void Init(IProtoSystems systems)
        {
            systems
                .AddSystem(new BuildingShop_InitSystem())
                .AddSystem(new BuildingShop_VisibleSystem())
                ;
        }

        public IProtoAspect[] Aspects()
        {
            return new IProtoAspect[]
            {
                new BuildingShop_Aspect()
            };
        }

        public IProtoModule[] Modules()
        {
            return null;
        }

        public Type[] Events() => Ev.E<
            Command_BuyBuilding,
            Command_Buildings_RefreshData
        >();

        public IStateExtension StateEx() => new BuildingShop_StateEx();
    }
}