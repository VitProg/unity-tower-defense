using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using td.features._common;
using td.features.destroy;
using td.features.enemy;
using td.features.eventBus;
using td.features.impactEnemy;
using td.features.movement;
using td.features.state;
using td.utils;
using UnityEngine;

namespace td.features.projectile.explosion
{
    public class Explosion_System : IProtoRunSystem
    {
        [DI] private Projectile_Aspect projectileAspect;
        [DI] private Explosion_Aspect explosionAspect;
        [DI] private State state;
        [DI] private Enemy_Service enemyService;
        [DI] private Enemy_Aspect enemyAspect;
        [DI] private EventBus events;
        [DI] private ImpactEnemy_Service impactEnemy;
        [DI] private Movement_Service movementService;
        [DI] private Destroy_Service destroyService;
        
        public void Run()
        {
            foreach (var entity in explosionAspect.it)
            {
                ref var explosion = ref explosionAspect.explosionPool.Get(entity);
                ref var explosiveAttribute = ref projectileAspect.explosiveAttributePool.Get(entity);
                var explosionMb = explosionAspect.refExplosionMBPool.Get(entity).reference;

                if (explosionMb == null)
                {
                    destroyService.MarkAsRemoved(projectileAspect.World().PackEntityWithWorld(entity));
                    continue;
                }

                explosion.progress += explosion.diameterIncreaseSpeed * Time.deltaTime * state.GetGameSpeed();

                explosion.currentDiameter = Mathf.Lerp(
                    0f,
                    explosiveAttribute.diameter,
                    EasingUtils.EaseOutQuad(explosion.progress)
                );

                var diameterDelta = explosion.currentDiameter - explosion.lastCalcDiameter;
                
                var calcDamage = diameterDelta > Constants.WeaponEffects.ExplosionDiameterTresholdToToakeDamage;

                if (explosion.currentDiameter >= explosiveAttribute.diameter || explosion.progress > 1f)
                {
                    calcDamage = true;
                    destroyService.MarkAsRemoved(projectileAspect.World().PackEntityWithWorld(entity));
                }

                if (calcDamage)
                {
                    var sqrRadiusMax = Mathf.Pow(explosiveAttribute.diameter / 2f, 2f);
                    var sqrRadiusFrom = Mathf.Pow(explosion.lastCalcDiameter / 2f, 2f);
                    var sqrRadiusTo = Mathf.Pow(explosion.currentDiameter / 2f, 2f);

                    var enemiesInRadius = enemyService.FindNearestEnemies(explosion.position, sqrRadiusTo, sqrRadiusFrom);
                    
                    for (var idx = 0; idx < enemiesInRadius.Len(); idx++)
                    {
                        var enemyEntity = enemiesInRadius.Get(idx);
                        var enemyPosition = movementService.GetTransform(enemyEntity).position;

                        var sqrDistanse = (explosion.position - enemyPosition).sqrMagnitude;

                        if (sqrRadiusFrom <= sqrDistanse && sqrDistanse <= sqrRadiusTo)
                        {
                            var fade = 1 - sqrDistanse / sqrRadiusMax;
                            var damage = explosiveAttribute.damage * (fade * explosiveAttribute.damageFading); // todo
                            // todo explosiveAttribute.damageFading
                            impactEnemy.TakeDamage(enemyEntity, damage, DamageType.Explosion);
                        }
                    }
                }

                explosionMb.SetDiameter(explosion.currentDiameter);
            }
        }
    }
}