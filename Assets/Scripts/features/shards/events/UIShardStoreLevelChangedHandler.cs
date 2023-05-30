using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.components.flags;
using td.features.shards.commands;
using td.features.shards.config;
using td.features.shards.flags;
using td.utils.ecs;

namespace td.features.shards.events
{
    public class UIShardStoreLevelChangedHandler : IEcsRunSystem
    {
        [Inject] private ShardsConfig shardsConfig;
        [Inject] private ShardCalculator shardCalculator;
        [InjectWorld] private EcsWorld world;
        [InjectPool] private EcsPool<Shard> shardPool;
        [InjectPool] private EcsPool<ShardInStore> shardInStorePool;

        private EcsFilterInject<Inc<UIShardStoreLevelChangedOuterEvent>> entities = Constants.Worlds.Outer;

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in entities.Value)
            {
                var level = entities.Pools.Inc1.Get(entity).level;

                var shardFilter = world.Filter<Shard>().Inc<ShardInStore>().Exc<IsDisabled>().Exc<IsDestroyed>().End();

                foreach (var shardEntity in shardFilter)
                {
                    ref var shard = ref shardPool.Get(shardEntity);
                    ref var storeItem = ref shardInStorePool.Get(shardEntity);
                    
                    var shardAmountForLevel = shardsConfig.triangularPyramids[level - 1];
                    ShardUtils.ReduceToOne(ref shard);
                    ShardUtils.Multiple(ref shard, shardAmountForLevel);

                    storeItem.cost = shardCalculator.CalculateCost(ref shard, storeItem.costSingle);
                }

                systems.Outer<UIRefreshShardStoreOuterCommand>();
            }
            
            systems.CleanupOuter(entities);
        }
    }
}