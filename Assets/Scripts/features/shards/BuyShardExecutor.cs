using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.components.flags;
using td.features.shards.commands;
using td.features.shards.flags;
using td.features.state;
using td.utils.ecs;
using UnityEngine;

namespace td.features.shards
{
    public class BuyShardExecutor : IEcsRunSystem
    {
        [Inject] private State state;
        [InjectWorld] private EcsWorld world;
        
        private readonly EcsFilterInject<Inc<BuyShardCommand>> entities = Constants.Worlds.Outer;
        private readonly EcsFilterInject<Inc<Shard, ShardInCollection, IsDisabled>, Exc<IsDestroyed>> freeShardInCollectionEntities = default;

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in entities.Value)
            {
                ref var command = ref entities.Pools.Inc1.Get(entity);

                if (!command.shardPackedEntity.Unpack(world, out var sourceShardEntity)) continue;
                
                ref var sourceShard = ref world.GetComponent<Shard>(sourceShardEntity);

                var shardArray = freeShardInCollectionEntities.Value.ToArray();
                if (shardArray.Length <= 0) continue;
                
                var freeShardInCollectionEntity = Mathf.Min(shardArray);

                //todo spend money and check balance
                state.Money -= command.cost;

                ref var shard = ref world.GetComponent<Shard>(freeShardInCollectionEntity);
                ShardUtils.Copy(ref shard, ref sourceShard);

                world.DelComponent<IsDisabled>(freeShardInCollectionEntity);
                world.DelComponent<ShardColor>(freeShardInCollectionEntity);

                systems.OuterSingle<UIRefreshShardCollectionOuterCommand>();
                systems.OuterSingle<UIHideShardStoreOuterCommand>();
            }

            if (entities.Value.GetEntitiesCount() > 0)
            {
                systems.CleanupOuter(entities);
            }
        }
    }
}