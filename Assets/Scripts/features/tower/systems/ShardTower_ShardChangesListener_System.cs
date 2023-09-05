using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using td.features.eventBus;
using td.features.shard;
using td.features.shard.bus;

namespace td.features.tower.systems {
    public class ShardTower_ShardChangesListener_System : IProtoRunSystem, IProtoDestroySystem {
        [DI] private EventBus events;
        [DI] private Tower_Service towerService;
        [DI] private Shard_Service shardService;
        
        public void Run() {
            events.global.ListenTo<Event_ShardInserted_InBuilding>(OnShardInserted);
            events.global.ListenTo<Event_ShardsCombined>(OnShardsCombined);
        }

        public void Destroy() {
            events.global.RemoveListener<Event_ShardInserted_InBuilding>(OnShardInserted);
            events.global.RemoveListener<Event_ShardsCombined>(OnShardsCombined);
        }

        private void OnShardInserted(ref Event_ShardInserted_InBuilding ev) {
            if (!ev.ShardEntity.Unpack(out var shardWorld, out var shardEntity)) return;
            if (!ev.BuildingEntity.Unpack(out var buildingWorld, out var builtingEntity)) return;
            if (!shardService.HasShard(shardEntity)) return;
            if (!towerService.HasShardTower(builtingEntity)) return;

            ref var shard = ref shardService.GetShard(shardEntity);
            ref var tower = ref towerService.GetShardTower(builtingEntity);
            
            tower.SetRadius(shard.radius);
        }

        private void OnShardsCombined(ref Event_ShardsCombined ev) {
            if (!ev.ShardEntity.Unpack(out var shardWorld, out var shardEntity)) return;
            if (!ev.BuildingEntity.Unpack(out var buildingWorld, out var builtingEntity)) return;
            if (!shardService.HasShard(shardEntity)) return;
            if (!towerService.HasShardTower(builtingEntity)) return;
            
            ref var shard = ref shardService.GetShard(shardEntity);
            ref var tower = ref towerService.GetShardTower(builtingEntity);
            
            tower.SetRadius(shard.radius);
        }
    }
}
