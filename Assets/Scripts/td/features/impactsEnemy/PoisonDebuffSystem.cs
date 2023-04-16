using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.features.enemies;
using td.utils.ecs;
using UnityEngine;

namespace td.features.impactsEnemy
{
    public class PoisonDebuffSystem : IEcsRunSystem
    {
        [EcsWorld] private EcsWorld world;

        private readonly EcsFilterInject<Inc<PoisonDebuff, Enemy>> poisonDebufEntities = default;

        public void Run(IEcsSystems systems)
        {
            foreach (var enemyEntity in poisonDebufEntities.Value)
            {
                ref var debuff = ref poisonDebufEntities.Pools.Inc1.Get(enemyEntity);

                if (!debuff.started)
                {
                    debuff.Start();
                }
                
                debuff.timeRemains -= Time.deltaTime;
                debuff.damageIntervalRemains -= Time.deltaTime;

                if (debuff.timeRemains < -0.001f)
                {
                    world.DelComponent<PoisonDebuff>(enemyEntity);
                }

                if (debuff.damageIntervalRemains < 0f)
                {
                    systems.SendOuter(new TakeDamageOuter
                    {
                        TargetEntity = world.PackEntity(enemyEntity),
                        damage = debuff.damage
                    });
                    debuff.damageIntervalRemains = debuff.damageInterval;
                }
            }
        }
    }
}