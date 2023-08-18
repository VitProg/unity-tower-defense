using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using td.features._common;
using td.features.eventBus;
using td.features.impactEnemy.bus;
using td.features.state;
using UnityEngine;

namespace td.features.impactEnemy.systems
{
    public class PoisonDebuffSystem : IProtoRunSystem
    {
        [DI] private ImpactEnemy_Aspect aspect;
        [DI] private State state;
        [DI] private ImpactEnemy_Service impactEnemy;
        [DI] private EventBus events;
        // [DI] private FX_Service fxService;
        
        public void Run()
        {
            foreach (var enemyEntity in aspect.itPoisonDebuff)
            {
                ref var debuff = ref aspect.poisonDebuffPool.Get(enemyEntity);

                if (!debuff.started)
                {
                    debuff.timeRemains = debuff.duration;
                    debuff.intervalRemains = .5f;
                    debuff.started = true;
                    
                    ref var gotEvent = ref events.global.Add<Event_GotPoisonDebuff>();
                    gotEvent.entity = aspect.World().PackEntityWithWorld(enemyEntity);
                    gotEvent.duration = debuff.duration;
                    /*fxService.entityFallow.GetOrAdd<PoisonStatusFX>(
                        aspect.World().PackEntityWithWorld(enemyEntity),
                        debuff.duration
                    );*/
                }
                
                debuff.timeRemains -= Time.deltaTime * state.GetGameSpeed();
                debuff.intervalRemains -= Time.deltaTime * state.GetGameSpeed();

                if (debuff.timeRemains < 0f)
                {
                    impactEnemy.RemovePoisonDebuff(enemyEntity);
                    continue;
                }

                if (debuff.intervalRemains < 0f)
                {
                    impactEnemy.TakeDamage(enemyEntity, debuff.damage, DamageType.Poison);
                    debuff.intervalRemains = .5f;
                }
            }
        }
    }
}