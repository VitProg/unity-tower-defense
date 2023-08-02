using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.features._common;
using td.features.goPool;
using td.monoBehaviours;
using td.utils;
using UnityEngine;

namespace td.features.projectile.explosion
{
    public class Explosion_Service
    {
        private readonly EcsInject<Prefab_Service> prefabService;
        private readonly EcsInject<GameObjectPool_Service> poolService;
        private readonly EcsInject<Explosion_Converter> converter;
        private readonly EcsInject<Projectile_Service> projectileService;
        private readonly EcsWorldInject world;

        private PoolableObject CreateObject(Vector2 position)
        {
            var prefab = prefabService.Value.GetPrefab(PrefabCategory.Projectiles, "explosion");
            var projectilePoolableObject = poolService.Value.Get(
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

            var explosionEntity = converter.Value.GetEntity(explosivePo.gameObject) ?? world.Value.NewEntity();
            converter.Value.Convert(explosivePo.gameObject, explosionEntity);
            
            ref var explosiveAttribute = ref projectileService.Value.GetExplosiveAttribute(explosionEntity);
            explosiveAttribute.damage = damage;
            explosiveAttribute.diameter = diameter;
            explosiveAttribute.damageFading = damageFading;
            
            ref var explosion = ref projectileService.Value.GetExplosion(explosionEntity);
            explosion.position = position;
            explosion.currentDiameter = 0f;
            explosion.diameterIncreaseSpeed = 3f;
            explosion.lastCalcDiameter = 0f;
            explosion.progress = 0f;
        }
    }
}