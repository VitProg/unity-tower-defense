using Leopotam.EcsProto.QoL;
using td.features.goPool;
using td.features.prefab;
using td.features.projectile.attributes;
using td.utils;
using td.utils.ecs;
using UnityEngine;

namespace td.features.projectile.lightning
{
    public sealed class Lightning_Service
    {
        [DI] private Lightning_Aspect lightningAspect;
        [DI] private Projectile_Aspect projectileAspect;
        [DI] private Prefab_Service prefabService;
        [DI] private GOPool_Service goPoolService;
        [DI] private Projectile_Service projectileService;
        [DI] private Lightning_Converter converter;

        private PoolableObject CreateObject()
        {
            var prefab = prefabService.GetPrefab(PrefabCategory.Projectiles, "LightningLine");
            var projectilePoolableObject = goPoolService.Get(
                prefab,
                projectileService.container.transform,
                Constants.Pools.ProjectileEffectsDefaultCopacity,
                Constants.Pools.ProjectileEffectsMaxCopacity,
                null,
                null,
                null,
                ActionOnDestroy
            );
            var transform = projectilePoolableObject.transform;
            transform.position = new Vector2(0f, 0f);
            transform.rotation = new Quaternion(0, 0, 0, 0);
            transform.localScale = Vector2.one;

            return projectilePoolableObject;
        }
        
        private void ActionOnDestroy(PoolableObject o) => Projectile_GOPoolUtils.ActionOnDestroy(o);
        
        public int SpawnLightningLine(ProtoPackedEntityWithWorld firstEntity, ref LightningAttribute lightningSource)
        {
            var lightningLinePo = CreateObject();

            var lightningLineEntity = converter.GetEntity(lightningLinePo.gameObject) ?? projectileAspect.World().NewEntity();
            converter.Convert(lightningLinePo.gameObject, lightningLineEntity);
            
            ref var lightning = ref projectileService.GetLightningAttribute(lightningLineEntity);
            lightning.duration = lightningSource.duration;
            lightning.damage = lightningSource.damage;
            lightning.damageReduction = lightningSource.damageReduction;
            lightning.damageInterval = lightningSource.damageInterval;
            lightning.chainReaction = lightningSource.chainReaction;
            lightning.chainReactionRadius = lightningSource.chainReactionRadius;
            
            ref var lightningLine = ref lightningAspect.lightningPool.GetOrAdd(lightningLineEntity);
            lightningLine.started = false;
            lightningLine.chainEntities ??= new ProtoPackedEntityWithWorld[Constants.WeaponEffects.MaxLightningChainReaction];
            lightningLine.chainEntities[0] = firstEntity;
            for (var index = 1; index < Constants.WeaponEffects.MaxLightningChainReaction; index++)
            {
                lightningLine.chainEntities[index] = default;
            }
            lightningLine.length = 1;
            lightningLine.timeRemains = 0f;
            lightningLine.damageIntervalRemains = lightningSource.damageInterval;

            return lightningLineEntity;
        }
    }
}