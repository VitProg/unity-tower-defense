using System;
using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using td.features.enemy;
using td.features.eventBus;
using td.features.fx;
using td.features.fx.effects;
using td.features.impactEnemy.bus;
using td.features.state;
using UnityEngine;

namespace td.features.impactEnemy.systems
{
    public class SpeedDebuffSystem : IProtoRunSystem
    {
        [DI] private ImpactEnemy_Aspect aspect;
        [DI] private State state;
        [DI] private ImpactEnemy_Service impactEnemy;
        [DI] private Enemy_Service enemyService;
        [DI] private EventBus events;
        // [DI] private FX_Service fxService;

        public void Run()
        {
            foreach (var enemyEntity in aspect.itSpeedDebuff)
            {
                ref var debuff = ref aspect.speedDebuffPool.Get(enemyEntity);
                ref var enemy = ref enemyService.GetEnemy(enemyEntity);
                
                if (!debuff.started)
                {
                    debuff.timeRemains = debuff.duration;
                    debuff.started = true;
                    
                    ref var gotEvent = ref events.global.Add<Event_GotSpeedDebuff>();
                    gotEvent.entity = aspect.World().PackEntityWithWorld(enemyEntity);
                    gotEvent.duration = debuff.duration;
                    /*fxService.entityFallow.GetOrAdd<ColdStatusFX>(
                        aspect.World().PackEntityWithWorld(enemyEntity),
                        debuff.duration
                    );*/
                }

                var debafedSpeed = enemy.startingSpeed / Mathf.Max(1f, debuff.speedMultipler);

                var startEndDuration = Math.Max(0.5f, debuff.duration / 10f);
                var mainDuration = Math.Max(0.001f, debuff.duration - startEndDuration * 2);
                
                debuff.timeRemains -= Time.deltaTime * state.GetGameSpeed();

                var timePassed = debuff.duration - debuff.timeRemains;

                ////////////////////////
                
                var inStartPhase = timePassed < startEndDuration;
                var inMainPhase = timePassed >= startEndDuration &&
                                  timePassed <= debuff.duration - startEndDuration;
                var inEndPhase = timePassed > debuff.duration - startEndDuration;

                ////////////////////////
                
                if (inStartPhase)
                {
                    debuff.phase = 1;
                    var timePassedInPhase = timePassed;
                    var t = timePassedInPhase / startEndDuration;
                    // todo
                    t = t * t * (3f - 2f * t);
                    enemyService.ChangeSpeed(enemyEntity, Mathf.Lerp(
                        enemy.startingSpeed,
                        debafedSpeed,
                        t
                    ));
                }

                if (inMainPhase)
                {
                    debuff.phase = 2;
                    enemyService.ChangeSpeed(enemyEntity, debafedSpeed);
                }

                if (inEndPhase)
                {
                    debuff.phase = 3;
                    var timePassedInPhase = timePassed - mainDuration - startEndDuration;
                    var t = timePassedInPhase / startEndDuration;
                    // todo
                    t =  t * t * (3f - 2f * t);
                    enemyService.ChangeSpeed(enemyEntity, Mathf.Lerp(
                        debafedSpeed,
                        enemy.startingSpeed,
                        t
                    ));
                }
                
                if (debuff.timeRemains < -0.001f)
                {
                    impactEnemy.RemoveSpeedDebuff(enemyEntity);
                    enemyService.ChangeSpeed(enemyEntity, enemy.startingSpeed);
                    // fxService.entityFallow.Remove<ColdStatusFX>(aspect.World().PackEntityWithWorld(enemyEntity));
                }
            }
        }
    }
}