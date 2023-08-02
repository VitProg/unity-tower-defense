using UnityEngine;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.common;
using td.features._common;
using td.features.level.cells;
using td.features.shard;
using td.features.shard.components;
using td.features.state;
using td.features.tower;
using td.monoBehaviours;
using td.utils;

namespace td.features.level
{
    public class LevelMap_Service
    {
        private readonly EcsInject<LevelMap> levelMap;
        private readonly EcsInject<SharedData> shared;
        private readonly EcsInject<Shard_Service> shardService;
        private readonly EcsInject<Tower_Service> towerService;
        private readonly EcsInject<ShardCalculator> calc;
        private readonly EcsInject<IState> state;
        private readonly EcsWorldInject world;

        public (CanDropShardOnMapType, uint cost, EcsPackedEntity? towerEntity, EcsPackedEntity? shardEntity) CheckCanDropByScreen(Vector3 screenPosition, ref Shard shard)
        {
            var position = shared.Value.mainCamera.ScreenToWorldPoint(screenPosition);
            position.z = 0f;
            var coords = HexGridUtils.PositionToCell(position);
            return CheckCanDrop(coords.x, coords.y, ref shard);
        }

        public (CanDropShardOnMapType, uint cost, EcsPackedEntity? towerEntity, EcsPackedEntity? shardEntity) CheckCanDrop(Vector2 position, ref Shard shard)
        {
            var coords = HexGridUtils.PositionToCell(position);
            return CheckCanDrop(coords.x, coords.y, ref shard);
        }

        public (CanDropShardOnMapType, uint cost, EcsPackedEntity? towerEntity, EcsPackedEntity? shardEntity) CheckCanDrop(ref Int2 coords, ref Shard shard) => CheckCanDrop(coords.x, coords.y, ref shard);

        public (CanDropShardOnMapType, uint cost, EcsPackedEntity? towerEntity, EcsPackedEntity? shardEntity) CheckCanDrop(int x, int y, ref Shard shard)
        {
            if (!levelMap.Value.HasCell(x, y, CellTypes.CanBuild) && !levelMap.Value.HasCell(x, y, CellTypes.CanWalk))
            {
                return (CanDropShardOnMapType.False, 0, null, null);
            }

            ref var cell = ref levelMap.Value.GetCell(x, y);

            var cellIsCanBuild = cell is { IsEmpty: false, type: CellTypes.CanBuild, packedBuildingEntity: not null };
            
            // Insert into tower or combine whith shard in this tower
            if (
                cellIsCanBuild &&
                cell.packedBuildingEntity.Value.Unpack(world.Value, out var towerEntity) &&
                towerService.Value.HasTower(towerEntity) &&
                towerService.Value.HasShardTower(towerEntity)
            )
            {
                if (shardService.Value.HasShardInTower(towerEntity, out var shardInTowerEntity) &&
                    shardService.Value.HasShard(shardInTowerEntity))
                {
                    ref var shardInTower = ref shardService.Value.GetShard(shardInTowerEntity);
                    var combineInTowerCost = calc.Value.CalculateCombinerIntoTowerCost(ref shardInTower, ref shard); //calc.Value.CalculateCombineCost(ref shardInTower, ref shard);
                    return (
                        state.Value.Energy >= combineInTowerCost
                            ? CanDropShardOnMapType.CombineInTower
                            : CanDropShardOnMapType.FalseCombineInTower,
                        combineInTowerCost,
                        world.Value.PackEntity(towerEntity),
                        world.Value.PackEntity(shardInTowerEntity)
                    );
                }

                var insertCost = shard.costInsert; //calc.Value.CalculateInsertCost(ref shard);
                return (
                    state.Value.Energy >= insertCost
                        ? CanDropShardOnMapType.InsertInTower
                        : CanDropShardOnMapType.FalseInsertInTower,
                    insertCost,
                    world.Value.PackEntity(towerEntity), null
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
                    state.Value.Energy >= dropCost
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
            var position = shared.Value.mainCamera.ScreenToWorldPoint(screenPosition);
            position.z = 0f;
            var coords = HexGridUtils.PositionToCell(position);
            return ref levelMap.Value.GetCell(coords.x, coords.y);
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