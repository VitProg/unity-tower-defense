using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using td.features.eventBus;
using td.features.level.bus;
using td.features.shard;
using td.features.shard.shardCollection;
using td.features.shard.shardStore;
using td.features.state;

namespace td.features.priceTimePopup.systems
{
    public class PriceTimePopup_System : IProtoInitSystem, IProtoDestroySystem
    {
        [DI] private EventBus events;
        [DI] private State state;
        [DI] private Shard_Service shardService;

        private PriceTimePopup_State _popupState;
        private PriceTimePopup_State PopupState => _popupState ??= state.Ex<PriceTimePopup_State>();
        
        private ShardStore_State _shardStoreState;
        private ShardStore_State ShardStoreState => _shardStoreState ??= state.Ex<ShardStore_State>();
        
        private ShardCollection_State _shardCollectionState;
        private ShardCollection_State ShardCollectionState => _shardCollectionState ??= state.Ex<ShardCollection_State>();
        
        public void Init(IProtoSystems systems)
        {
            events.unique.ListenTo<Event_LevelPreLoaded>(OnLevelPreLoaded);
            events.unique.ListenTo<Event_ShardStore_StateChanged>(OnShardStoreStateChanged);
            events.unique.ListenTo<Event_ShardCollection_StateChanged>(OnShardCollectionStateChanged);
        }
        
        public void Destroy()
        {
            events.unique.RemoveListener<Event_LevelPreLoaded>(OnLevelPreLoaded);
            events.unique.RemoveListener<Event_ShardStore_StateChanged>(OnShardStoreStateChanged);
            events.unique.RemoveListener<Event_ShardCollection_StateChanged>(OnShardCollectionStateChanged);
        }
        
        // ----------------------------------------------------------------
        
        private void OnLevelPreLoaded(ref Event_LevelPreLoaded obj)
        {
            PopupState.SetVisible(false);
        }
        
        private void OnShardCollectionStateChanged(ref Event_ShardCollection_StateChanged ev)
        {
            var s = ShardCollectionState;

            if (ev.operation && s.IsAnyOperation())
            {
                PopupState.Clear();

                if (s.IsCombineOperation()) PopupState.SetTitle("Combine shards");
                if (s.IsInsertOperation()) PopupState.SetTitle("Insert shard");
                if (s.IsDropOperation()) PopupState.SetTitle("Explode shard");
                
                PopupState.SetPrice(s.GetOperationPrice());
                PopupState.SetTime(s.GetOperationTime());
                PopupState.SetIsFine(state.IsEnoughEnergy(s.GetOperationPrice()));
                PopupState.SetVisible(true);
            }

            if (ev.operation && !s.IsAnyOperation())
            {
                PopupState.Clear();
            }
        }

        private void OnShardStoreStateChanged(ref Event_ShardStore_StateChanged ev)
        {
            if (!ev.hoveredIndex) return;

            if (ShardStoreState.HasHovered())
            {
                var shardItem = ShardStoreState.GetItem(ShardStoreState.GetHoveredIndex());
                
                PopupState.Clear();
                PopupState.SetTitle("Buy a shard");
                PopupState.SetTargetId(shardItem.shard._id_);
                PopupState.SetPrice(shardItem.shard.price);
                PopupState.SetIsFine(state.IsEnoughEnergy(shardItem.shard.price));
                PopupState.SetVisible(true);
            }
            else
            {
                PopupState.Clear();
                /*var last = ShardStoreState.GetLastHoveredIndex();
                if (!PopupState.GetVisible() || !ShardStoreState.HasItem(last)) return;
                
                var lastShardItem = ShardStoreState.GetItem(last);

                if (lastShardItem.shard._id_ == PopupState.GetTargetId())
                {
                    PopupState.Clear();
                }*/
            }
        }
    }
}