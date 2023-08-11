using System.Collections.Generic;
using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using td.features.eventBus;
using td.features.level;
using td.features.level.bus;
using td.features.shard.components;
using td.features.state;

namespace td.features.shard.shardStore
{
    public class InitializeShardStoreSystem : IProtoInitSystem, IProtoDestroySystem
    {
        [DI] private State state;
        [DI] private LevelMap levelMap;
        [DI] private Shard_Calculator calc;
        [DI] private EventBus events;
        
        public void Init(IProtoSystems systems)
        {
            events.unique.ListenTo<Event_LevelLoaded>(OnEvent);
        }

        public void Destroy()
        {
            events.unique.RemoveListener<Event_LevelLoaded>(OnEvent);
        }

        // --------------------------------------------- //

        private void OnEvent(ref Event_LevelLoaded item)
        {
            state.ShardStore.Clear();
            if (levelMap.LevelConfig == null) return;
            
            var shardsStore = levelMap.LevelConfig.Value.shardsStore;
            // var shardsCost = levelMap.Value.LevelConfig.Value.shardsCost;

            var toSore = new List<ShardTypes>();
            
            if (shardsStore.red) toSore.Add(ShardTypes.Red); 
            if (shardsStore.green) toSore.Add(ShardTypes.Green); 
            if (shardsStore.blue) toSore.Add(ShardTypes.Blue); 
            if (shardsStore.yellow) toSore.Add(ShardTypes.Yellow); 
            if (shardsStore.orange) toSore.Add(ShardTypes.Orange); 
            if (shardsStore.pink) toSore.Add(ShardTypes.Pink); 
            if (shardsStore.violet) toSore.Add(ShardTypes.Violet); 
            if (shardsStore.aquamarine) toSore.Add(ShardTypes.Aquamarine);

            foreach (var shardType in toSore)
            {
                var cost = calc.GetBaseCostByType(shardType);

                var storeItem = new ShardStore_Item
                {
                    shardType = shardType,
                    cost = cost,
                };

                state.ShardStore.AddItem(ref storeItem);
            }
        }
    }
}