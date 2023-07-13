using System;
using System.Collections.Generic;
using Leopotam.EcsLite;
using td.components.behaviors;
using td.components.flags;
using td.components.refs;
using td.features.shards;
using td.features.shards.config;
using td.monoBehaviours;
using td.services;
using td.services.ecsConverter;
using td.utils;
using td.utils.ecs;
using UnityEngine;

namespace td.features.projectiles
{
    public sealed class ProjectileService
    {
        [Inject] private GameObjectPoolService poolService;
        [Inject] private EntityConverters converters;
        [Inject] private PrefabService prefabService;
        [Inject] private ShardsConfig shardsConfig; // todo
        [InjectWorld] private EcsWorld world;
        [InjectWorld(Constants.Worlds.Outer)] private EcsWorld outerWorld;

        private PoolableObject CreateObject(string name, Vector2 position)
        {
            var prefab = prefabService.GetPrefab(PrefabCategory.Projectiles, name);
            var projectilePoolableObject = poolService.Get(
                prefab,
                // todo add parent
                Constants.Pools.ProjectileDefaultCopacity,
                Constants.Pools.ProjectileMaxCopacity,
                null,
                null,
                ActionOnRelease,
                EcsPoolUtils.ActionOnDestroy
            );
            var transform = projectilePoolableObject.transform;
            transform.position = position;
            transform.rotation = new Quaternion(0, 0, 0, 0);
            transform.localScale = Vector2.one;

            return projectilePoolableObject;
        }

        private static void ActionOnRelease(PoolableObject o)
        {
            o.gameObject.SetActive(false);
            var projectileMb = o.GetComponent<ProjectileMonoBehaviour>();
            projectileMb.trailRenderer.Clear();
        }

        //todo
        public int SpawnProjectile(string name, Vector2 position, int targetEntity, float speed, int whoFired, ref Shard shard)
        {
            var projectile = CreateObject(name, position);

            if (!converters.Convert<Projectile>(projectile.gameObject, out var projectileEntity))
            {
                throw new NullReferenceException($"Failed to convert GameObject {projectile.gameObject.name}");
            }

            if (!world.HasComponent<Ref<GameObject>>(targetEntity))
            {
                throw new NullReferenceException($"Target Entity should be have Ref<GameObject> component");
            }

            ref var targetGameObjectRef = ref world.GetComponent<Ref<GameObject>>(targetEntity);

            if (targetGameObjectRef.reference == null)
            {
                throw new NullReferenceException($"Reference to GameObject in Target Entity is empty");
            }

            var targetPosition = (Vector2)targetGameObjectRef.reference.transform.position;

            world.GetComponent<Projectile>(projectileEntity).whoFired = world.PackEntity(whoFired);

            ref var movement = ref world.GetComponent<LinearMovementToTarget>(projectileEntity);
            movement.target = targetPosition;
            movement.speed = speed;
            movement.gap = Constants.DefaultGap;
            movement.speedOfGameAffected = true;

            world.GetComponent<ProjectileTarget>(projectileEntity).targetEntity = world.PackEntity(targetEntity);

            world.DelComponent<IsDisabled>(projectileEntity);
            world.DelComponent<IsDestroyed>(projectileEntity);
            
            /***/
            var projectileMB = projectile.GetComponent<ProjectileMonoBehaviour>();
            var color = ShardUtils.GetMixedColor(ref shard, shardsConfig);
            projectileMB.SetColor(color);
            projectileMB.trailRenderer.gameObject.SetActive(true);
            /***/

            return projectileEntity;
        }
    }
}