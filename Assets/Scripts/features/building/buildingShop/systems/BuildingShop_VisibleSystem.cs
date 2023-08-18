using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using td.features.building.buildingShop.state;
using td.features.eventBus;
using td.features.level.bus;
using td.features.state;

namespace td.features.building.buildingShop.systems
{
    public class BuildingShop_VisibleSystem : IProtoInitSystem, IProtoDestroySystem
    {
        [DI] private EventBus events;
        [DI] private State state;
        [DI] private BuildingShop_StateEx stateEx;
        
        public void Init(IProtoSystems systems)
        {
            events.global.ListenTo<Event_CellCanBuild_Clicked>(OnCellClicked);
        }

        public void Destroy()
        {
            events.global.RemoveListener<Event_CellCanBuild_Clicked>(OnCellClicked);
        }
        
        // ----------------------------------------------------------------

        private void OnCellClicked(ref Event_CellCanBuild_Clicked ev)
        {
            if (ev.isLong)
            {
                stateEx.SetVisible(true);
                stateEx.SetCellCoords(ev.coords.x, ev.coords.y);
            }
        }
    }
}