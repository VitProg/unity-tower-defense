using System;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.components.behaviors;
using td.components.flags;
using td.features.enemies;
using td.features.enemies.components;
using td.utils;
using td.utils.ecs;
using UnityEngine;

namespace td.features.impactsEnemy
{
    public class SpeedDebuffSystem : IEcsRunSystem
    {
        [InjectWorld] private EcsWorld world;
        
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
                }

                var debafedSpeed = enemy.startingSpeed / Mathf.Max(1f, debuff.speedMultipler);

                var startEndDuration = Math.Max(0.5f, debuff.duration / 10f);
                var mainDuration = Math.Max(0.001f, debuff.duration - startEndDuration * 2);
                
                debuff.timeRemains -= Time.deltaTime;

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
                    t = t * t * (3f - 2f * t);;
                    enemy.speed = Mathf.Lerp(
                        enemy.startingSpeed,
                        debafedSpeed,
                        t
                    );
                }

                if (inMainPhase)
                {
                    debuff.phase = 2;
                    enemy.speed = debafedSpeed;
                }

                if (inEndPhase)
                {
                    debuff.phase = 3;
                    var timePassedInPhase = timePassed - mainDuration - startEndDuration;
                    var t = timePassedInPhase / startEndDuration;
                    // todo
                    t =  t * t * (3f - 2f * t);;
                    enemy.speed = Mathf.Lerp(
                        debafedSpeed,
                        enemy.startingSpeed,
                        t
                    );
                }
               
                ////////////////////////
                
                if (world.HasComponent<LinearMovementToTarget>(enemyEntity))
                {
                    world.GetComponent<LinearMovementToTarget>(enemyEntity).speed = enemy.speed;
                }
                
                ////////////////////////

                if (debuff.timeRemains < -0.001f)
                {
                    world.DelComponent<SpeedDebuff>(enemyEntity);
                }
            }
        }
    }
}