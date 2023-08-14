using System;
using Leopotam.EcsProto.QoL;
using td.features._common;
using td.features._common.components;
using td.features.level;
using td.features.level.cells;
using td.features.movement;
using td.features.prefab;
using td.features.shard.components;
using td.features.shard.mb;
using td.features.state;
using td.features.tower;
using td.monoBehaviours;
using td.utils.ecs;
using UnityEngine;
using NullReferenceException = System.NullReferenceException;
using Object = UnityEngine.Object;

namespace td.features.shard
{
    public class Shard_Service
    {
        [DI] private Shard_Aspect aspect;
        [DI] private Shard_Calculator calc;
        [DI] private State state;
        [DI] private LevelMap levelMap;
        [DI] private Movement_Service movementService;
        [DI] private Tower_Service towerService;
        [DI] private Prefab_Service prefabService;
        [DI] private Shard_Converter converter;

        public int SpawnShard(ref Shard sourceShard, Vector2 position, Transform parent)
        {
            var shardPosition = new Vector3(
                position.x,
                position.y,
                -0.01f
            );
            
            var prefab = prefabService.GetPrefab(PrefabCategory.Shard, "Shard");
            var shardGO = Object.Instantiate(prefab, shardPosition, Quaternion.identity, parent);
            var entity = converter.GetEntity(shardGO) ?? aspect.World().NewEntity();
            converter.Convert(shardGO, entity);

            var scale = shardGO.transform.localScale;

            shardGO.transform.localScale = new Vector2(scale.x, scale.y * Constants.UI.Shard.InTowerScaleY);

            ref var shard = ref GetShard(entity);
            ShardUtils.Copy(ref shard, ref sourceShard);
            
            var mb = GetShardMB(entity);
            mb.shardData = shard;
            mb.Refresh();

            return entity;
        }

        public bool HasShardInTower(ProtoPackedEntity towerPackedEntity) => HasShardInTower(towerPackedEntity, out _);
        public bool HasShardInTower(ProtoPackedEntity towerPackedEntity, out int shardEntity)
        {
            shardEntity = -1;
            return towerPackedEntity.Unpack(aspect.World(), out var towerEntity) && HasShardInTower(towerEntity, out shardEntity);
        }
        public bool HasShardInTower(ProtoPackedEntityWithWorld towerPackedEntity) => HasShardInTower(towerPackedEntity, out _);
        public bool HasShardInTower(ProtoPackedEntityWithWorld towerPackedEntity, out int shardEntity)
        {
            shardEntity = -1;
            return towerPackedEntity.Unpack(out var _, out var towerEntity) && HasShardInTower(towerEntity, out shardEntity);
        }
        public bool HasShardInTower(int towerEntity) => HasShardInTower(towerEntity, out var _);
        public bool HasShardInTower(int towerEntity, out int shardEntity)
        {
            shardEntity = -1;

            if (
                aspect.shardTowerWithShardPool.Has(towerEntity) &&
                aspect.shardTowerWithShardPool.Get(towerEntity).shardEntity.Unpack(out _, out shardEntity)
            ) {
                return true;
            }

            ref var transform = ref movementService.GetTransform(towerEntity);

            if (!levelMap.HasCell(transform.position, CellTypes.CanBuild)) return false;

            ref var cell = ref levelMap.GetCell(transform.position, CellTypes.CanBuild);
            
            if (
                !cell.packedShardEntity.HasValue || 
                !towerService.HasShardTower(cell.kernelNumber) ||
                !HasShard(cell.packedShardEntity.Value, out shardEntity)
            ) return false;

            return true;
        }

        public ref Shard GetShardInTower(int towerEntity, out int shardEntity)
        {
            if (!HasShardInTower(towerEntity, out shardEntity)) throw new NullReferenceException("Shard not found in Tower. Use HasShardInTower method for check");
            return ref GetShard(shardEntity);
        }

        public bool HasShard(int shardEntity) => aspect.shardPool.Has(shardEntity);
        public bool HasShard(ProtoPackedEntity packedEntity, out int shardEntity) => packedEntity.Unpack(aspect.World(), out shardEntity) && HasShard(shardEntity);
        public bool HasShard(ProtoPackedEntityWithWorld packedEntity, out int shardEntity) => packedEntity.Unpack(out _, out shardEntity) && HasShard(shardEntity);

        public ref Shard GetShard(int shardEntity) => ref aspect.shardPool.GetOrAdd(shardEntity);

        public ref Shard GetShard(ProtoPackedEntityWithWorld packedEntity, out int shardEntity) {
            if (!packedEntity.Unpack(out var w, out shardEntity)) throw new NullReferenceException("Shard not found. Use HasShard method for check");
            if (!aspect.World().Equals(w)) throw new Exception("Shards not equal.");
            return ref aspect.shardPool.GetOrAdd(shardEntity);
        }
        public ref Shard GetShard(ProtoPackedEntity packedEntity, out int shardEntity) {
            if (!packedEntity.Unpack(aspect.World(), out shardEntity)) throw new NullReferenceException("Shard not found. Use HasShard method for check");
            return ref aspect.shardPool.GetOrAdd(shardEntity);
        }
        public (CanCombineShardType check, uint cost) CheckCanCombineShards(ref Shard targetShard, ref Shard sourceShard)
        {
            var cost = calc.CalculateCombineCost(ref targetShard, ref sourceShard);
            return state.GetEnergy() >= cost 
                ? (CanCombineShardType.True, cost) 
                : (CanCombineShardType.FalseCost, cost);
        }
        
        public ref Ref<ShardMonoBehaviour> GetShardMBRef(int enemyEntity) => ref aspect.shardRefMBPool.GetOrAdd(enemyEntity);
        public ShardMonoBehaviour GetShardMB(int enemyEntity) => GetShardMBRef(enemyEntity).reference!;

        public void PrecalcAllCosts(ref Shard shard)
        {
            shard.cost = 0;
            shard.costInsert = 0;
            shard.costRemove = 0;
            shard.costCombine = 0;
            shard.costDrop = 0;
            
            shard.cost = calc.CalculateCost(ref shard);
            shard.costInsert = calc.CalculateInsertCost(ref shard);
            shard.costRemove = calc.CalculateRemoveCost(ref shard);
            shard.costCombine = calc.CalculateCombineCost(ref shard);
            shard.costDrop = calc.CalculateDropCost(ref shard);
            //
            // Debug.Log("shard = " + shard);
            // Debug.Log(".cost = " + shard.cost);
            // Debug.Log(".costInsert = " + shard.costInsert);
            // Debug.Log(".costRemove = " + shard.costRemove);
            // Debug.Log(".costCombine = " + shard.costCombine);
            // Debug.Log(".costDrop = " + shard.costDrop);
        }

        public bool InsertShardInTower(ref Shard shard, ProtoPackedEntityWithWorld towerPackedEntity)
        {
            if (!towerService.HasTower(towerPackedEntity, out var towerEntity) && towerService.HasShardTower(towerPackedEntity)) return false;
            
            var transform = movementService.GetGOTransform(towerEntity);
            var position = (Vector2)transform.position;
            
            if (!levelMap.HasCell(position, CellTypes.CanBuild)) return false;
            
            ref var cell = ref levelMap.GetCell(transform.position, CellTypes.CanBuild);

            if (cell.IsEmpty) return false;
            if (cell.packedShardEntity.HasValue) return false;
            if (!cell.packedBuildingEntity.HasValue || !cell.packedBuildingEntity.Value.Unpack(out _, out var towerEntityInCell)) return false;
            
            if (towerEntity != towerEntityInCell) return false;
            
            //

            ref var tower =ref towerService.GetTower(towerEntity);

            tower.radius = calc.GetTowerRadius(ref shard);
            
            var shardEntity = SpawnShard(ref shard, position + tower.barrel, transform);

            cell.packedShardEntity = aspect.World().PackEntityWithWorld(shardEntity);
            
            aspect.shardTowerWithShardPool.GetOrAdd(towerEntity).shardEntity = aspect.World().PackEntityWithWorld(shardEntity);
            
            // add effects
            // add idle to use for shardTower
            
            return true;
        }

        public void DropToMap(ref Shard shard, Vector2 worldPosition)
        {
            throw new NullReferenceException();
        }
        
        public float GetRadiusByTower(int towerEntity)
        {
            if (!HasShardInTower(towerEntity, out var shardEntity)) return 0f;
            ref var shard = ref GetShard(shardEntity);
            return calc.GetTowerRadius(ref shard);
        }

        public float GetRadius(int shardEntity)
        {
            ref var shard = ref GetShard(shardEntity);
            return calc.GetTowerRadius(ref shard);
        }
        public float GetRadius(ref Shard shard)
        {
            return calc.GetTowerRadius(ref shard);
        }
        public float GetRadius(Shard shard)
        {
            return calc.GetTowerRadius(ref shard);
        }
    }
    
    public enum CanCombineShardType
    {
        FalseCost = -1,
        False = 0,
        True = 1,
    }
}