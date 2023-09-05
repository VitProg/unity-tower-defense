using System;
using JetBrains.Annotations;
using Leopotam.EcsProto.QoL;
using td.features._common.components;
using td.features.level;
using td.features.level.cells;
using td.features.movement;
using td.features.prefab;
using td.features.shard.components;
using td.features.shard.mb;
using td.features.state;
using td.utils;
using td.utils.ecs;
using UnityEngine;
using Object = UnityEngine.Object;

namespace td.features.shard
{
    public class Shard_Service
    {
        [DI] private Shard_Aspect aspect;
        [DI] private Shard_Calculator calc;
        [DI] private State state;
        [DI] private Movement_Service movementService;
        [DI] private Prefab_Service prefabService;
        [DI] private Shard_Converter converter;

        public int SpawnShard(ref Shard sourceShard, Vector2 position, [CanBeNull] Transform parent = null)
        {
            var shardPosition = new Vector3(
                position.x,
                position.y,
                -0.01f
            );
            
            var prefab = prefabService.GetPrefab(PrefabCategory.Shard, "shard");
            var shardGO = Object.Instantiate(prefab, shardPosition, Quaternion.identity);
            if (parent) shardGO.transform.SetParent(parent);
            var entity = converter.GetEntity(shardGO) ?? aspect.World().NewEntity();
            converter.Convert(shardGO, entity);

            var scale = shardGO.transform.localScale;

            shardGO.transform.localScale = new Vector2(scale.x, scale.y * Constants.UI.Shard.InTowerScaleY);

            ref var shard = ref GetShard(entity);
            ShardUtils.Copy(ref shard, ref sourceShard);
            
            var mb = GetShardMB(entity);
            mb.shard = shard;
            PrecalcAllData(ref shard);
            mb.FullRefresh();

            return entity;
        }

        public bool HasShard(int shardEntity) => aspect.shardPool.Has(shardEntity);
        public bool HasShard(ProtoPackedEntityWithWorld packedEntity, out int shardEntity) => packedEntity.Unpack(out _, out shardEntity) && HasShard(shardEntity);

        public ref Shard GetShard(int shardEntity) => ref aspect.shardPool.GetOrAdd(shardEntity);
        public ref Shard GetShard(ProtoPackedEntityWithWorld packedEntity, out int shardEntity) {
            var check = packedEntity.Unpack(out var w, out shardEntity);
#if UNITY_EDITOR
            if (!check) throw new NullReferenceException("Shard not found. Use HasShard method for check");
            if (!aspect.World().Equals(w)) throw new Exception("Shards not equal.");
#endif
            return ref aspect.shardPool.GetOrAdd(shardEntity);
        }
        public ref Shard GetShard(ProtoPackedEntity packedEntity, out int shardEntity) {
            var check = packedEntity.Unpack(aspect.World(), out shardEntity);
#if UNITY_EDITOR
            if (!check) throw new NullReferenceException("Shard not found. Use HasShard method for check");
#endif
            return ref aspect.shardPool.GetOrAdd(shardEntity);
        }
        
        public ref Ref<UI_Shard> GetShardMBRef(int enemyEntity) => ref aspect.shardRefMBPool.GetOrAdd(enemyEntity);
        public UI_Shard GetShardMB(int enemyEntity) => GetShardMBRef(enemyEntity).reference!;

        public void PrecalcAllData(ref Shard shard)
        {
            shard.price = 0;
            shard.priceInsert = 0;
            shard.priceRemove = 0;
            shard.priceCombine = 0;
            shard.priceDrop = 0;

            shard.timeInsert = 0;
            shard.timeRemove = 0;
            shard.timeCombine = 0;
            
            shard.price = calc.CalculatePrice(ref shard);
            shard.priceInsert = calc.CalculateInsertPrice(ref shard);
            shard.priceRemove = calc.CalculateRemovePrice(ref shard);
            shard.priceCombine = calc.CalculateCombinePrice(ref shard);
            shard.priceDrop = calc.CalculateDropPrice(ref shard);
            
            shard.timeInsert = calc.CalculateInsertTime(ref shard);
            shard.timeRemove = calc.CalculateRemoveTime(ref shard);
            shard.timeCombine = calc.CalculateCombineTime(ref shard);

            shard.level = calc.GetShardLevel(ref shard);
            
            shard.radius = calc.GetRadius(ref shard);
            shard.fireRate = calc.GetFireRate(ref shard);
            shard.fireCountdown = 1f / shard.fireRate;
            shard.projectileSpeed = calc.GetProjectileSpeed(ref shard);
            
#if UNITY_EDITOR
            shard.currentColor.r = .5f;
            shard.currentColor.g = .5f;
            shard.currentColor.b = .5f;
            shard.currentColor.a = 1f;
#else
            shard.currentColor.r = 1f;
            shard.currentColor.g = 1f;
            shard.currentColor.b = 1f;
            shard.currentColor.a = 0f;
#endif
        }
    }
}