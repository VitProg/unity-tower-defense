using UnityEngine;
using Leopotam.EcsProto.QoL;
using td.common;
using td.features.camera;
using td.features.level.cells;
using td.features.shard;
using td.features.shard.components;
using td.features.state;
using td.features.tower;
using td.utils;

namespace td.features.level
{
    public class Level_Map_Service
    {
        [DI] private Level_Aspect aspect;
        [DI] private LevelMap levelMap;
        [DI] private Shard_Service shardService;
        [DI] private Tower_Service towerService;
        [DI] private Shard_Calculator calc;
        [DI] private Camera_Service cameraService;
        [DI] private State state;

        public (CanDropShardOnMapType, uint cost, ProtoPackedEntityWithWorld? towerEntity, ProtoPackedEntityWithWorld? shardEntity) CheckCanDropByScreen(Vector3 screenPosition, ref Shard shard)
        {
            var position = cameraService.GetMainCamera().ScreenToWorldPoint(screenPosition);
            position.z = 0f;
            var coords = HexGridUtils.PositionToCell(position);
            return CheckCanDrop(coords.x, coords.y, ref shard);
        }

        public (CanDropShardOnMapType, uint cost, ProtoPackedEntityWithWorld? towerEntity, ProtoPackedEntityWithWorld? shardEntity) CheckCanDrop(Vector2 position, ref Shard shard)
        {
            var coords = HexGridUtils.PositionToCell(position);
            return CheckCanDrop(coords.x, coords.y, ref shard);
        }

        public (CanDropShardOnMapType, uint cost, ProtoPackedEntityWithWorld? towerEntity, ProtoPackedEntityWithWorld? shardEntity) CheckCanDrop(ref Int2 coords, ref Shard shard) => CheckCanDrop(coords.x, coords.y, ref shard);

        public (CanDropShardOnMapType, uint cost, ProtoPackedEntityWithWorld? towerEntity, ProtoPackedEntityWithWorld? shardEntity) CheckCanDrop(int x, int y, ref Shard shard)
        {
            if (!levelMap.HasCell(x, y, CellTypes.CanBuild) && !levelMap.HasCell(x, y, CellTypes.CanWalk))
            {
                return (CanDropShardOnMapType.False, 0, null, null);
            }

            ref var cell = ref levelMap.GetCell(x, y);

            var cellIsCanBuild = cell is { IsEmpty: false, type: CellTypes.CanBuild, packedBuildingEntity: not null };
            
            // Insert into tower or combine whith shard in this tower
            if (
                cellIsCanBuild &&
                cell.packedBuildingEntity.Value.Unpack(out _, out var towerEntity) &&
                towerService.HasTower(towerEntity) &&
                towerService.HasShardTower(towerEntity)
            )
            {
                if (shardService.HasShardInTower(towerEntity, out var shardInTowerEntity) &&
                    shardService.HasShard(shardInTowerEntity))
                {
                    ref var shardInTower = ref shardService.GetShard(shardInTowerEntity);
                    var combineInTowerCost = calc.CalculateCombinerIntoTowerCost(ref shardInTower, ref shard); //calc.Value.CalculateCombineCost(ref shardInTower, ref shard);
                    return (
                        state.GetEnergy() >= combineInTowerCost
                            ? CanDropShardOnMapType.CombineInTower
                            : CanDropShardOnMapType.FalseCombineInTower,
                        combineInTowerCost,
                        aspect.World().PackEntityWithWorld(towerEntity),
                        aspect.World().PackEntityWithWorld(shardInTowerEntity)
                    );
                }

                if (shard.costInsert == 0) shardService.PrecalcAllCosts(ref shard);
                var insertCost = shard.costInsert; //calc.Value.CalculateInsertCost(ref shard);
                return (
                    state.GetEnergy() >= insertCost
                        ? CanDropShardOnMapType.InsertInTower
                        : CanDropShardOnMapType.FalseInsertInTower,
                    insertCost,
                    aspect.World().PackEntityWithWorld(towerEntity),
                    null
                );
            }

            // Drop To Floor / Explode Shard
            if (cell is
                {
                    IsEmpty: false, 
                    type: CellTypes.CanBuild or CellTypes.CanWalk or CellTypes.Barrier
                })
            {
                // drop on floor
                var dropCost = shard.costDrop;

                return (
                    state.GetEnergy() >= dropCost
                        ? CanDropShardOnMapType.DropToFloor
                        : CanDropShardOnMapType.FalseDropToFloor,
                    dropCost,
                    null, null
                );
            }
            
            return (CanDropShardOnMapType.False, 0, null, null);
        }

        public ref Cell GetCellByScreen(Vector3 screenPosition)
        {
            var position = cameraService.GetMainCamera().ScreenToWorldPoint(screenPosition);
            position.z = 0f;
            var coords = HexGridUtils.PositionToCell(position);
            return ref levelMap.GetCell(coords.x, coords.y);
        }
        
        private bool debugInfoVisible = false;
        public bool DebugInfoVisible
        {
            get => debugInfoVisible;
            set
            {
                if (debugInfoVisible == value) return;
                debugInfoVisible = value;

                cameraService.GetMainCamera().cullingMask ^= 1 << LayerMask.NameToLayer("Debug1");
                cameraService.GetMainCamera().cullingMask ^= 1 << LayerMask.NameToLayer("Debug2");
            }
        }
    }

    public enum CanDropShardOnMapType
    {
        FalseDropToFloor = -4,
        FalseCombineInTower = -3,
        // FalseCombineCost = -2,
        FalseInsertInTower = -1,
        False = 0,
        InsertInTower = 1,
        // Combine = 2,
        CombineInTower = 3,
        DropToFloor = 4,
    }
}