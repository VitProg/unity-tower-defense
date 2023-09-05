using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using td.features.building.buildingShop.bus;
using td.features.eventBus;
using td.features.state;
using td.features.tower.bus;
using td.utils;
using UnityEngine;

namespace td.features.tower.systems
{
    public class Tower_BuyHandler_System : IProtoInitSystem, IProtoDestroySystem
    {
        [DI] private Tower_Aspect aspect;
        [DI] private EventBus events;
        [DI] private State state;
        [DI] private Tower_Service towerService;
 
        public void Init(IProtoSystems systems)
        {
            events.global.ListenTo<Command_BuyBuilding>(OnCommand);
        }
        public void Destroy()
        {
            events.global.RemoveListener<Command_BuyBuilding>(OnCommand);
        }
 
        // ----------------------------------------------------------------
        private void OnCommand(ref Command_BuyBuilding cmd)
        {
            if (cmd.buildingId != Constants.Buildings.ShardTower) return;
            
            Debug.Log("Tower_BuyHandler_System:OnCommand");
     
            if (state.GetEnergy() < cmd.price) return;
            state.ReduceEnergy(cmd.price);
            //todo buildTime
            //todo add other
            var entity = towerService.SpawnShardTower(cmd.cellCoords, cmd.buildTime);
            if (entity > 0)
            {
                events.unique.GetOrAdd<Command_Buildings_RefreshData>();
            }

            events.global.Add<Event_Tower_Created>().Tower = aspect.World().PackEntityWithWorld(entity);
        }
    }
}