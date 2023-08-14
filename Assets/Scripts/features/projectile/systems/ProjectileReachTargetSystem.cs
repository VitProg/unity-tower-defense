using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using td.features.destroy;
using td.features.enemy;
using td.features.impactEnemy;
using td.features.movement;
using td.features.projectile.explosion;
using td.features.projectile.lightning;

namespace td.features.projectile.systems
{
    public class ProjectileReachTargetSystem : IProtoRunSystem
    {
        [DI] private Projectile_Aspect aspect;
        [DI] private Lightning_Service lightningService;
        [DI] private Explosion_Service explosionService;
        [DI] private Projectile_Service projectileService;
        [DI] private ImpactEnemy_Service impactEnemy;
        [DI] private Movement_Service movementService;
        [DI] private Destroy_Service destroyService;
        [DI] private Enemy_Service enemyService;

        public void Run()
        {
            foreach (var projectileEntity in aspect.itProjectileReachTarget)
            {
                ref var projectile = ref aspect.projectilePool.Get(projectileEntity);
                ref var transform = ref movementService.GetTransform(projectileEntity);
                ref var movement = ref movementService.GetMovement(projectileEntity);

                // Debug.Log("Projectile reach target: " + transform.position);

                destroyService.MarkAsRemoved(aspect.World().PackEntityWithWorld(projectileEntity));

                // ищем ближайшего врага
                if (!enemyService.FindNearestEnemy(movement.target, movement.gapSqr, out var enemyEntity))
                    continue;

                if (projectileService.HasDamageAttribute(projectileEntity))
                {
                    ref var damageProjectile = ref projectileService.GetDamageAttribute(projectileEntity);
                    impactEnemy.TakeDamage(
                        enemyEntity, 
                        damageProjectile.damage,
                        damageProjectile.type
                    );
                }

                if (projectileService.HasExplosiveAttribute(projectileEntity))
                {
                    ref var explosiveProjectile = ref projectileService.GetExplosiveAttribute(projectileEntity);
                    var targetPosition = movementService.GetGOTransform(enemyEntity).position;
                    explosionService.SpawnExplosion(
                        position: targetPosition,
                        damage: explosiveProjectile.damage,
                        diameter: explosiveProjectile.diameter,
                        damageFading: explosiveProjectile.damageFading
                    );
                }

                if (projectileService.HasLightningAttribute(projectileEntity))
                {
                    ref var lightningProjectile = ref projectileService.GetLightningAttribute(projectileEntity);
                    lightningService.SpawnLightningLine(
                        firstEntity: aspect.World().PackEntityWithWorld(enemyEntity),
                        lightningSource: ref lightningProjectile
                    );
                }

                if (projectileService.HasSlowingAttribute(projectileEntity))
                {
                    ref var slowingProjectile = ref projectileService.GetSlowingAttribute(projectileEntity);
                    impactEnemy.SpeedDebuff(
                        target: enemyEntity,
                        duration: slowingProjectile.duration,
                        speedMultipler: slowingProjectile.speedMultipler
                    );
                }

                if (projectileService.HasPoisonAttribute(projectileEntity))
                {
                    ref var poisonProjectile = ref projectileService.GetPoisonAttribute(projectileEntity);
                    impactEnemy.PoisonDebuff(
                        target: enemyEntity, 
                        damage: poisonProjectile.damage,
                        duration: poisonProjectile.duration
                    );
                }

                if (projectileService.HasShockingAttribute(projectileEntity))
                {
                    ref var shockingProjectile = ref projectileService.GetShockingAttribute(projectileEntity);
                    impactEnemy.ShockingDebuff(
                        target: enemyEntity, 
                        probability: shockingProjectile.probability,
                        duration: shockingProjectile.duration
                    );
                }
            }
        }
    }
}