using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using td.features.eventBus;
using td.features.level;
using td.features.level.bus;
using td.features.shard.components;
using td.features.state;
using td.utils;

namespace td.features.shard.shardCollection.systems
{
    public class ShardCollection_Initialize_System : IProtoInitSystem, IProtoDestroySystem
    {
        [DI] private State state;
        [DI] private ShardCollection_State collState;
        [DI] private Level_State levelState;
        [DI] private EventBus events;
        [DI] private Shard_Calculator calc;
        [DI] private Shard_Service shardService;
        
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
            ref var cfg = ref levelState.GetLevelConfig();

            collState.Clear();
            
            if (cfg.IsEmpty()) return;
            
            collState.SetMaxShards(cfg.maxShards);
            
            for (var index = 0; index < cfg.startedShards.Length; index++)
            {
                var startedShard = cfg.startedShards[index];
                
                var shard = new Shard();
                shard.AutoReset(ref shard);
                shard.red = startedShard.red;
                shard.green = startedShard.green;
                shard.blue = startedShard.blue;
                shard.aquamarine = startedShard.aquamarine;
                shard.pink = startedShard.pink;
                shard.orange = startedShard.orange;
                shard.violet = startedShard.violet;
                shard.yellow = startedShard.yellow;
                shardService.PrecalcAllData(ref shard);
                
                collState.AddItem(ref shard);
            }
        }
    }
}