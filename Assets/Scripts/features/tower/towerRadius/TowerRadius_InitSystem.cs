// using System;
// using Leopotam.EcsProto;
// using Leopotam.EcsProto.QoL;
// using td.features.building;
// using td.features.eventBus;
// using td.features.level;
// using td.features.level.cells;
// using td.features.shard.bus;
// using td.utils.ecs;
// using UnityEngine;
//
// namespace td.features.tower.towerRadius
// {
//     public class TowerRadius_InitSystem : ProtoIntervalableRunSystem
//     {
//         [DI] private Tower_Aspect aspect;
//         [DI] private Tower_Service towerService;
//         [DI] private Building_Service buildingService;
//         [DI] private LevelMap levelMap;
//
//         public override void IntervalRun(float deltaTime)
//         {
//             foreach (var towerEntity in aspect.itShardTower)
//             {
//                 ref var shardTower = ref towerService.GetShardTower(towerEntity);
//                 ref var building = ref buildingService.GetBuilding(towerEntity);
//                 if (!levelMap.HasCell(building.coords, CellTypes.CanBuild)) continue;
//
//                 ref var cell = ref levelMap.GetCell(building.coords, CellTypes.CanBuild);
//                 // todo check cell.build is equals tower
//                 
//                 if (cell.mb.IsHovered)
//                 
//             }
//         }
//
//         public TowerRadius_InitSystem(float interval, float timeShift, Func<float> getDeltaTime) : base(interval, timeShift, getDeltaTime)
//         {
//         }
//     }
// }