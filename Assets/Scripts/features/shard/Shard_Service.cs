using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.features._common;
using td.features._common.components;
using td.features.level;
using td.features.shard.components;
using td.features.shard.mb;
using td.features.state;
using td.features.tower;
using td.monoBehaviours;
using td.utils.ecs;
using UnityEngine;
using NullReferenceException = System.NullReferenceException;
using NotImplementedException = System.NullReferenceException;

namespace td.features.shard
{
    public class Shard_Service
    {
        private readonly EcsInject<Shard_Pools> pools;
        private readonly EcsInject<ShardCalculator> calc;
        private readonly EcsInject<IState> state;
        private readonly EcsInject<LevelMap> levelMap;
        private readonly EcsInject<Common_Service> common;
        private readonly EcsInject<Tower_Service> towerService;
        private readonly EcsInject<Prefab_Service> prefabService;
        private readonly EcsInject<Shard_Converter> converter;
        private readonly EcsWorldInject world;

        public int SpawnShard(ref Shard sourceShard, Vector2 position, Transform parent)
        {
            var shardPosition = new Vector3(
                position.x,
                position.y,
                -0.01f
            );
            
            var prefab = prefabService.Value.GetPrefab(PrefabCategory.Shard, "Shard");
            var shardGO = Object.Instantiate(prefab, shardPosition, Quaternion.identity, parent);
            var entity = converter.Value.GetEntity(shardGO) ?? world.Value.NewEntity();
            converter.Value.Convert(shardGO, entity);

            var scale = shardGO.transform.localScale;

            shardGO.transform.localScale = new Vector2(scale.x, scale.y * Constants.UI.Shard.InTowerScaleY);

            ref var shard = ref GetShard(entity);
            ShardUtils.Copy(ref shard, ref sourceShard);
            
            var mb = GetShardMB(entity);
            mb.shardData = shard;
            mb.Refresh();

            return entity;
        }

        public bool HasShardInTower(EcsPackedEntity towerPackedEntity) => HasShardInTower(towerPackedEntity, out _);
        public bool HasShardInTower(EcsPackedEntity towerPackedEntity, out int shardEntity)
        {
            shardEntity = -1;
            return towerPackedEntity.Unpack(world.Value, out var towerEntity) && HasShardInTower(towerEntity, out shardEntity);
        }
        public bool HasShardInTower(EcsPackedEntityWithWorld towerPackedEntity) => HasShardInTower(towerPackedEntity, out _);
        public bool HasShardInTower(EcsPackedEntityWithWorld towerPackedEntity, out int shardEntity)
        {
            shardEntity = -1;
            return towerPackedEntity.Unpack(out var _, out var towerEntity) && HasShardInTower(towerEntity, out shardEntity);
        }
        public bool HasShardInTower(int towerEntity) => HasShardInTower(towerEntity, out var _);
        public bool HasShardInTower(int towerEntity, out int shardEntity)
        {
            shardEntity = -1;

            if (
                pools.Value.shardTowerWithShardPool.Value.Has(towerEntity) &&
                pools.Value.shardTowerWithShardPool.Value.Get(towerEntity).shardEntity.Unpack(out _, out shardEntity)
            ) {
                return true;
            }

            ref var transform = ref common.Value.GetTransform(towerEntity);

            if (!levelMap.Value.HasCell(transform.position, CellTypes.CanBuild)) return false;

            ref var cell = ref levelMap.Value.GetCell(transform.position, CellTypes.CanBuild);
            
            if (
                !cell.packedShardEntity.HasValue || 
                !towerService.Value.HasShardTower(cell.kernelNumber) ||
                !HasShard(cell.packedShardEntity.Value, out shardEntity)
            ) return false;

            return true;
        }

        public ref Shard GetShardInTower(int towerEntity, out int shardEntity)
        {
            if (!HasShardInTower(towerEntity, out shardEntity)) throw new NullReferenceException("Shard not found in Tower. Use HasShardInTower method for check");
            return ref GetShard(shardEntity);
        }

        public bool HasShard(int shardEntity) => pools.Value.shardPool.Value.Has(shardEntity);
        public bool HasShard(EcsPackedEntity packedEntity, out int shardEntity) => packedEntity.Unpack(world.Value, out shardEntity) && HasShard(shardEntity);
        public bool HasShard(EcsPackedEntityWithWorld packedEntity, out int shardEntity) => packedEntity.Unpack(out _, out shardEntity) && HasShard(shardEntity);

        public ref Shard GetShard(int shardEntity) => ref pools.Value.shardPool.Value.GetOrAdd(shardEntity);

        public ref Shard GetShard(EcsPackedEntity packedEntity, out int shardEntity) {
            if (!packedEntity.Unpack(world.Value, out shardEntity)) throw new NullReferenceException("Shard not found. Use HasShard method for check");
            return ref pools.Value.shardPool.Value.GetOrAdd(shardEntity);
        }
        public (CanCombineShardType check, uint cost) CheckCanCombineShards(ref Shard targetShard, ref Shard sourceShard)
        {
            var cost = calc.Value.CalculateCombineCost(ref targetShard, ref sourceShard);
            return state.Value.Energy >= cost 
                ? (CanCombineShardType.True, cost) 
                : (CanCombineShardType.FalseCost, cost);
        }
        
        public ref Ref<ShardMonoBehaviour> GetShardMBRef(int enemyEntity) => ref pools.Value.shardRefMBPool.Value.GetOrAdd(enemyEntity);
        public ShardMonoBehaviour GetShardMB(int enemyEntity) => GetShardMBRef(enemyEntity).reference!;

        public void PrecalcAllCosts(ref Shard shard)
        {
            shard.cost = 0;
            shard.costInsert = 0;
            shard.costRemove = 0;
            shard.costCombine = 0;
            shard.costDrop = 0;
            
            shard.cost = calc.Value.CalculateCost(ref shard);
            shard.costInsert = calc.Value.CalculateInsertCost(ref shard);
            shard.costRemove = calc.Value.CalculateRemoveCost(ref shard);
            shard.costCombine = calc.Value.CalculateCombineCost(ref shard);
            shard.costDrop = calc.Value.CalculateDropCost(ref shard);
            //
            // Debug.Log("shard = " + shard);
            // Debug.Log(".cost = " + shard.cost);
            // Debug.Log(".costInsert = " + shard.costInsert);
            // Debug.Log(".costRemove = " + shard.costRemove);
            // Debug.Log(".costCombine = " + shard.costCombine);
            // Debug.Log(".costDrop = " + shard.costDrop);
        }

        public bool InsertShardInTower(ref Shard shard, EcsPackedEntity towerPackedEntity)
        {
            if (!towerService.Value.HasTower(towerPackedEntity, out var towerEntity) && towerService.Value.HasShardTower(towerPackedEntity)) return false;
            
            var transform = common.Value.GetGOTransform(towerEntity);
            var position = (Vector2)transform.position;
            
            if (!levelMap.Value.HasCell(position, CellTypes.CanBuild)) return false;
            
            ref var cell = ref levelMap.Value.GetCell(transform.position, CellTypes.CanBuild);

            if (cell.IsEmpty) return false;
            if (cell.packedShardEntity.HasValue) return false;
            if (!cell.packedBuildingEntity.HasValue || !cell.packedBuildingEntity.Value.Unpack(world.Value, out var towerEntityInCell)) return false;
            
            if (towerEntity != towerEntityInCell) return false;
            
            //

            ref var tower =ref towerService.Value.GetTower(towerEntity);

            tower.radius = calc.Value.GetTowerRadius(ref shard);
            
            var shardEntity = SpawnShard(ref shard, position + tower.barrel, transform);

            cell.packedShardEntity = world.Value.PackEntity(shardEntity);
            
            pools.Value.shardTowerWithShardPool.Value.GetOrAdd(towerEntity).shardEntity = world.Value.PackEntityWithWorld(shardEntity);
            
            // add effects
            // add idle to use for shardTower
            
            return true;
        }

        public void DropToMap(ref Shard shard, Vector2 worldPosition)
        {
            throw new NotImplementedException();
        }
        
        public float GetRadiusByTower(int towerEntity)
        {
            if (!HasShardInTower(towerEntity, out var shardEntity)) return 0f;
            ref var shard = ref GetShard(shardEntity);
            return calc.Value.GetTowerRadius(ref shard);
        }

        public float GetRadius(int shardEntity)
        {
            ref var shard = ref GetShard(shardEntity);
            return calc.Value.GetTowerRadius(ref shard);
        }
        public float GetRadius(ref Shard shard)
        {
            return calc.Value.GetTowerRadius(ref shard);
        }
        public float GetRadius(Shard shard)
        {
            return calc.Value.GetTowerRadius(ref shard);
        }
    }
    
    public enum CanCombineShardType
    {
        FalseCost = -1,
        False = 0,
        True = 1,
    }
}