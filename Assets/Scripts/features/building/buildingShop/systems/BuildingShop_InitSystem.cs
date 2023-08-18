using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using td.features.building.buildingShop.state;
using td.features.building.data;
using td.features.eventBus;
using td.features.level;
using td.features.level.bus;
using td.features.shard;
using td.features.state;
using td.utils.di;

namespace td.features.building.buildingShop.systems
{
    public class BuildingShop_InitSystem : IProtoInitSystem, IProtoDestroySystem
    {
        [DI] private State state;
        [DI] private LevelMap levelMap;
        [DI] private Shard_Calculator calc;
        [DI] private EventBus events;
        [DI] private Building_Service buildingService;
        
        public void Init(IProtoSystems systems)
        {
            events.unique.ListenTo<Event_LevelLoaded>(OnEvent);
        }

        public void Destroy()
        {
            events.unique.RemoveListener<Event_LevelLoaded>(OnEvent);
        }

        // --------------------------------------------- //

        private void OnEvent(ref Event_LevelLoaded ev)
        {
            // todo: we need to determine which buildings are available to the player and add only those to the store

            var s = state.Ex<BuildingShop_StateEx>();
            s.Clear();
            

            var buildingConfigs = ServiceContainer.Get<Building_Config[]>();

            for (var idx = 0; idx < buildingConfigs.Length; idx++)
            {
                var config = buildingConfigs[idx];
                var item = new BuildingShop_Item();
                var count = buildingService.GetCount(config.id);
                
                item.id = config.id;
                item.name = config.name;
                item.price = buildingService.CalcPrice(ref config, count);
                item.buildTime = buildingService.CalcBuildingTime(ref config, count);
                
                s.SetBuilding(ref item);
            }
        }

    }
}