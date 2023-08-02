using Leopotam.EcsLite;
using td.features.level;
using td.features.level.bus;
using td.features.shard;
using td.features.state;

namespace td.features.shardCollection
{
    public class InitializeShardCollectionSystem : IEcsInitSystem, IEcsDestroySystem
    {
        private readonly EcsInject<IState> state;
        private readonly EcsInject<LevelMap> levelMap;
        private readonly EcsInject<IEventBus> events;
        private readonly EcsInject<ShardCalculator> calc;
        private readonly EcsInject<Shard_Service> shardService;
        
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
            state.Value.ShardCollection.Clear();
            if (levelMap.Value.LevelConfig == null) return;
            
            var started = levelMap.Value.LevelConfig.Value.startedShards;
            for (var index = 0; index < started.Length; index++)
            {
                var shard = started[index];
                shardService.Value.PrecalcAllCosts(ref shard);
                state.Value.ShardCollection.AddItem(ref shard);
            }
        }
    }
}