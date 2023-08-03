using System;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.features._common;
using td.features._common.components;
using td.features._common.systems;
using td.features.enemy.components;

namespace td.features.enemy.systems
{
    public class EnemyBaseMovementToTargetSystem : BaseMovementToTargetSystem
    {
        private readonly EcsFilterInject<Inc<MovementToTarget, ObjectTransform, Enemy>, ExcludeImmoveable> enemyMovementFilter = default;

        protected override void InitFilter()
        {
            filter = enemyMovementFilter.Value;
        }

        public EnemyBaseMovementToTargetSystem(float interval, float timeShift, Func<float> getDeltaTime) : base(interval, timeShift, getDeltaTime)
        {
        }
    }
}