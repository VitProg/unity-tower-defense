using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using td.features._common;
using td.features.destroy;
using td.features.enemy;
using td.features.impactEnemy;
using td.features.state;
using td.utils;
using UnityEngine;

namespace td.features.projectile.lightning
{
    /**
     * Periodically deals damage to enemies in chains, with damage fading from the first to the last one
     */
    public class LightningDamageSystem : IProtoRunSystem
    {
        [DI] private Lightning_Aspect lightningAspect;
        [DI] private Projectile_Aspect projectileAspect;
        [DI] private Destroy_Service destroyService; 
        [DI] private Enemy_Service enemyService; 
        [DI] private ImpactEnemy_Service impactEnemy; 
        [DI] private State state; 

        public void Run()
        {
            foreach (var entity in lightningAspect.it)
            {
                ref var lightning = ref lightningAspect.lightningPool.Get(entity);
                ref var lightningAttr = ref projectileAspect.lightningAttributePool.Get(entity);
                
                if (!lightning.started)
                {
                    lightning.timeRemains = lightningAttr.duration;
                    lightning.damageIntervalRemains = lightningAttr.damageInterval;
                    lightning.started = true;
                }
                
                lightning.timeRemains -= Time.deltaTime * state.GetGameSpeed();
                lightning.damageIntervalRemains -= Time.deltaTime * state.GetGameSpeed();

                if (lightning.timeRemains < Constants.ZeroFloat)
                {
                    destroyService.MarkAsRemoved(projectileAspect.World().PackEntityWithWorld(entity));
                    continue;
                }

                if (lightning.damageIntervalRemains < 0f)
                {
                    var damage = lightningAttr.damage;
                    for (var index = 0; index < lightning.length; index++)
                    {
                        if (!enemyService.IsAlive(lightning.chainEntities[index], out var chainEntity)) continue;
                        impactEnemy.TakeDamage(chainEntity, damage, DamageType.Electro);
                        damage /= lightningAttr.damageReduction;
                    }
                    lightning.damageIntervalRemains = lightningAttr.damageInterval;
                }
            }
        }
    }
}