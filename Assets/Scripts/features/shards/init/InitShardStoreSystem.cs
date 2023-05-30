using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.common;
using td.components.commands;
using td.components.flags;
using td.components.refs;
using td.features.levels;
using td.features.shards.commands;
using td.features.shards.flags;
using td.services;
using td.utils.ecs;
using UnityEngine;

namespace td.features.shards.init
{
    public class InitShardStoreSystem : IEcsRunSystem
    {
        [Inject] private LevelMap levelMap;
        [InjectWorld] private EcsWorld world;
        
        private readonly EcsFilterInject<Inc<LevelLoadedOuterEvent>> loadedEventEntities = Constants.Worlds.Outer;
        private readonly EcsFilterInject<Inc<Shard, ShardInStore>, Exc<IsDestroyed, IsDisabled>> shardEntities = default;
        
        public void Run(IEcsSystems systems)
        {
            foreach (var entity in loadedEventEntities.Value)
            {
                InitShardStore();
                systems.OuterSingle<UIRefreshShardStoreOuterCommand>();
                
                break;
            }
        }
        
        private void InitShardStore()
        {
            foreach (var shardEntity in shardEntities.Value)
            {
                world.DelComponent<ShardInStore>(shardEntity);
                world.SafeDelEntity(shardEntity);
            }

            // add new

            if (levelMap.LevelConfig != null)   
            {
                for (var index = 0; index < levelMap.LevelConfig.Value.availableShards.Length; index++)
                {
                    var shardType = levelMap.LevelConfig.Value.availableShards[index];
                    var cost = levelMap.LevelConfig.Value.shardsCost.Length > index
                        ? levelMap.LevelConfig.Value.shardsCost[index]
                        : (ushort)10;

                    var shardEntity = world.NewEntity();
                    
                    ref var shard = ref world.GetComponent<Shard>(shardEntity);
                    ShardUtils.Clear(ref shard);
                    ShardUtils.Set(ref shard, shardType, 1);

                    ref var storeItem = ref world.GetComponent<ShardInStore>(shardEntity);
                    storeItem.cost = cost;
                    storeItem.costSingle = cost;
                }
            }
        }
    }
}