using System;
using System.Collections.Generic;
using Leopotam.EcsLite;
using td.features.projectiles.attributes;
using td.features.shards.config;
using td.monoBehaviours;
using td.services;
using td.services.ecsConverter;
using td.utils;
using td.utils.ecs;
using UnityEngine;

namespace td.features.projectiles.lightning
{
    public sealed class LightningLineService
    {
        [Inject] private PrefabService prefabService;
        [Inject] private GameObjectPoolService poolService;
        [Inject] private EntityConverters converters;
        [Inject] private ShardsConfig shardsConfig; // todo
        [InjectWorld] private EcsWorld world;
        [InjectWorld(Constants.Worlds.Outer)] private EcsWorld outerWorld;

        private PoolableObject CreateObject()
        {
            var prefab = prefabService.GetPrefab(PrefabCategory.Projectiles, "LightningLine");
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
            transform.position = new Vector2(0f, 0f);
            transform.rotation = new Quaternion(0, 0, 0, 0);
            transform.localScale = Vector2.one;

            return projectilePoolableObject;
        }
        
        public int SpawnLightningLine(EcsPackedEntity firstEntity, ref LightningAttribute lightningSource)
        {
            var lightningLinePo = CreateObject();

            if (!converters.Convert<LightningLine>(lightningLinePo.gameObject, out var lightningLineEntity))
            {
                throw new NullReferenceException($"Failed to convert GameObject {lightningLinePo.gameObject.name}");
            }
            
            ref var lightning = ref world.GetComponent<LightningAttribute>(lightningLineEntity);
            lightning.duration = lightningSource.duration;
            lightning.damage = lightningSource.damage;
            lightning.damageReduction = lightningSource.damageReduction;
            lightning.damageInterval = lightningSource.damageInterval;
            lightning.chainReaction = lightningSource.chainReaction;
            lightning.chainReactionRadius = lightningSource.chainReactionRadius;
            
            ref var lightningLine = ref world.GetComponent<LightningLine>(lightningLineEntity);
            lightningLine.started = false;
            lightningLine.chainEntities ??= new EcsPackedEntity[Constants.WeaponEffects.MaxLightningChainReaction];
            lightningLine.chainEntities[0] = firstEntity;
            for (var index = 1; index < Constants.WeaponEffects.MaxLightningChainReaction; index++)
            {
                lightningLine.chainEntities[index] = default;
            }
            lightningLine.length = 1;
            lightningLine.timeRemains = 0f;
            lightningLine.findNeighborsTimeRemains = 0f;
            lightningLine.damageIntervalRemains = lightningSource.damageInterval;

            return lightningLineEntity;
        }
    }
}