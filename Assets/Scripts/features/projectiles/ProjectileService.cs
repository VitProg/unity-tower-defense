using System;
using System.Collections.Generic;
using Leopotam.EcsLite;
using td.components.behaviors;
using td.components.flags;
using td.components.refs;
using td.features.projectiles.attributes;
using td.monoBehaviours;
using td.services;
using td.services.ecsConverter;
using td.utils.ecs;
using UnityEngine;

namespace td.features.projectiles
{
    public class ProjectileService
    {
        [Inject] private GameObjectPoolService poolService;
        [Inject] private EntityConverters converters;
        [InjectWorld] private EcsWorld world;
        [InjectWorld(Constants.Worlds.Outer)] private EcsWorld outerWorld;

        private readonly Dictionary<string, GameObject> prefabsDictionary = new();

        private GameObject GetPrefab(string name)
        {
            if (!prefabsDictionary.TryGetValue(name, out var prefab))
            {
                prefab = (GameObject)Resources.Load($"Prefabs/projectiles/{name}", typeof(GameObject));
                prefabsDictionary.Add(name, prefab);
            }

            return prefab;
        }

        public PoolableObject CreateObject(string name, Vector2 position)
        {
            var prefab = GetPrefab(name);
            var projectilePoolableObject = poolService.Get(
                prefab,
                // todo add parent
                Constants.Pools.ProjectileDefaultCopacity,
                Constants.Pools.ProjectileMaxCopacity,
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

        public void ApplyCommandAttributes(int commandEntity, int projectileEntity)
        {
            outerWorld.CopyComponent<DamageAttribute>(commandEntity, projectileEntity);
            outerWorld.CopyComponent<ExplosiveAttribute>(commandEntity, projectileEntity);
            outerWorld.CopyComponent<LightningAttribute>(commandEntity, projectileEntity);
            outerWorld.CopyComponent<PoisonAttribute>(commandEntity, projectileEntity);
            outerWorld.CopyComponent<SlowingAttribute>(commandEntity, projectileEntity);
        }

        public int SpawnProjectile(string name, Vector2 position, int targetEntity, float speed, int whoFired)
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

            // if (world.HasComponent<LinearMovementToTarget>(targetEntity))
            // {
            //     ref var targetMovement = ref world.GetComponent<LinearMovementToTarget>(targetEntity);
            //     
            //     var vector = targetMovement.target - targetPosition;
            //     var sqrDistance = vector.sqrMagnitude;
            //     
            //     vector.Normalize();
            //     vector *= ((targetMovement.speed / 2f) + (speed / 2f)) * (sqrDistance / 100f);
            //     targetPosition += vector;
            // }

            world.GetComponent<Projectile>(projectileEntity).WhoFired = world.PackEntity(whoFired);

            ref var movement = ref world.GetComponent<LinearMovementToTarget>(projectileEntity);
            movement.target = targetPosition;
            movement.speed = speed;
            movement.gap = Constants.DefaultGap;

            world.GetComponent<ProjectileTarget>(projectileEntity).TargetEntity = world.PackEntity(targetEntity);

            world.DelComponent<IsDisabled>(projectileEntity);
            world.DelComponent<IsDestroyed>(projectileEntity);

            return projectileEntity;
        }
    }
}