using System;
using Leopotam.EcsProto.QoL;
using td.features._common;
using td.features.enemy;
using td.features.level;
using td.features.movement;
using td.features.projectile;
using td.features.shard;
using td.utils.ecs;

namespace td.features.tower.systems
{
    public class TowerFindTargetSystem : ProtoIntervalableRunSystem
    {
        [DI] private Tower_Aspect aspect;
        [DI] private LevelMap levelMap;
        [DI] private Shard_Calculator shardCalculator;
        [DI] private Shard_Service shardService;
        [DI] private Projectile_Service projectileService;
        [DI] private Tower_Service towerService;
        [DI] private Enemy_Service enemyService;
        [DI] private Movement_Service movementService;
        
        public override void IntervalRun(float _)
        {
            foreach (var towerEntity in aspect.itTower)
            {
                var tower = aspect.towerPool.Get(towerEntity);
                var transform = movementService.GetTransform(towerEntity);

                var towerPosition = transform.position;

                var radius = tower.radius;
                
                var towerSqrRadius = radius * radius;
                
                var minDistanceToKernel = float.MaxValue;
                var targetEntity = -1;

                var enemiesInRadius = enemyService.FindNearestEnemies(towerPosition, towerSqrRadius);

                for (var idx = 0; idx < enemiesInRadius.Len(); idx++)
                {
                    var enemyEntity = enemiesInRadius.Get(idx);
                    var enemy = enemyService.GetEnemy(enemyEntity);
                    var enemyPosition = movementService.GetTransform(enemyEntity).position;

                    if (minDistanceToKernel < enemy.distanceToKernel) continue;
                    
                    minDistanceToKernel = enemy.distanceToKernel;
                    targetEntity = enemyEntity;
                }

                if (targetEntity >= 0)
                {
                    towerService.GetTowerTarget(towerEntity).targetEntity = aspect.World().PackEntityWithWorld(targetEntity);
                }
                else towerService.RemoveTowerTarget(towerEntity);
            }
        }

        public TowerFindTargetSystem(float interval, float timeShift, Func<float> getDeltaTime) : base(interval, timeShift, getDeltaTime)
        {
        }
    }
}