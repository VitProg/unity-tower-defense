using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using td.features.eventBus;
using td.features.infoPanel.bus;
using td.features.shard;
using td.features.state;
using td.features.tower;

namespace td.features.infoPanel
{
    public class InfoPanel_System : IProtoInitSystem, IProtoDestroySystem
    {
        [DI] private EventBus events;
        [DI] private State state;
        [DI] private Tower_Service towerService;
        [DI] private Shard_Service shardService;
        
        public void Init(IProtoSystems systems)
        {
            events.global.ListenTo<Command_ShowTowerInfo>(ShowTowerInfo);
            events.global.ListenTo<Command_HideTowerInfo>(HideTowerInfo);
        }

        public void Destroy()
        {
            events.global.RemoveListener<Command_ShowTowerInfo>(ShowTowerInfo);
            events.global.RemoveListener<Command_HideTowerInfo>(HideTowerInfo);
        }
        
        // ----------------------------------------------------------------

        private void ShowTowerInfo(ref Command_ShowTowerInfo item)
        {
            if (!shardService.HasShardInTower(item.towerEntity, out var shardEntity)) return;

            var infoPanel = state.Ex<InfoPanel_StateExtension>();
            infoPanel.Clear();
            infoPanel.SetShard(ref shardService.GetShard(shardEntity));
            infoPanel.SetTitle("Tower");
            infoPanel.SetVisible(true);
        }

        private void HideTowerInfo(ref Command_HideTowerInfo item)
        {
            if (!shardService.HasShardInTower(item.towerEntity, out var shardEntity)) return;
            
            ref var shard = ref shardService.GetShard(shardEntity);

            var infoPanel = state.Ex<InfoPanel_StateExtension>();
            if (infoPanel.HasShard() && infoPanel.GetShard()._id_ == shard._id_)
            {
                infoPanel.Clear();
            }
        }
         
    }
}