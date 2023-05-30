using System.Collections.Generic;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.common;
using td.components.flags;
using td.components.refs;
using td.features.shards.commands;
using td.features.shards.flags;
using td.features.shards.mb;
using td.services;
using td.services.ecsConverter;
using td.utils.ecs;
using UnityEngine;

namespace td.features.shards.executors
{
    public class UIRefreshShardCollectionExecutor : IEcsRunSystem
    {
        [Inject] private LevelMap levelMap;
        [Inject] private EntityConverters converters;
        [InjectWorld] private EcsWorld world;
        [InjectShared] private SharedData shared;
        [InjectSystems] private IEcsSystems systems;
        
        private readonly EcsFilterInject<Inc<UIRefreshShardCollectionOuterCommand>> eventEntities = Constants.Worlds.Outer;
        private readonly EcsFilterInject<Inc<Shard, ShardInCollection, Ref<GameObject>>, Exc<IsDestroyed>> shardEntities = default;
        
        public void Run(IEcsSystems unused)
        {
            foreach (var eventEntity in eventEntities.Value)
            {
                if (shared.shardCollection != null)
                {
                    Refresh();
                }

                break;
            }
            systems.CleanupOuter(eventEntities);
        }

        private void Refresh()
        {
            var plusShowed = false;
            foreach (var shardEntity in shardEntities.Value)
            {
                ref var shardGO = ref shardEntities.Pools.Inc3.Get(shardEntity);
                var shardUiButton = shardGO.reference.transform.GetComponentInParent<ShardUIButton>();
                
                var hasShard = !world.HasComponent<IsDisabled>(shardEntity);
                
                shardUiButton.druggable = true;
                shardUiButton.hasShard = hasShard;
                shardUiButton.showPlus = !hasShard && !plusShowed;
                shardUiButton.cost = 0;

                if (shardUiButton.showPlus) plusShowed = true;

                shardUiButton.Refresh();
            }
        }
    }
}