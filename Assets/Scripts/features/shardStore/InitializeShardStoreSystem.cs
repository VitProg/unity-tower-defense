using System.Collections.Generic;
using Leopotam.EcsLite;
using td.features.level;
using td.features.level.bus;
using td.features.shard;
using td.features.shard.components;
using td.features.state;

namespace td.features.shardStore
{
    public class InitializeShardStoreSystem : IEcsInitSystem, IEcsDestroySystem
    {
        private readonly EcsInject<IState> state;
        private readonly EcsInject<LevelMap> levelMap;
        private readonly EcsInject<ShardCalculator> calc;
        private readonly EcsInject<IEventBus> events;
        
        public void Init(IEcsSystems systems)
        {
            events.Value.Unique.ListenTo<Event_LevelLoaded>(OnEvent);
        }

        public void Destroy(IEcsSystems systems)
        {
            events.Value.Unique.RemoveListener<Event_LevelLoaded>(OnEvent);
        }

        // --------------------------------------------- //

        private void OnEvent(ref Event_LevelLoaded item)
        {
            state.Value.ShardStore.Clear();
            if (levelMap.Value.LevelConfig == null) return;
            
            var shardsStore = levelMap.Value.LevelConfig.Value.shardsStore;
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
                var cost = calc.Value.GetBaseCostByType(shardType);

                var storeItem = new ShardStore_Item
                {
                    shardType = shardType,
                    cost = cost,
                };

                state.Value.ShardStore.AddItem(ref storeItem);
            }
        }
    }
}