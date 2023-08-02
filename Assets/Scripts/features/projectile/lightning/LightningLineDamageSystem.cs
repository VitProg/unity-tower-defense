using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.features._common;
using td.features._common.components;
using td.features._common.flags;
using td.features.enemy;
using td.features.impactEnemy;
using td.features.projectile.attributes;
using td.features.state;
using UnityEngine;

namespace td.features.projectile.lightning
{
    /**
     * Periodically deals damage to enemies in chains, with damage fading from the first to the last one
     */
    public class LightningLineDamageSystem : IEcsRunSystem
    {
        private readonly EcsInject<IState> state;
        private readonly EcsInject<Enemy_Service> enemyService;
        private readonly EcsInject<ImpactEnemy_Service> impactEnemy;
        private readonly EcsInject<Common_Service> common;
        private readonly EcsInject<IEventBus> events;

        private readonly EcsFilterInject<Inc<LightningLine, LightningAttribute, Ref<GameObject>>, Exc<IsDisabled, IsDestroyed>> entities = default;

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in entities.Value)
            {
                ref var lightningLine = ref entities.Pools.Inc1.Get(entity);
                ref var lightning = ref entities.Pools.Inc2.Get(entity);
                
                if (!lightningLine.started)
                {
                    lightningLine.timeRemains = lightning.duration;
                    lightningLine.damageIntervalRemains = lightning.damageInterval;
                    lightningLine.started = true;
                }
                
                lightningLine.timeRemains -= Time.deltaTime * state.Value.GameSpeed;
                lightningLine.damageIntervalRemains -= Time.deltaTime * state.Value.GameSpeed;

                if (lightningLine.timeRemains < Constants.ZeroFloat)
                {
                    common.Value.SafeDelete(entity);
                    continue;
                }

                if (lightningLine.damageIntervalRemains < 0f)
                {
                    var damage = lightning.damage;
                    for (var index = 0; index < lightningLine.length; index++)
                    {
                        if (!enemyService.Value.IsAlive(lightningLine.chainEntities[index], out var chainEntity)) continue;
                        impactEnemy.Value.TakeDamage(chainEntity, damage, DamageType.Electro);
                        damage /= lightning.damageReduction;
                    }
                    lightningLine.damageIntervalRemains = lightning.damageInterval;
                }
            }
        }
    }
}