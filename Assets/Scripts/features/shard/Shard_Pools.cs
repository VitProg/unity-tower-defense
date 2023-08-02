using Leopotam.EcsLite.Di;
using td.features._common.components;
using td.features.shard.components;
using td.features.shard.flags;
using td.features.shard.mb;
using td.features.tower.components;

namespace td.features.shard
{
    public class Shard_Pools
    {
        // public readonly EcsPoolInject<ShardInCollection> shardInCollectionPool = default;
        // public readonly EcsPoolInject<ShardInStore> shardInStorePool = default;
        // public readonly EcsPoolInject<ShardInTower> shardInTowerPool = default;
        // public readonly EcsPoolInject<ShardIsHovered> shardIsHoveredPool = default;
        
        public readonly EcsPoolInject<Shard> shardPool = default;
        public readonly EcsPoolInject<Ref<ShardMonoBehaviour>> shardRefMBPool = default;
        public readonly EcsPoolInject<ShardTowerWithShard> shardTowerWithShardPool = default;
        
        // public readonly EcsPoolInject<ShardTower> shardTowerPool = default;

        // public readonly EcsFilterInject<Inc<Shard, ShardInStore, Ref<GameObject>>, ExcludeNotAlive> shardInStoreFilter = default;
        // public readonly EcsFilterInject<Inc<Shard, ShardInCollection, Ref<GameObject>>, ExcludeNotAlive> shardInCollectionFilter = default;
        // public readonly EcsFilterInject<Inc<Shard, ShardInCollection, Ref<GameObject>, IsDisabled>, Exc<IsDestroyed>> disabledShardInCollectionFilter = default;
        // public readonly EcsFilterInject<Inc<Shard, ShardInCollection, Ref<GameObject>>, Exc<IsDestroyed>> shardInCollectionWithDisabledFilter = default;

        // private readonly EcsWorldInject world;

        // [CanBeNull] private EcsFilter _hoveredShardFilter;
        // public EcsFilter hoveredShardFilter =>
        //     _hoveredShardFilter ??= world.Value.Filter<Shard>()
        //         .Inc<ShardInCollection>()
        //         .Inc<ShardIsHovered>()
        //         .Exc<ShardInStore>()
        //         .Exc<IsHidden>()
        //         .Exc<IsDisabled>()
        //         .Exc<IsDestroyed>()
        //         .Exc<DraggingStartedData>()
        //         .End(3);
    }
}