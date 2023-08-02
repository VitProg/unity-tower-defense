using System;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.common;
using td.features._common.components;
using td.features.enemy.components;
using td.features.level;
using td.utils.ecs;
using UnityEngine;

namespace td.features.enemy.systems
{
    public class EnemyCalcDistanceToKernelSystem : EcsIntervalableRunSystem
    {
        private readonly EcsInject<LevelMap> levelMap;
        private readonly EcsInject<EnemyPath_Service> enemyPathService;

        private readonly EcsFilterInject<Inc<Enemy, Ref<GameObject>, EnemyPath, MovementToTarget>, ExcludeNotAliveEnemies> entities = default;
            
        public override void IntervalRun(IEcsSystems systems, float deltaTime)
        {
            foreach (var enemyEntity in entities.Value)
            {
                ref var enemy = ref entities.Pools.Inc1.Get(enemyEntity);
                ref var enemyPath = ref entities.Pools.Inc3.Get(enemyEntity);
                ref var toTarget = ref entities.Pools.Inc4.Get(enemyEntity);
                
                var enemyGameObject = entities.Pools.Inc2.Get(enemyEntity);
                var enemyPosition = enemyGameObject.reference.transform.position;

                var path = enemyPathService.Value.GetPath(ref enemyPath);

                var nextStep = enemyPath.index + 1 < path.Count ? path[enemyPath.index + 1] : (Int2?)null;

                var nextCellPosition = nextStep.HasValue
                    ? EnemyUtils.CalcPosition(nextStep.Value, enemyGameObject.reference.transform.rotation, enemy.offset) : 
                    (Vector2?)null;
                
                var percentToNextCell = Mathf.Min(1f, nextCellPosition.HasValue ? ((Vector2)enemyPosition - toTarget.target).magnitude : 0f);

                var numberOfCellsToKernel = path.Count - enemyPath.index;
                
                var distanceToKernel = numberOfCellsToKernel + percentToNextCell;
                
                enemy.distanceToKernel = distanceToKernel;
            }
        }

        public EnemyCalcDistanceToKernelSystem(float interval, float timeShift, Func<float> getDeltaTime) : base(interval, timeShift, getDeltaTime)
        {
        }
    }
}