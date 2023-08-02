// using Leopotam.EcsLite;
// using Leopotam.EcsLite.Di;
// using td.features._common;
// using td.features._common.flags;
// using td.features.impactEnemy;
// using td.features.projectile.components;
// using td.features.projectile.explosion;
// using td.features.projectile.lightning;
// using UnityEngine;
//
// namespace td.features.projectile.systems
// {
//     public class OLDProjectileReachTargetSystem : IEcsRunSystem
//     {
//         private readonly EcsInject<LightningLine_Service> lightningLineService;
//         private readonly EcsInject<Explosion_Service> explosionService;
//         private readonly EcsInject<Common_Service> common;
//         private readonly EcsInject<Projectile_Service> projectileService;
//         private readonly EcsInject<ImpactEnemy_Service> impactEnemy;
//         private readonly EcsWorldInject world;
//         
//         private readonly EcsFilterInject<Inc<IsTargetReached, Projectile/*, ProjectileTarget*/>, ExcludeNotAlive> entities = default;
//
//         public void Run(IEcsSystems systems)
//         {
//             foreach (var projectileEntity in entities.Value)
//             {
//                 ref var fireTarget = ref projectileService.Value.GetTarget(projectileEntity);
//                 Debug.Log("Projectile reach target: " + fireTarget);
//                 
//                 if (fireTarget.targetEntity.Unpack(world.Value, out var targetEntity))
//                 {
//                     if (projectileService.Value.HasDamageAttribute(projectileEntity))
//                     {
//                         ref var damageProjectile = ref projectileService.Value.GetDamageAttribute(projectileEntity);
//                         impactEnemy.Value.TakeDamage(targetEntity, damageProjectile.damage);
//                     }
//
//                     if (projectileService.Value.HasExplosiveAttribute(projectileEntity))
//                     {
//                         ref var explosiveProjectile = ref projectileService.Value.GetExplosiveAttribute(projectileEntity);
//                         var targetPosition = common.Value.GetGOPosition(targetEntity);
//                         explosionService.Value.SpawnExplosion(targetPosition, explosiveProjectile.damage, explosiveProjectile.diameter, explosiveProjectile.damageFading);
//                     }
//
//                     if (projectileService.Value.HasLightningAttribute(projectileEntity))
//                     {
//                         ref var lightningProjectile = ref projectileService.Value.GetLightningAttribute(projectileEntity);
//                         lightningLineService.Value.SpawnLightningLine(fireTarget.targetEntity, ref lightningProjectile);
//                     }
//
//                     if (projectileService.Value.HasSlowingAttribute(projectileEntity))
//                     {
//                         ref var slowingProjectile = ref projectileService.Value.GetSlowingAttribute(projectileEntity);
//                         impactEnemy.Value.SpeedDebuff(targetEntity, slowingProjectile.duration, slowingProjectile.speedMultipler);
//                     }
//
//                     if (projectileService.Value.HasPoisonAttribute(projectileEntity))
//                     {
//                         ref var poisonProjectile = ref projectileService.Value.GetPoisonAttribute(projectileEntity);
//                         impactEnemy.Value.PoisonDebuff(targetEntity, poisonProjectile.damageInterval, poisonProjectile.duration, poisonProjectile.damageInterval);
//                     }
//
//                     if (projectileService.Value.HasShockingAttribute(projectileEntity))
//                     {
//                         ref var shockingProjectile = ref projectileService.Value.GetShockingAttribute(projectileEntity);
//                         impactEnemy.Value.ShockingDebuff(targetEntity, shockingProjectile.probability, shockingProjectile.duration);
//                     }
//                 }
//
//                 common.Value.SafeDelete(projectileEntity);
//             }
//         }
//     }
// }