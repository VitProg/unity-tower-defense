using System;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.features._common.flags;
using td.features.enemy;
using td.features.enemy.components;
using td.features.fx;
using td.features.fx.effects;
using td.features.impactEnemy.components;
using td.features.state;
using td.utils.ecs;
using UnityEngine;

namespace td.features.impactEnemy.systems
{
    public class SpeedDebuffSystem : IEcsRunSystem
    {
        private readonly EcsInject<IState> state;
        private readonly EcsInject<Enemy_Service> enemyService;
        private readonly EcsInject<ImpactEnemy_Service> impactEnemy;
        private readonly EcsInject<FX_Service> fxService;
        private readonly EcsWorldInject world;
        
        private readonly EcsFilterInject<Inc<SpeedDebuff, Enemy>, Exc<IsDestroyed>> speedDebuffEntities = default;

        public void Run(IEcsSystems systems)
        {
            foreach (var enemyEntity in speedDebuffEntities.Value)
            {
                ref var debuff = ref speedDebuffEntities.Pools.Inc1.Get(enemyEntity);
                ref var enemy = ref speedDebuffEntities.Pools.Inc2.Get(enemyEntity);
                
                if (!debuff.started)
                {
                    debuff.timeRemains = debuff.duration;
                    debuff.started = true;
                    fxService.Value.EntityFallow.GetOrAdd<ColdStatusFX>(
                        world.Value.PackEntityWithWorld(enemyEntity),
                        debuff.duration
                    );
                }

                var debafedSpeed = enemy.startingSpeed / Mathf.Max(1f, debuff.speedMultipler);

                var startEndDuration = Math.Max(0.5f, debuff.duration / 10f);
                var mainDuration = Math.Max(0.001f, debuff.duration - startEndDuration * 2);
                
                debuff.timeRemains -= Time.deltaTime * state.Value.GameSpeed;

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
                    enemyService.Value.ChangeSpeed(enemyEntity, Mathf.Lerp(
                        enemy.startingSpeed,
                        debafedSpeed,
                        t
                    ));
                }

                if (inMainPhase)
                {
                    debuff.phase = 2;
                    enemyService.Value.ChangeSpeed(enemyEntity, debafedSpeed);
                }

                if (inEndPhase)
                {
                    debuff.phase = 3;
                    var timePassedInPhase = timePassed - mainDuration - startEndDuration;
                    var t = timePassedInPhase / startEndDuration;
                    // todo
                    t =  t * t * (3f - 2f * t);
                    enemyService.Value.ChangeSpeed(enemyEntity, Mathf.Lerp(
                        debafedSpeed,
                        enemy.startingSpeed,
                        t
                    ));
                }
                
                if (debuff.timeRemains < -0.001f)
                {
                    impactEnemy.Value.RemoveSpeedDebuff(enemyEntity);
                    enemyService.Value.ChangeSpeed(enemyEntity, enemy.startingSpeed);
                    fxService.Value.EntityFallow.Remove<ColdStatusFX>(world.Value.PackEntityWithWorld(enemyEntity));
                }
            }
        }
    }
}