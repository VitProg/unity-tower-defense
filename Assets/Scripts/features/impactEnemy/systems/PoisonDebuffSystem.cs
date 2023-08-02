using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.features._common;
using td.features._common.components;
using td.features.enemy.components;
using td.features.impactEnemy.components;
using td.features.state;
using td.utils.ecs;
using UnityEngine;

namespace td.features.impactEnemy.systems
{
    public class PoisonDebuffSystem : IEcsRunSystem
    {
        private readonly EcsInject<IState> state;
        private readonly EcsInject<IEventBus> events;
        private readonly EcsInject<ImpactEnemy_Service> impactEnemy;
        
        private readonly EcsFilterInject<Inc<PoisonDebuff, Enemy, Ref<GameObject>>, ExcludeNotAlive> filter = default;

        public void Run(IEcsSystems systems)
        {
            foreach (var enemyEntity in filter.Value)
            {
                ref var debuff = ref filter.Pools.Inc1.Get(enemyEntity);

                if (!debuff.started)
                {
                    debuff.timeRemains = debuff.duration;
                    debuff.damageIntervalRemains = debuff.damageInterval;
                    debuff.started = true;
                }
                
                debuff.timeRemains -= Time.deltaTime * state.Value.GameSpeed;
                debuff.damageIntervalRemains -= Time.deltaTime * state.Value.GameSpeed;

                if (debuff.timeRemains < Constants.ZeroFloat)
                {
                    impactEnemy.Value.RemovePoisonDebuff(enemyEntity);
                }

                if (debuff.damageIntervalRemains < 0f)
                {
                    impactEnemy.Value.TakeDamage(enemyEntity, debuff.damage, DamageType.Poison);
                    debuff.damageIntervalRemains = debuff.damageInterval;
                }
            }
        }
    }
}