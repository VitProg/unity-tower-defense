using System;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.features._common;
using td.features._common.components;
using td.features.enemy.components;
using td.features.level;
using td.features.projectile;
using td.features.shard;
using td.features.tower.components;
using td.utils.ecs;
using UnityEngine;

namespace td.features.tower.systems
{
    public class TowerFindTargetSystem : EcsIntervalableRunSystem
    {
        private readonly EcsInject<LevelMap> levelMap;
        private readonly EcsInject<ShardCalculator> shardCalculator;
        private readonly EcsInject<Shard_Service> shardService;
        private readonly EcsInject<Projectile_Service> projectileService;
        private readonly EcsInject<Tower_Service> towerService;
        
        private readonly EcsFilterInject<Inc<Tower, Ref<GameObject>>, ExcludeNotAlive> towerEntities = default;
        private readonly EcsFilterInject<Inc<Enemy, Ref<GameObject>>, ExcludeNotAlive> enemyEntities = default;

        public override void IntervalRun(IEcsSystems systems, float _)
        {
            var world = systems.GetWorld();
            
            foreach (var towerEntity in towerEntities.Value)
            {
                var tower = towerEntities.Pools.Inc1.Get(towerEntity);
                var towerGameObject = towerEntities.Pools.Inc2.Get(towerEntity);

                var towerPosition = towerGameObject.reference.transform.position;

                var radius = tower.radius;
                
                // if (shardService.Value.HasShardInTower(towerEntity, out var shardEntity))
                // {
                    // ref var shard = ref shardService.Value.GetShard(shardEntity);
                    // yellow - увеличивает радиус стрельбы
                    // radius = shardCalculator.Value.GetTowerRadius(ref shard);
                // }
                
                var towerSqrRadius = radius * radius;
                
                // var maxDistanceFromSpawn = 0f;
                var minDistanceToKernel = float.MaxValue;
                var targetEntity = -1;

                foreach (var enemyEntity in enemyEntities.Value)
                {
                    var enemy = enemyEntities.Pools.Inc1.Get(enemyEntity);
                    var enemyGameObject = enemyEntities.Pools.Inc2.Get(enemyEntity);
                    var enemyPosition = enemyGameObject.reference.transform.position;

                    if (
                        Math.Abs(enemyPosition.x - towerPosition.x) > tower.radius ||
                        Math.Abs(enemyPosition.y - towerPosition.y) > tower.radius
                    )
                    {
                        continue;
                    }

                    if ((enemyPosition - towerPosition).sqrMagnitude < towerSqrRadius)
                    {
                        if (minDistanceToKernel > enemy.distanceToKernel)
                        {
                            minDistanceToKernel = enemy.distanceToKernel;
                            targetEntity = enemyEntity;
                        }
                        // var distanceFromSpawn = float.MaxValue; 
                        //
                        // //todo select method by tower settings
                        // var enemyCoordinate = HexGridUtils.PositionToCell(enemyPosition);
                        // var cell = levelMap.GetCell(enemyCoordinate, CellTypes.CanWalk);
                        // if (cell && cell.isSpawn && enemy.distanceFromSpawn > 0)
                        // {
                        //     distanceFromSpawn = enemy.distanceFromSpawn;
                        // }
                        //
                        // if (maxDistanceFromSpawn < distanceFromSpawn)
                        // {
                        //     maxDistanceFromSpawn = distanceFromSpawn;
                        //     targetEntity = enemyEntity;
                        // }
                    }
                }

                if (targetEntity >= 0) towerService.Value.GetTowerTarget(towerEntity).targetEntity = world.PackEntity(targetEntity);
                else towerService.Value.RemoveTowerTarget(towerEntity);
            }
        }

        public TowerFindTargetSystem(float interval, float timeShift, Func<float> getDeltaTime) : base(interval, timeShift, getDeltaTime)
        {
        }
    }
}