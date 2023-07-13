using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.components.flags;
using td.features.enemies.components;
using td.utils.ecs;
using UnityEngine;

namespace td.features.impactsEnemy
{
    public class PoisonDebuffSystem : IEcsRunSystem
    {
        [InjectWorld] private EcsWorld world;

        private readonly EcsFilterInject<Inc<PoisonDebuff, Enemy>, Exc<IsDestroyed>> poisonDebuffEntities = default;

        public void Run(IEcsSystems systems)
        {
            foreach (var enemyEntity in poisonDebuffEntities.Value)
            {
                ref var debuff = ref poisonDebuffEntities.Pools.Inc1.Get(enemyEntity);

                if (!debuff.started)
                {
                    debuff.timeRemains = debuff.duration;
                    debuff.damageIntervalRemains = debuff.damageInterval;
                    debuff.started = true;
                }
                
                debuff.timeRemains -= Time.deltaTime;
                debuff.damageIntervalRemains -= Time.deltaTime;

                if (debuff.timeRemains < Constants.ZeroFloat)
                {
                    world.DelComponent<PoisonDebuff>(enemyEntity);
                }

                if (debuff.damageIntervalRemains < 0f)
                {
                    ref var takeDamage = ref systems.Outer<TakeDamageOuter>();
                    takeDamage.targetEntity = world.PackEntity(enemyEntity);
                    takeDamage.damage = debuff.damage;
                    takeDamage.type = DamageType.Poison;
                    debuff.damageIntervalRemains = debuff.damageInterval;
                }
            }
        }
    }
}