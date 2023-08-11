using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using td.features.eventBus;
using td.features.level;
using td.features.level.bus;
using td.features.state;

namespace td.features.shard.shardCollection
{
    public class ShardCollection_Initialize_System : IProtoInitSystem, IProtoDestroySystem
    {
        [DI] private State state;
        [DI] private LevelMap levelMap;
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
            state.ShardCollection.Clear();
            if (levelMap.LevelConfig == null) return;
            
            var started = levelMap.LevelConfig.Value.startedShards;
            for (var index = 0; index < started.Length; index++)
            {
                var shard = started[index];
                shardService.PrecalcAllCosts(ref shard);
                state.ShardCollection.AddItem(ref shard);
            }
        }
    }
}