using System;
using Leopotam.EcsLite;
using td.features.shards;
using td.utils.ecs;

namespace td.features.towers
{
    public static class ShardTowerUtils
    {
        // public static bool TryGetShard(EcsWorld world, ref ShardTower shardTower, out int shardEntity, out Shard shard)
        // {
        //     shard = default;
        //
        //     if (
        //         !shardTower.shardPackedEntity.Unpack(world, out shardEntity) ||
        //         !world.HasComponent<Shard>(shardEntity)
        //     )
        //     {
        //         return false;
        //     }
        //
        //     shard = ref world.GetComponent<Shard>(shardEntity);
        //
        //     return true;
        // }

        public static ref Shard GetShard(EcsWorld world, ref ShardTower shardTower, out int shardEntity)
        {
            if (HasShard(world, ref shardTower) == false)
                throw new NullReferenceException(
                    "Shard not found in ShardTower. You need to check with HasShard() before call GetShard()");

            shardTower.shardPackedEntity.Unpack(world, out shardEntity);

            return ref world.GetComponent<Shard>(shardEntity);
        }

        public static bool HasShard(EcsWorld world, ref ShardTower shardTower) =>
            shardTower.shardPackedEntity.Unpack(world, out var shardEntity) &&
            world.HasComponent<Shard>(shardEntity);

        public static bool HasShard(EcsWorld world, int shardTowerEntity)
        {
            if (!world.HasComponent<ShardTower>(shardTowerEntity)) return false;
            ref var shardTower = ref world.GetComponent<ShardTower>(shardTowerEntity);

            return HasShard(world, ref shardTower);
        }
    }
}