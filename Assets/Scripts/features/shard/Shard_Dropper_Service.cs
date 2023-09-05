// using Leopotam.EcsProto.QoL;
// using td.features.camera;
// using td.features.level;
// using td.features.level.cells;
// using td.features.shard.components;
// using td.features.state;
// using td.utils;
// using Unity.Mathematics;
// using UnityEngine;
//
// namespace td.features.shard
// {
//     public class Shard_Dropper_Service
//     {
//         [DI] private Shard_Aspect aspect;
//         [DI] private LevelMap levelMap;
//         [DI] private Shard_Service shardService;
//         [DI] private Shard_Calculator calc;
//         [DI] private Camera_Service cameraService;
//         [DI] private State state;
//         
//         public (CanDropShardOnMapType, uint cost, ProtoPackedEntityWithWorld? towerEntity, ProtoPackedEntityWithWorld? shardEntity) 
//             CheckCanDropByScreen(Vector3 screenPosition, ref Shard shard)
//         {
//             var position = cameraService.GetMainCamera().ScreenToWorldPoint(screenPosition);
//             position.z = 0f;
//             var coords = HexGridUtils.PositionToCell(position);
//             return CheckCanDrop(coords.x, coords.y, ref shard);
//         }
//
//         public (CanDropShardOnMapType, uint cost, ProtoPackedEntityWithWorld? towerEntity, ProtoPackedEntityWithWorld? shardEntity) 
//             CheckCanDrop(Vector2 position, ref Shard shard)
//         {
//             var coords = HexGridUtils.PositionToCell(position);
//             return CheckCanDrop(coords.x, coords.y, ref shard);
//         }
//
//         public (CanDropShardOnMapType, uint cost, ProtoPackedEntityWithWorld? towerEntity, ProtoPackedEntityWithWorld? shardEntity) 
//             CheckCanDrop(ref int2 coords, ref Shard shard) => CheckCanDrop(coords.x, coords.y, ref shard);
//
//         public (CanDropShardOnMapType, uint cost, ProtoPackedEntityWithWorld? towerEntity, ProtoPackedEntityWithWorld? shardEntity) 
//             CheckCanDrop(int x, int y, ref Shard shard)
//         {
//             if (!levelMap.HasCell(x, y, CellTypes.CanBuild) && !levelMap.HasCell(x, y, CellTypes.CanWalk))
//             {
//                 return (CanDropShardOnMapType.False, 0, null, null);
//             }
//
//             ref var cell = ref levelMap.GetCell(x, y);
//
//             var cellIsCanBuild = cell is { IsEmpty: false, type: CellTypes.CanBuild, packedBuildingEntity: not null };
//             
//             // Insert into tower or combine whith shard in this tower
//             if (
//                 cellIsCanBuild &&
//                 cell.packedBuildingEntity.Value.Unpack(out _, out var towerEntity) &&
//                 towerService.HasTower(towerEntity) &&
//                 towerService.HasShardTower(towerEntity)
//             )
//             {
//                 if (shardService.HasShardInTower(towerEntity, out var shardInTowerEntity) &&
//                     shardService.HasShard(shardInTowerEntity))
//                 {
//                     ref var shardInTower = ref shardService.GetShard(shardInTowerEntity);
//                     var combineInTowerCost = calc.CalculateCombineIntoTowerCost(ref shardInTower, ref shard); //calc.Value.CalculateCombineCost(ref shardInTower, ref shard);
//                     return (
//                         state.GetEnergy() >= combineInTowerCost
//                             ? CanDropShardOnMapType.CombineInTower
//                             : CanDropShardOnMapType.FalseCombineInTower,
//                         combineInTowerCost,
//                         aspect.World().PackEntityWithWorld(towerEntity),
//                         aspect.World().PackEntityWithWorld(shardInTowerEntity)
//                     );
//                 }
//
//                 if (shard.costInsert == 0) shardService.PrecalcAllCostsAndTimes(ref shard);
//                 var insertCost = shard.costInsert; //calc.Value.CalculateInsertCost(ref shard);
//                 return (
//                     state.GetEnergy() >= insertCost
//                         ? CanDropShardOnMapType.InsertInTower
//                         : CanDropShardOnMapType.FalseInsertInTower,
//                     insertCost,
//                     aspect.World().PackEntityWithWorld(towerEntity),
//                     null
//                 );
//             }
//
//             // Drop To Floor / Explode Shard
//             if (cell is
//                 {
//                     IsEmpty: false, 
//                     type: CellTypes.CanBuild or CellTypes.CanWalk or CellTypes.Barrier
//                 })
//             {
//                 // drop on floor
//                 var dropCost = shard.costDrop;
//
//                 return (
//                     state.GetEnergy() >= dropCost
//                         ? CanDropShardOnMapType.DropToFloor
//                         : CanDropShardOnMapType.FalseDropToFloor,
//                     dropCost,
//                     null, null
//                 );
//             }
//             
//             return (CanDropShardOnMapType.False, 0, null, null);
//         }
//     }
// }