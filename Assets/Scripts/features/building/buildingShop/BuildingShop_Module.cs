using Leopotam.EcsProto;
using td.features.building.buildingShop.state;
using td.features.building.buildingShop.systems;
using td.features.state;
using td.utils.ecs;

namespace td.features.building.buildingShop
{
    public class BuildingShop_Module : IProtoModuleWithStateEx
    {
        private readonly BuildingShop_StateEx stateEx;

        public BuildingShop_Module()
        {
            stateEx = new BuildingShop_StateEx();
        }
        
        public void Init(IProtoSystems systems)
        {
            systems
                .AddSystem(new BuildingShop_InitSystem())
                .AddSystem(new BuildingShop_VisibleSystem())
                //
                .AddService(stateEx, true)
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

        public IStateExtension StateEx() => stateEx;
    }
}