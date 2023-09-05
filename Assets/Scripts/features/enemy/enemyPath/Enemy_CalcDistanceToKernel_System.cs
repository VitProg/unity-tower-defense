using System;
using Leopotam.EcsProto.QoL;
using Leopotam.Types;
using td.features.movement;
using td.utils;
using td.utils.ecs;

namespace td.features.enemy.enemyPath
{
    public class Enemy_CalcDistanceToKernel_System : ProtoIntervalableRunSystem
    {
        [DI] private Enemy_Aspect aspect;
        [DI] private Movement_Service movementService;
        [DI] private Enemy_Service enemyService;
        [DI] private EnemyPath_Service enemyPathService;
        [DI] private EnemyPath_State enemyPathState;

        public override void IntervalRun(float deltaTime)
        {
            foreach (var enemyEntity in aspect.itMovementEnemies)
            {
                ref var enemy = ref enemyService.GetEnemy(enemyEntity);
                ref var movement = ref movementService.GetMovement(enemyEntity);
                ref var transform = ref movementService.GetTransform(enemyEntity);
                ref var route = ref enemyPathService.GetRoute(enemyEntity);
                
                if (!enemyPathState.HasRoute(route.routeIdx)) continue;

                var routeLength = enemyPathState.GetRouteLength(route.routeIdx);
                ref var currentRouteItem = ref enemyPathState.GetRouteItem(route.routeIdx, route.step);

                var percentToNextCell = 1f;

                if (enemyPathState.HasRouteItem(route.routeIdx, route.step + 1)) {
                    var nextStep = enemyPathState.GetRouteItem(route.routeIdx, route.step + 1);
                    var nextCellPosition = Enemy_Utils.CalcPosition(nextStep.x, nextStep.y, transform.rotation, enemy.offset);
                    percentToNextCell = MathFast.Min(1f, (nextCellPosition - transform.position).Magnitude());
                }

                var numberOfCellsToKernel = routeLength - route.step;

                var distanceToKernel = numberOfCellsToKernel + percentToNextCell;

                enemy.distanceToKernel = distanceToKernel;
            }
        }

        public Enemy_CalcDistanceToKernel_System(float interval, float timeShift, Func<float> getDeltaTime) : base(
            interval, timeShift, getDeltaTime)
        {
        }
    }
}