using System;
using Leopotam.EcsLite.Di;
using td.features._common;
using td.features._common.components;
using td.features._common.systems;
using td.features.projectile.components;

namespace td.features.projectile.systems
{
    public class ProjectileMovementToTargetSystem : BaseMovementToTargetSystem
    {
        private readonly EcsFilterInject<Inc<MovementToTarget, ObjectTransform, Projectile>, ExcludeImmoveable> projectileMovementFilter = default;

        protected override void InitFilter()
        {
            filter = projectileMovementFilter.Value;
        }

        public ProjectileMovementToTargetSystem(float interval, float timeShift, Func<float> getDeltaTime) : base(interval, timeShift, getDeltaTime)
        {
        }   
    }
}