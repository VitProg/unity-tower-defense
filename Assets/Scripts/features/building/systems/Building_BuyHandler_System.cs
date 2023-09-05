// using Leopotam.EcsProto;
// using Leopotam.EcsProto.QoL;
// using td.features.building.buildingShop.bus;
// using td.features.eventBus;
// using td.features.state;
// using td.features.tower;
// using td.utils;
// using UnityEngine;
//
// namespace td.features.building.systems
// {
//     //todo maybe need to move into buildingsShop
//     public class Building_BuyHandler_System : IProtoInitSystem, IProtoDestroySystem
//     {
//         [DI] private EventBus events;
//         [DI] private State state;
//         [DI] private Tower_Service towerService;
//         
//         public void Init(IProtoSystems systems)
//         {
//             events.global.ListenTo<Command_BuyBuilding>(OnCommand);
//         }
//
//         public void Destroy()
//         {
//             events.global.RemoveListener<Command_BuyBuilding>(OnCommand);
//         }
//         
//         // ----------------------------------------------------------------
//
//         private void OnCommand(ref Command_BuyBuilding cmd)
//         {
//             Debug.Log("Building_BuyHandler_System:OnCommand");
//             
//             if (state.GetEnergy() < cmd.price) return;
//             state.ReduceEnergy(cmd.price);
//
//             //todo buildTime
//             //todo add other
//
//             var entity = -1;
//             
//             switch (cmd.buildingId)
//             {
//                 case Constants.Buildings.ShardTower:
//                 {
//                     entity = towerService.SpawnShardTower(cmd.cellCoords, cmd.buildTime);
//                     break;
//                 }
//             }
//
//             if (entity > 0)
//             {
//                 // /*??*/state.Ex<BuildingShop_StateEx>().RefreshItems();
//                 events.unique.GetOrAdd<Command_Buildings_RefreshData>();
//             }
//         }
//     }
// }