using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.features._common;
using td.features._common.components;
using td.features.enemy;
using td.features.enemy.components;
using td.features.impactEnemy;
using td.features.projectile.attributes;
using td.features.state;
using td.utils;
using td.utils.ecs;
using UnityEngine;

namespace td.features.projectile.explosion
{
    public class ExplosionSystem : IEcsRunSystem
    {
        private readonly EcsInject<IState> state;
        private readonly EcsInject<Enemy_Service> enemyService;
        private readonly EcsInject<Enemy_Aspect> enemyPools;
        private readonly EcsInject<Common_Service> common;
        private readonly EcsWorldInject world;
        private readonly EcsInject<IEventBus> events;
        private readonly EcsInject<ImpactEnemy_Service> impactEnemy;
        
        private readonly EcsFilterInject<Inc<Explosion, ExplosiveAttribute, Ref<GameObject>>, ExcludeNotAlive> entities = default;
        
        public void Run(IEcsSystems systems)
        {
            foreach (var entity in entities.Value)
            {
                ref var explosion = ref entities.Pools.Inc1.Get(entity);
                ref var explosiveAttribute = ref entities.Pools.Inc2.Get(entity);
                var explosionGO = entities.Pools.Inc3.Get(entity).reference;
                var explosionMb = explosionGO!.GetComponent<ExplosionMonoBehaviour>();

                explosion.progress += explosion.diameterIncreaseSpeed * Time.deltaTime * state.Value.GameSpeed;

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
                    common.Value.SafeDelete(entity);
                }

                if (calcDamage)
                {
                    var sqrRadiusMax = Mathf.Pow(explosiveAttribute.diameter / 2f, 2f);
                    var sqrRadiusFrom = Mathf.Pow(explosion.lastCalcDiameter / 2f, 2f);
                    var sqrRadiusTo = Mathf.Pow(explosion.currentDiameter / 2f, 2f);
                    
                    foreach (var enemyEntity in enemyPools.Value.livingEnemiesFilter.Value)
                    {
                        var enemyPosition = enemyPools.Value.livingEnemiesFilter.Pools.Inc2.Get(enemyEntity).position;

                        var sqrDistanse = (explosion.position - enemyPosition).sqrMagnitude;

                        if (sqrRadiusFrom <= sqrDistanse && sqrDistanse <= sqrRadiusTo)
                        {
                            var fade = 1 - sqrDistanse / sqrRadiusMax;
                            var damage = explosiveAttribute.damage * (fade * explosiveAttribute.damageFading); // todo
                            // todo explosiveAttribute.damageFading
                            impactEnemy.Value.TakeDamage(enemyEntity, damage, DamageType.Explosion);
                        }
                    }
                }

                explosionMb.SetDiameter(explosion.currentDiameter);
            }
        }
    }
}