using System;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.features._common;
using td.features.enemy;
using td.features.goPool;
using td.features.projectile;
using td.features.projectile.attributes;
using td.features.projectile.components;
using td.features.projectile.explosion;
using td.features.projectile.lightning;
using td.features.shard;
using td.monoBehaviours;
using td.utils;
using td.utils.ecs;
using UnityEngine;
using Random = UnityEngine.Random;

namespace td.features.projectile
{
    public sealed class Projectile_Service
    {
        private readonly EcsInject<GameObjectPool_Service> poolService;
        private readonly EcsInject<Projectile_Converter> converter;
        private readonly EcsInject<Prefab_Service> prefabService;
        private readonly EcsInject<Enemy_Service> enemyService; // todo
        private readonly EcsInject<ShardsConfig> shardsConfig; // todo
        private readonly EcsInject<Projectile_Pools> pools;
        private readonly EcsInject<Common_Service> common;
        
        private readonly EcsWorldInject world;

        public bool HasProjectile(int projectileEntity) => pools.Value.projectilePool.Value.Has(projectileEntity);
        public ref Projectile GetProjectile(int projectileEntity) => ref pools.Value.projectilePool.Value.GetOrAdd(projectileEntity);
        
        public bool HasTarget(int projectileEntity) => pools.Value.projectileTargetPool.Value.Has(projectileEntity);
        public ref ProjectileTarget GetTarget(int projectileEntity) => ref pools.Value.projectileTargetPool.Value.GetOrAdd(projectileEntity);
        
        public bool HasDamageAttribute(int projectileEntity) => pools.Value.damageAttributePool.Value.Has(projectileEntity);
        public bool HasExplosiveAttribute(int projectileEntity) => pools.Value.explosiveAttributePool.Value.Has(projectileEntity);
        public bool HasPoisonAttribute(int projectileEntity) => pools.Value.poisonAttributePool.Value.Has(projectileEntity);
        public bool HasSlowingAttribute(int projectileEntity) => pools.Value.slowingAttributePool.Value.Has(projectileEntity);
        public bool HasLightningAttribute(int projectileEntity) => pools.Value.lightningAttributePool.Value.Has(projectileEntity);
        public bool HasShockingAttribute(int projectileEntity) => pools.Value.shockingAttributePool.Value.Has(projectileEntity);
        
        public ref DamageAttribute GetDamageAttribute(int projectileEntity) => ref pools.Value.damageAttributePool.Value.GetOrAdd(projectileEntity);
        public ref ExplosiveAttribute GetExplosiveAttribute(int projectileEntity) => ref pools.Value.explosiveAttributePool.Value.GetOrAdd(projectileEntity);
        public ref PoisonAttribute GetPoisonAttribute(int projectileEntity) => ref pools.Value.poisonAttributePool.Value.GetOrAdd(projectileEntity);
        public ref SlowingAttribute GetSlowingAttribute(int projectileEntity) => ref pools.Value.slowingAttributePool.Value.GetOrAdd(projectileEntity);
        public ref LightningAttribute GetLightningAttribute(int projectileEntity) => ref pools.Value.lightningAttributePool.Value.GetOrAdd(projectileEntity);
        public ref ShockingAttribute GetShockingAttribute(int projectileEntity) => ref pools.Value.shockingAttributePool.Value.GetOrAdd(projectileEntity);
        
        public bool HasExplosion(int projectileEntity) => pools.Value.explosionPool.Value.Has(projectileEntity);
        public ref Explosion GetExplosion(int projectileEntity) => ref pools.Value.explosionPool.Value.GetOrAdd(projectileEntity);
        public bool HasLightningLine(int projectileEntity) => pools.Value.lightningLinePool.Value.Has(projectileEntity);
        public ref LightningLine GetLightningLine(int projectileEntity) => ref pools.Value.lightningLinePool.Value.GetOrAdd(projectileEntity);

        private PoolableObject CreateObject(string name, Vector2 position)
        {
            var prefab = prefabService.Value.GetPrefab(PrefabCategory.Projectiles, name);
            var projectilePoolableObject = poolService.Value.Get(
                prefab,
                // todo add parent
                Constants.Pools.ProjectileDefaultCopacity,
                Constants.Pools.ProjectileMaxCopacity,
                null,
                null,
                ActionOnRelease,
                ActionOnDestroy
            );
            var transform = projectilePoolableObject.transform;
            transform.position = position;
            transform.rotation = new Quaternion(0, 0, 0, 0);
            transform.localScale = Vector2.one;

            return projectilePoolableObject;
        }
        
        private void ActionOnDestroy(PoolableObject o) => EcsPoolUtils.ActionOnDestroy(o);

        private static void ActionOnRelease(PoolableObject o)
        {
            o.gameObject.SetActive(false);
            var projectileMb = o.GetComponent<ProjectileMonoBehaviour>();
            projectileMb.trailRenderer.Clear();
        }

        //todo
        public int SpawnProjectile(
            string name, 
            Vector2 position,
            Vector2 target,
            float speed,
            float sqrGap,
            int whoFired,
            Color? color = null
        )
        {
            var projectile = CreateObject(name, position);

            var projectileEntity = converter.Value.GetEntity(projectile.gameObject) ?? world.Value.NewEntity();
            converter.Value.Convert(projectile.gameObject, projectileEntity);

            ref var movement = ref common.Value.GetMovement(projectileEntity);
            movement.from = position;
            movement.target = target;
            movement.speedOfGameAffected = true;
            movement.SetSqrGap(sqrGap);
            movement.SetSpeed(speed);
            
            pools.Value.projectilePool.Value.GetOrAdd(projectileEntity).whoFired = world.Value.PackEntity(whoFired);

            common.Value.RemoveDestroyedMarks(projectileEntity);

            /***/
            var projectileMB = projectile.GetComponent<ProjectileMonoBehaviour>();
            projectileMB.SetColor(color ?? Color.white);
            projectileMB.trailRenderer.gameObject.SetActive(true);
            /***/

            return projectileEntity;
        }

        public void RemoveAllAttributes(int projectileEntity)
        {
            pools.Value.damageAttributePool.Value.SafeDel(projectileEntity);
            pools.Value.explosiveAttributePool.Value.SafeDel(projectileEntity);
            pools.Value.lightningAttributePool.Value.SafeDel(projectileEntity);
            pools.Value.poisonAttributePool.Value.SafeDel(projectileEntity);
            pools.Value.shockingAttributePool.Value.SafeDel(projectileEntity);
            pools.Value.slowingAttributePool.Value.SafeDel(projectileEntity);
        }
    }
}