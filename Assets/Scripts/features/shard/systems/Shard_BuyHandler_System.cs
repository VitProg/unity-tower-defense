using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using td.features.eventBus;
using td.features.shard.bus;
using td.features.shard.shardCollection;
using td.features.shard.shardStore;
using td.features.state;

namespace td.features.shard.systems
{
    public class Shard_BuyHandler_System : IProtoInitSystem, IProtoDestroySystem
    {
        [DI] private EventBus events;
        [DI] private State state;

        private ShardCollection_State _collectionState;
        private ShardCollection_State CollectionState => _collectionState ??= state.Ex<ShardCollection_State>();
        
        private ShardStore_State _storeState;
        private ShardStore_State StoreState => _storeState ??= state.Ex<ShardStore_State>();
        
        public void Init(IProtoSystems systems)
        {
            events.global.ListenTo<Command_BuyShard>(OnCommand);
        }

        public void Destroy()
        {
            events.global.RemoveListener<Command_BuyShard>(OnCommand);
        }
        
        // ----------------------------------------------------------------

        private void OnCommand(ref Command_BuyShard cmd)
        {
            if (cmd.shard.price == 0 || !state.IsEnoughEnergy(cmd.shard.price)) return;

            var newShard = cmd.shard.MakeCopy();

            state.ReduceEnergy(cmd.shard.price);

            CollectionState.AddItem(ref newShard);
            StoreState.SetVisible(false);
        }
    }
}