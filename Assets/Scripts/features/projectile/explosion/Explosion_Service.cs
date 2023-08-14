using Leopotam.EcsProto.QoL;
using td.features.goPool;
using td.features.prefab;
using td.monoBehaviours;
using td.utils;
using td.utils.ecs;
using UnityEngine;

namespace td.features.projectile.explosion
{
    public class Explosion_Service
    {
        [DI] private Explosion_Aspect explosionAspect;
        [DI] private Projectile_Aspect projectileAspect;
        [DI] private Prefab_Service prefabService;
        [DI] private GOPool_Service poolService;
        [DI] private Explosion_Converter converter;

        private PoolableObject CreateObject(Vector2 position)
        {
            var prefab = prefabService.GetPrefab(PrefabCategory.Projectiles, "explosion");
            var projectilePoolableObject = poolService.Get(
                prefab,
                // todo add parent
                Constants.Pools.ProjectileEffectsDefaultCopacity,
                Constants.Pools.ProjectileEffectsMaxCopacity,
                null,
                null,
                null,
                ActionOnDestroy
            );
            var transform = projectilePoolableObject.transform;
            transform.position = position;
            transform.rotation = new Quaternion(0, 0, 0, 0);
            transform.localScale = Vector2.one;

            return projectilePoolableObject;
        }

        private void ActionOnDestroy(PoolableObject o) => EcsPoolUtils.ActionOnDestroy(o);
        
        public void SpawnExplosion(Vector2 position, float damage, float diameter, float damageFading)
        {
            var explosivePo = CreateObject(position);

            var explosionEntity = converter.GetEntity(explosivePo.gameObject) ?? projectileAspect.World().NewEntity();
            converter.Convert(explosivePo.gameObject, explosionEntity);
            
            ref var explosiveAttribute = ref projectileAspect.explosiveAttributePool.GetOrAdd(explosionEntity);
            explosiveAttribute.damage = damage;
            explosiveAttribute.diameter = diameter;
            explosiveAttribute.damageFading = damageFading;
            
            ref var explosion = ref explosionAspect.explosionPool.GetOrAdd(explosionEntity);
            explosion.position = position;
            explosion.currentDiameter = 0f;
            explosion.diameterIncreaseSpeed = 3f;
            explosion.lastCalcDiameter = 0f;
            explosion.progress = 0f;
        }
    }
}