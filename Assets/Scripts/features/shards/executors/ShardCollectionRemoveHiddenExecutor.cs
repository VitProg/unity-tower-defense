using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.components.flags;
using td.components.refs;
using td.features.shards.commands;
using td.features.shards.flags;
using td.features.shards.mb;
using td.utils.ecs;
using UnityEngine;

namespace td.features.shards.executors
{
    public class ShardCollectionRemoveHiddenExecutor : IEcsRunSystem
    {
        [InjectWorld] private EcsWorld world;
        
        private readonly EcsFilterInject<Inc<ShardCollectionRemoveHiddenOuterCommand>> entities = Constants.Worlds.Outer;
        private readonly EcsFilterInject<Inc<Shard, ShardInCollection, Ref<GameObject>>, Exc<IsDestroyed>> shardEntities = default;


        public void Run(IEcsSystems systems)
        {
            foreach (var entity in entities.Value)
            {
                var shards = shardEntities.Value.ToArray();
                var length = shards.Length;

                var isMoveLeft = false;
                
                var plusShowed = false;

                // move all shards ui left, start at hidden shard ui
                for (var index = 0; index < length; index++)
                {
                    var shardEntity = shards[index];
                    var isHidden = world.HasComponent<IsHidden>(shardEntity);

                    if (isHidden || isMoveLeft)
                    {
                        isMoveLeft = true;
                        
                        ref var shard = ref shardEntities.Pools.Inc1.Get(shardEntity);
                        var shardGO = shardEntities.Pools.Inc3.Get(shardEntity).reference;
                        var shardUIButton = shardGO.GetComponentInParent<ShardUIButton>();
                        
                        world.DelComponent<ShardColor>(shardEntity);

                        world.DelComponent<IsHidden>(shardEntity);
                        
                        if (index + 1 < length)
                        {
                            var nextShardEntity = shards[index + 1];
                            ref var nextShard = ref shardEntities.Pools.Inc1.Get(nextShardEntity);
                            var nextShardGO = shardEntities.Pools.Inc3.Get(nextShardEntity).reference;
                            var nextShardUIButton = nextShardGO.GetComponentInParent<ShardUIButton>();

                            ShardUtils.Copy(ref shard, ref nextShard);
                            shardUIButton.showPlus = nextShardUIButton.showPlus;
                            shardUIButton.hasShard = nextShardUIButton.hasShard;

                            if (shardUIButton.hasShard)
                            {
                                world.DelComponent<IsDisabled>(shardEntity);
                            }
                            else
                            {
                                world.GetComponent<IsDisabled>(shardEntity);
                            }

                            if (shardUIButton.showPlus) plusShowed = true;
                        }
                        else
                        {
                            shardUIButton.hasShard = false;
                            shardUIButton.showPlus = !plusShowed;
                        }

                        shardUIButton.Refresh();
                    }
                }

                systems.OuterSingle<UIRefreshShardCollectionOuterCommand>();
                systems.OuterSingle<UIHideShardStoreOuterCommand>();

                break;
            }
            
            systems.CleanupOuter(entities);
        }
    }
}