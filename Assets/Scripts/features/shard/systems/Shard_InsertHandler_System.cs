using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using td.features._common;
using td.features.building;
using td.features.eventBus;
using td.features.level;
using td.features.level.cells;
using td.features.movement;
using td.features.shard.bus;
using td.features.shard.shardCollection;
using td.features.shard.shardStore;
using td.features.state;
using td.utils;

namespace td.features.shard.systems
{
    public class Shard_InsertHandler_System : IProtoInitSystem, IProtoDestroySystem
    {
        [DI] private EventBus events;
        [DI] private State state;
        [DI] private Level_State levelState;
        [DI] private Shard_Service shardService;
        [DI] private Movement_Service movementService;
        [DI] private Building_Service buildingService;
        [DI] private Common_Service common;

        private ShardCollection_State _collectionState;
        private ShardCollection_State CollectionState => _collectionState ??= state.Ex<ShardCollection_State>();
        
        public void Init(IProtoSystems systems)
        {
            events.global.ListenTo<Command_InsertShard_InBuilding>(OnInsert);
        }

        public void Destroy()
        {
            events.global.RemoveListener<Command_InsertShard_InBuilding>(OnInsert);
        }
        
        // ----------------------------------------------------------------

        private void OnInsert(ref Command_InsertShard_InBuilding cmd)
        {
            if (cmd.cost <= 0 || !state.IsEnoughEnergy(cmd.cost)) return;
            if (!CollectionState.HasItem(cmd.sourceIndex)) return;
            // if (!CollectionState.HasOperationTargetEntity()) return;

            var targetEntity = cmd.targetBuilding;
            if (!targetEntity.Unpack(out var world, out var buildingEntity)) return;

            ref var building = ref buildingService.GetBuilding(buildingEntity);

            ref var cell = ref levelState.GetCell(building.coords.x, building.coords.y);
            if (cell.IsEmpty || (cell.type != CellTypes.CanWalk && cell.type != CellTypes.CanBuild)) return;
            if (!cell.HasBuilding() || cell.HasShard()) return;
            if (cell.buildingId != Constants.Buildings.ShardTower && cell.buildingId != Constants.Buildings.ShardTrap) return;
            if (!cell.packedBuildingEntity.EqualsTo(targetEntity)) return;
            
            // OK

            var position = HexGridUtils.CellToPosition(cell.coords);

            if (movementService.HasTargetPoint(buildingEntity))
            {
                ref var shardTowerTargetPoint = ref movementService.GetTargetPointPool(buildingEntity);
                position.x += shardTowerTargetPoint.Point.x;
                position.y += shardTowerTargetPoint.Point.y;
            }

            state.ReduceEnergy(cmd.cost);

            var shard = CollectionState.GetItem(cmd.sourceIndex);
            var shardEntity = shardService.SpawnShard(
                ref shard,
                position,
                common.GetGOTransform(buildingEntity)
            );
            
            cell.packedShardEntity = world.PackEntityWithWorld(shardEntity);
            
            CollectionState.RemoveItemAt(cmd.sourceIndex);
            
            ref var ev = ref events.global.Add<Event_ShardInserted_InBuilding>();
            ev.BuildingEntity = cmd.targetBuilding;
            ev.ShardEntity = cell.packedShardEntity;

            // todo use cmd.time !
        }
    }
}