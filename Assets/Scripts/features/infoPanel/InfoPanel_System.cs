using Leopotam.EcsLite;
using td.features.infoPanel.bus;
using td.features.shard;
using td.features.state;
using td.features.tower;

namespace td.features.infoPanel
{
    public class InfoPanel_System : IEcsInitSystem, IEcsDestroySystem
    {
        private readonly EcsInject<IEventBus> events;
        private readonly EcsInject<IState> state;
        private readonly EcsInject<Tower_Service> towerService;
        private readonly EcsInject<Shard_Service> shardService;
        private readonly EcsWorld world;
        
        public void Init(IEcsSystems systems)
        {
            events.Value.Global.ListenTo<Command_ShowTowerInfo>(ShowTowerInfo);
            events.Value.Global.ListenTo<Command_HideTowerInfo>(HideTowerInfo);
        }

        public void Destroy(IEcsSystems systems)
        {
            events.Value.Global.RemoveListener<Command_ShowTowerInfo>(ShowTowerInfo);
            events.Value.Global.RemoveListener<Command_HideTowerInfo>(HideTowerInfo);
        }
        
        // ----------------------------------------------------------------

        private void ShowTowerInfo(ref Command_ShowTowerInfo item)
        {
            if (!shardService.Value.HasShardInTower(item.towerEntity, out var shardEntity)) return;

            ref var shard = ref shardService.Value.GetShard(shardEntity);
            
            state.Value.InfoPanel.Clear();
            state.Value.InfoPanel.Shard = shard;
            state.Value.InfoPanel.Title = "Tower";
        }

        private void HideTowerInfo(ref Command_HideTowerInfo item)
        {
            if (!shardService.Value.HasShardInTower(item.towerEntity, out var shardEntity)) return;
            
            ref var shard = ref shardService.Value.GetShard(shardEntity);

            if (state.Value.InfoPanel.Shard.HasValue && state.Value.InfoPanel.Shard.Value._id_ == shard._id_)
            {
                state.Value.InfoPanel.Clear();
            }
        }
         
    }
}