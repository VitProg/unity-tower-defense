using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.features._common;
using td.features._common.components;
using td.features._common.flags;
using td.features.enemy;
using td.features.impactEnemy;
using td.features.projectile.components;
using td.features.projectile.explosion;
using td.features.projectile.lightning;
using UnityEngine;

namespace td.features.projectile.systems
{
    public class ProjectileReachTargetSystem : IEcsRunSystem
    {
        private readonly EcsInject<LightningLine_Service> lightningLineService;
        private readonly EcsInject<Explosion_Service> explosionService;
        private readonly EcsInject<Common_Service> common;
        private readonly EcsInject<Projectile_Service> projectileService;
        private readonly EcsInject<ImpactEnemy_Service> impactEnemy;
        private readonly EcsInject<Enemy_Service> enemyService;
        private readonly EcsWorldInject world;

        private readonly
            EcsFilterInject<Inc<Projectile, IsTargetReached, ObjectTransform, MovementToTarget>, ExcludeNotAlive>
            entities = default;

        public void Run(IEcsSystems systems)
        {
            foreach (var projectileEntity in entities.Value)
            {
                ref var projectile = ref entities.Pools.Inc1.Get(projectileEntity);
                ref var transform = ref entities.Pools.Inc3.Get(projectileEntity);
                ref var movement = ref entities.Pools.Inc4.Get(projectileEntity);

                // Debug.Log("Projectile reach target: " + transform.position);

                common.Value.SafeDelete(projectileEntity);

                // ищем ближайшего врага
                if (!enemyService.Value.FindNearestEnemy(movement.target, movement.gapSqr, out var enemyEntity))
                    continue;

                if (projectileService.Value.HasDamageAttribute(projectileEntity))
                {
                    ref var damageProjectile = ref projectileService.Value.GetDamageAttribute(projectileEntity);
                    impactEnemy.Value.TakeDamage(enemyEntity, damageProjectile.damage);
                }

                if (projectileService.Value.HasExplosiveAttribute(projectileEntity))
                {
                    ref var explosiveProjectile = ref projectileService.Value.GetExplosiveAttribute(projectileEntity);
                    var targetPosition = common.Value.GetGOPosition(enemyEntity);
                    explosionService.Value.SpawnExplosion(targetPosition, explosiveProjectile.damage,
                        explosiveProjectile.diameter, explosiveProjectile.damageFading);
                }

                if (projectileService.Value.HasLightningAttribute(projectileEntity))
                {
                    ref var lightningProjectile = ref projectileService.Value.GetLightningAttribute(projectileEntity);
                    lightningLineService.Value.SpawnLightningLine(world.Value.PackEntity(enemyEntity), ref lightningProjectile);
                }

                if (projectileService.Value.HasSlowingAttribute(projectileEntity))
                {
                    ref var slowingProjectile = ref projectileService.Value.GetSlowingAttribute(projectileEntity);
                    impactEnemy.Value.SpeedDebuff(enemyEntity, slowingProjectile.duration,
                        slowingProjectile.speedMultipler);
                }

                if (projectileService.Value.HasPoisonAttribute(projectileEntity))
                {
                    ref var poisonProjectile = ref projectileService.Value.GetPoisonAttribute(projectileEntity);
                    impactEnemy.Value.PoisonDebuff(enemyEntity, poisonProjectile.damageInterval,
                        poisonProjectile.duration, poisonProjectile.damageInterval);
                }

                if (projectileService.Value.HasShockingAttribute(projectileEntity))
                {
                    ref var shockingProjectile = ref projectileService.Value.GetShockingAttribute(projectileEntity);
                    impactEnemy.Value.ShockingDebuff(enemyEntity, shockingProjectile.probability,
                        shockingProjectile.duration);
                }
            }
        }
    }
}