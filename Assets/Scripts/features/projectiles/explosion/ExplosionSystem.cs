using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.components.commands;
using td.components.flags;
using td.components.refs;
using td.features.enemies.components;
using td.features.impactsEnemy;
using td.features.projectiles.attributes;
using td.utils;
using td.utils.ecs;
using UnityEngine;

namespace td.features.projectiles.explosion
{
    public class ExplosionSystem : IEcsRunSystem
    {
        [InjectWorld] private EcsWorld world;
        
        private readonly EcsFilterInject<Inc<Explosion, ExplosiveAttribute, Ref<GameObject>>, Exc<IsDisabled, IsDestroyed>> entities = default;
        private readonly EcsFilterInject<Inc<Enemy, Ref<GameObject>>, Exc<IsDisabled, IsDestroyed>> enemyEntities = default;

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in entities.Value)
            {
                ref var explosion = ref entities.Pools.Inc1.Get(entity);
                ref var explosiveAttribute = ref entities.Pools.Inc2.Get(entity);
                var explosionGO = entities.Pools.Inc3.Get(entity).reference;
                var explosionMb = explosionGO.GetComponent<ExplosionMonoBehaviour>();

                explosion.progress += explosion.diameterIncreaseSpeed * Time.deltaTime;

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
                    world.GetComponent<IsDisabled>(entity);
                    world.GetComponent<RemoveGameObjectCommand>(entity);
                }

                if (calcDamage)
                {
                    var sqrRadiusMax = Mathf.Pow(explosiveAttribute.diameter / 2f, 2f);
                    var sqrRadiusFrom = Mathf.Pow(explosion.lastCalcDiameter / 2f, 2f);
                    var sqrRadiusTo = Mathf.Pow(explosion.currentDiameter / 2f, 2f);
                    
                    foreach (var enemyEntity in enemyEntities.Value)
                    {
                        var enemyPosition = (Vector2)enemyEntities.Pools.Inc2.Get(enemyEntity).reference.transform.position;

                        var sqrDistanse = (explosion.position - enemyPosition).sqrMagnitude;

                        if (sqrRadiusFrom <= sqrDistanse && sqrDistanse <= sqrRadiusTo)
                        {
                            var fade = 1 - sqrDistanse / sqrRadiusMax;
                            var damage = explosiveAttribute.damage * (fade * explosiveAttribute.damageFading); // todo
                            // todo explosiveAttribute.damageFading
                            ref var takeDamage = ref systems.Outer<TakeDamageOuter>();
                            takeDamage.targetEntity = world.PackEntity(enemyEntity);
                            takeDamage.damage = damage;
                            takeDamage.type = DamageType.Explosion;
                        }
                    }
                }

                explosionMb.SetDiameter(explosion.currentDiameter);
            }
        }
    }
}