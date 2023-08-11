using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Leopotam.EcsProto.QoL;
using td.features._common;
using td.features.goPool;
using td.features.prefab;
using td.features.projectile.attributes;
using td.features.shard;
using td.monoBehaviours;
using td.utils;
using UnityEngine;

namespace td.features.projectile.lightning
{
    public sealed class Lightning_Service
    {
        private readonly EcsInject<Prefab_Service> prefabService;
        private readonly EcsInject<GOPool_Service> goPoolService;
        private readonly EcsInject<Projectile_Service> projectileService;
        private readonly EcsInject<Lightning_Converter> converter;
        private readonly EcsInject<ShardsConfig> shardsConfig; // todo
        private readonly EcsWorldInject world;

        private PoolableObject CreateObject()
        {
            var prefab = prefabService.Value.GetPrefab(PrefabCategory.Projectiles, "LightningLine");
            var projectilePoolableObject = goPoolService.Value.Get(
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
            transform.position = new Vector2(0f, 0f);
            transform.rotation = new Quaternion(0, 0, 0, 0);
            transform.localScale = Vector2.one;

            return projectilePoolableObject;
        }
        
        private void ActionOnDestroy(PoolableObject o) => EcsPoolUtils.ActionOnDestroy(o);
        
        public int SpawnLightningLine(ProtoPackedEntityWithWorld firstEntity, ref LightningAttribute lightningSource)
        {
            var lightningLinePo = CreateObject();

            var lightningLineEntity = converter.Value.GetEntity(lightningLinePo.gameObject) ?? world.Value.NewEntity();
            converter.Value.Convert(lightningLinePo.gameObject, lightningLineEntity);
            
            ref var lightning = ref projectileService.Value.GetLightningAttribute(lightningLineEntity);
            lightning.duration = lightningSource.duration;
            lightning.damage = lightningSource.damage;
            lightning.damageReduction = lightningSource.damageReduction;
            lightning.damageInterval = lightningSource.damageInterval;
            lightning.chainReaction = lightningSource.chainReaction;
            lightning.chainReactionRadius = lightningSource.chainReactionRadius;
            
            ref var lightningLine = ref projectileService.Value.GetLightningLine(lightningLineEntity);
            lightningLine.started = false;
            lightningLine.chainEntities ??= new EcsPackedEntity[Constants.WeaponEffects.MaxLightningChainReaction];
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