using System;
using Leopotam.EcsLite;
using td.features.projectiles.attributes;
using td.features.shards.config;
using td.monoBehaviours;
using td.services;
using td.services.ecsConverter;
using td.utils;
using td.utils.ecs;
using UnityEngine;

namespace td.features.projectiles.explosion
{
    public class ExplosionService
    {
        [Inject] private PrefabService prefabService;
        [Inject] private GameObjectPoolService poolService;
        [Inject] private EntityConverters converters;
        [Inject] private ShardsConfig shardsConfig; // todo
        [InjectWorld] private EcsWorld world;
        [InjectWorld(Constants.Worlds.Outer)] private EcsWorld outerWorld;
        
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
                EcsPoolUtils.ActionOnDestroy
            );
            var transform = projectilePoolableObject.transform;
            transform.position = position;
            transform.rotation = new Quaternion(0, 0, 0, 0);
            transform.localScale = Vector2.one;

            return projectilePoolableObject;
        }
        
        public int SpawnExplosion(Vector2 position, ref ExplosiveAttribute explosiveSource)
        {
            var explosivePo = CreateObject(position);
            
            if (!converters.Convert<Explosion>(explosivePo.gameObject, out var explosionEntity))
            {
                throw new NullReferenceException($"Failed to convert GameObject {explosivePo.gameObject.name}");
            }
            
            ref var explosiveAttribute = ref world.GetComponent<ExplosiveAttribute>(explosionEntity);
            explosiveAttribute.damage = explosiveSource.damage;
            explosiveAttribute.diameter = explosiveSource.diameter;
            explosiveAttribute.damageFading = explosiveSource.damageFading;
            
            ref var explosion = ref world.GetComponent<Explosion>(explosionEntity);
            explosion.position = position;
            explosion.currentDiameter = 0f;
            explosion.diameterIncreaseSpeed = 3f;
            explosion.lastCalcDiameter = 0f;
            explosion.progress = 0f;
            
            return explosionEntity;
        }
    }
}