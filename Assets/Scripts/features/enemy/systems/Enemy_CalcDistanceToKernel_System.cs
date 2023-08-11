using System;
using Leopotam.EcsProto.QoL;
using td.common;
using td.features._common;
using td.features.level;
using td.utils.ecs;
using UnityEngine;

namespace td.features.enemy.systems
{
    public class EnemyCalcDistanceToKernelSystem : ProtoIntervalableRunSystem
    {
        [DI] private Enemy_Aspect aspect;
        [DI] private LevelMap levelMap;
        [DI] private Common_Service common;
        [DI] private Enemy_Path_Service enemyPathService;

        public override void IntervalRun(float deltaTime)
        {
            foreach (var entity in aspect.itMovementEnemies)
            {
                ref var enemy = ref aspect.enemyPool.Get(entity);
                ref var enemyPath = ref aspect.enemyPathPool.Get(entity);
                ref var toTarget = ref common.GetMovement(entity);

                var transform = common.GetTransform(entity);

                var path = enemyPathService.GetPath(ref enemyPath);

                var nextStep = enemyPath.index + 1 < path.Count ? path[enemyPath.index + 1] : (Int2?)null;

                var nextCellPosition = nextStep.HasValue
                    ? EnemyUtils.CalcPosition(nextStep.Value, transform.rotation, enemy.offset)
                    : (Vector2?)null;

                var percentToNextCell = Mathf.Min(
                    1f,
                    nextCellPosition.HasValue
                        ? (transform.position - toTarget.target).magnitude
                        : 0f
                );

                var numberOfCellsToKernel = path.Count - enemyPath.index;

                var distanceToKernel = numberOfCellsToKernel + percentToNextCell;

                enemy.distanceToKernel = distanceToKernel;
            }
        }

        public EnemyCalcDistanceToKernelSystem(float interval, float timeShift, Func<float> getDeltaTime) : base(
            interval, timeShift, getDeltaTime)
        {
        }
    }
}