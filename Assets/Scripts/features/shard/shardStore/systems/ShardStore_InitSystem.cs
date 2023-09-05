using System.Collections.Generic;
using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using td.features.eventBus;
using td.features.level;
using td.features.level.bus;
using td.features.shard.components;
using td.features.state;

namespace td.features.shard.shardStore.systems
{
    public class ShardStore_InitSystem : IProtoInitSystem, IProtoDestroySystem
    {
        [DI] private State state;
        [DI] private Level_State levelState;
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
            state.Ex<ShardStore_State>().Clear();
            if (levelState.GetLevelConfig().IsEmpty()) return;
            
            ref var shardsStore = ref levelState.GetLevelConfig().shardsStore;
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
                // var price = calc.GetBasePriceByType(shardType);

                var shard = new Shard();
                ShardUtils.Clear(ref shard);
                ShardUtils.Set(ref shard, shardType, (byte)calc.GetQuantityForLevel(1));
                
                var storeItem = new ShardStore_Item
                {
                    shardType = shardType,
                    // price = price,
                    // basePrice = price,
                };

                state.Ex<ShardStore_State>().AddItem(ref storeItem);
            }
        }
    }
}