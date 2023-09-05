using System;
using Leopotam.EcsProto.QoL;
using td.features.enemy;
using td.features.level;
using td.features.movement;
using td.features.projectile;
using td.features.shard;
using td.utils.ecs;
using UnityEngine;

namespace td.features.tower.systems
{
    public class Tower_FindTarget_System : ProtoIntervalableRunSystem
    {
        [DI] private Tower_Aspect aspect;
        [DI] private Tower_Service towerService;
        [DI] private Enemy_Service enemyService;
        [DI] private Movement_Service movementService;
        
        public override void IntervalRun(float _)
        {
            foreach (var towerEntity in aspect.itShardTower)
            {
                ref var shardTower = ref aspect.shardTowerPool.Get(towerEntity);
                if (shardTower.radius < 0.001f) continue;
                ref var transform = ref movementService.GetTransform(towerEntity);

                var minDistanceToKernel = float.MaxValue;
                var targetEntity = -1;

                var enemiesInRadius = enemyService.FindNearestEnemies(transform.position, shardTower.sqrRadius);

                for (var idx = 0; idx < enemiesInRadius.Len(); idx++)
                {
                    var enemyEntity = enemiesInRadius.Get(idx);
                    var enemy = enemyService.GetEnemy(enemyEntity);
                    // var enemyPosition = movementService.GetTransform(enemyEntity).position;

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

        public Tower_FindTarget_System(float interval, float timeShift, Func<float> getDeltaTime) : base(interval, timeShift, getDeltaTime)
        {
        }
    }
}