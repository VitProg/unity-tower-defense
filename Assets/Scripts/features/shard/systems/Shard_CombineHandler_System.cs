using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using td.features.building;
using td.features.eventBus;
using td.features.level;
using td.features.level.cells;
using td.features.shard.bus;
using td.features.shard.shardCollection;
using td.features.state;
using td.utils;

namespace td.features.shard.systems
{
    public class Shard_CombineHandler_System : IProtoInitSystem, IProtoDestroySystem
    {
        [DI] private EventBus events;
        [DI] private State state;
        [DI] private Level_State levelState;
        [DI] private Building_Service buildingService;
        [DI] private Shard_Service shardService;
        [DI] private Shard_Calculator calc;

        private ShardCollection_State _collectionState;
        private ShardCollection_State CollectionState => _collectionState ??= state.Ex<ShardCollection_State>();
        
        public void Init(IProtoSystems systems)
        {
            events.global.ListenTo<Command_CombineShards_InCollection>(OnCombineInCollection);
            events.global.ListenTo<Command_CombineShards_InBuilding>(OnCombineInBuilding);
        }

        public void Destroy()
        {
            events.global.RemoveListener<Command_CombineShards_InCollection>(OnCombineInCollection);
            events.global.RemoveListener<Command_CombineShards_InBuilding>(OnCombineInBuilding);
        }
        
        // ----------------------------------------------------------------
        
        private void OnCombineInCollection(ref Command_CombineShards_InCollection cmd)
        {
            if (cmd.cost <= 0 || !state.IsEnoughEnergy(cmd.cost)) return;
            if (!CollectionState.HasItem(cmd.sourceIndex)) return;
            if (!CollectionState.HasItem(cmd.targetIndex)) return;

            // OK
            
            state.ReduceEnergy(cmd.cost);
            
            ref var sourceShard = ref CollectionState.GetItem(cmd.sourceIndex);
            ref var targetShard = ref CollectionState.GetItem(cmd.targetIndex); 

            targetShard.CombineWith(ref sourceShard);
            
            shardService.PrecalcAllData(ref targetShard);
            CollectionState.UpdateItems();
            
            CollectionState.RemoveItemAt(cmd.sourceIndex);
        }

        private void OnCombineInBuilding(ref Command_CombineShards_InBuilding cmd)
        {
            if (cmd.cost <= 0 || !state.IsEnoughEnergy(cmd.cost)) return;
            if (!CollectionState.HasItem(cmd.sourceIndex)) return;

            var targetEntity = cmd.targetBuilding;
            if (!targetEntity.Unpack(out _, out var buildingUntity)) return;

            ref var building = ref buildingService.GetBuilding(buildingUntity);

            ref var cell = ref levelState.GetCell(building.coords.x, building.coords.y);
            if (cell.IsEmpty || (cell.type != CellTypes.CanWalk && cell.type != CellTypes.CanBuild)) return;
            if (!cell.HasBuilding() || !cell.HasShard()) return;
            if (cell.buildingId != Constants.Buildings.ShardTower && cell.buildingId != Constants.Buildings.ShardTrap) return;
            if (!cell.packedBuildingEntity.EqualsTo(targetEntity)) return;
            if (!shardService.HasShard(cell.packedShardEntity, out var targetShardEntity)) return;
            
            // OK
            
            state.ReduceEnergy(cmd.cost);
            
            var shard = CollectionState.GetItem(cmd.sourceIndex);

            ref var targetShard = ref shardService.GetShard(targetShardEntity);
            targetShard.CombineWith(ref shard);
            
            shardService.PrecalcAllData(ref targetShard);
            var shardMB = shardService.GetShardMB(targetShardEntity);
            shardMB.shard = targetShard;
            shardMB.FullRefresh();
            
            ref var ev = ref events.global.Add<Event_ShardsCombined>();
            ev.BuildingEntity = cmd.targetBuilding;
            ev.ShardEntity = cell.packedShardEntity;

            CollectionState.RemoveItemAt(cmd.sourceIndex);
        }
    }
}