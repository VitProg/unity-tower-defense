using Leopotam.EcsProto.QoL;
using td.features.destroy;
using td.features.enemy;
using td.features.goPool;
using td.features.movement;
using td.features.prefab;
using td.features.projectile.attributes;
using td.features.projectile.components;
using td.features.projectile.explosion;
using td.features.projectile.lightning;
using td.features.shard;
using td.monoBehaviours;
using td.utils;
using td.utils.ecs;
using UnityEngine;

namespace td.features.projectile
{
    public sealed class Projectile_Service
    {
        [DI] private Projectile_Aspect aspect;
        [DI] private GOPool_Service poolService;
        [DI] private Projectile_Converter converter;
        [DI] private Prefab_Service prefabService;
        [DI] private Enemy_Service enemyService; // todo
        [DI] private ShardsConfig shardsConfig; // todo
        [DI] private Movement_Service movementService;
        [DI] private Destroy_Service destroyService;

        public bool HasProjectile(int projectileEntity) => aspect.projectilePool.Has(projectileEntity);
        public ref Projectile GetProjectile(int projectileEntity) => ref aspect.projectilePool.GetOrAdd(projectileEntity);
        
        public bool HasTarget(int projectileEntity) => aspect.projectileTargetPool.Has(projectileEntity);
        public ref ProjectileTarget GetTarget(int projectileEntity) => ref aspect.projectileTargetPool.GetOrAdd(projectileEntity);
        
        public bool HasDamageAttribute(int projectileEntity) => aspect.damageAttributePool.Has(projectileEntity);
        public bool HasExplosiveAttribute(int projectileEntity) => aspect.explosiveAttributePool.Has(projectileEntity);
        public bool HasPoisonAttribute(int projectileEntity) => aspect.poisonAttributePool.Has(projectileEntity);
        public bool HasSlowingAttribute(int projectileEntity) => aspect.slowingAttributePool.Has(projectileEntity);
        public bool HasLightningAttribute(int projectileEntity) => aspect.lightningAttributePool.Has(projectileEntity);
        public bool HasShockingAttribute(int projectileEntity) => aspect.shockingAttributePool.Has(projectileEntity);
        
        public ref DamageAttribute GetDamageAttribute(int projectileEntity) => ref aspect.damageAttributePool.GetOrAdd(projectileEntity);
        public ref ExplosiveAttribute GetExplosiveAttribute(int projectileEntity) => ref aspect.explosiveAttributePool.GetOrAdd(projectileEntity);
        public ref PoisonAttribute GetPoisonAttribute(int projectileEntity) => ref aspect.poisonAttributePool.GetOrAdd(projectileEntity);
        public ref SlowingAttribute GetSlowingAttribute(int projectileEntity) => ref aspect.slowingAttributePool.GetOrAdd(projectileEntity);
        public ref LightningAttribute GetLightningAttribute(int projectileEntity) => ref aspect.lightningAttributePool.GetOrAdd(projectileEntity);
        public ref ShockingAttribute GetShockingAttribute(int projectileEntity) => ref aspect.shockingAttributePool.GetOrAdd(projectileEntity);
        
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

            var projectileEntity = converter.GetEntity(projectile.gameObject) ?? aspect.World().NewEntity();

            converter.Convert(projectile.gameObject, projectileEntity);

            ref var movement = ref movementService.GetMovement(projectileEntity);
            movement.from = position;
            movement.target = target;
            movement.speedOfGameAffected = true;
            movement.SetSqrGap(sqrGap);
            movement.SetSpeed(speed);
            
            aspect.projectilePool.GetOrAdd(projectileEntity).whoFired = aspect.World().PackEntityWithWorld(whoFired);

            destroyService.RemoveDestroyedMarks(projectileEntity);

            /***/
            var projectileMB = projectile.GetComponent<ProjectileMonoBehaviour>();
            projectileMB.SetColor(color ?? Color.white);
            projectileMB.trailRenderer.gameObject.SetActive(true);
            /***/

            return projectileEntity;
        }

        public void RemoveAllAttributes(int projectileEntity)
        {
            aspect.damageAttributePool.Del(projectileEntity);
            aspect.explosiveAttributePool.Del(projectileEntity);
            aspect.lightningAttributePool.Del(projectileEntity);
            aspect.poisonAttributePool.Del(projectileEntity);
            aspect.shockingAttributePool.Del(projectileEntity);
            aspect.slowingAttributePool.Del(projectileEntity);
        }
    }
}