using System;
using System.Runtime.CompilerServices;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.features._common;
using td.features.fx.events;
using td.features.fx.types;
using td.utils.ecs;
using Unity.Mathematics;
using UnityEngine;

namespace td.features.fx.subServices
{
    public class FX_EntityFallow_SubService
    {
        private readonly EcsWorldInject fxWorld = Constants.Worlds.FX;
        private readonly EcsInject<FX_Pools> pools;
        private readonly EcsInject<Common_Service> common;
        private readonly EcsInject<IEventBus> events;

        public ref T Add<T>(
            EcsPackedEntityWithWorld packedEntity,
            float? duration = null,
            Vector2? position = null,
            Vector2? scale = null,
            Quaternion? rotation = null
        ) where T : struct, IEntityFallowFX
        {
            var fxEntity = fxWorld.Value.NewEntity();

            var pool = fxWorld.Value.GetPool<T>();
            
            ref var fx = ref pool.Add(fxEntity);

            pools.Value.isEntityFallowPool.Value.Add(fxEntity);

            ref var p = ref pools.Value.withTransformPool.Value.Add(fxEntity);
            
            p.SetPosition(position ?? Vector2.zero);
            p.SetScale(scale ?? Vector2.one);
            p.SetRotation(rotation ?? quaternion.identity);
            
            if (common.Value.HasTransform(packedEntity))
            {
                ref var t = ref common.Value.GetTransform(packedEntity);
                p.SetPosition(position ?? t.position);
            }
            else if (common.Value.HasGameObject(packedEntity, true))
            {
                var go = common.Value.GetGameObject(packedEntity)!;
                p.SetPosition(position ?? go.transform.position);
            }

            ref var fxDuration = ref pools.Value.withDurationPool.Value.Add(fxEntity);
            fxDuration.SetDuration(duration);
            fxDuration.remainingTime = fxDuration.duration;

            ref var fxTargetEntity = ref pools.Value.withTargetEntityPool.Value.Add(fxEntity);
            fxTargetEntity.entity = packedEntity;

            // todo add go for this effect

            events.Value.Entity.Add<FX_Event_EnemyFallow_Spawned<T>>(fxEntity, fxWorld.Value);

            return ref fx;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T GetOrAdd<T>(
            EcsPackedEntityWithWorld packedEntity,
            float? duration = null,
            Vector2? position = null,
            Vector2? scale = null,
            Quaternion? rotation = null
        ) where T : struct, IEntityFallowFX
        {
            if (Has<T>(packedEntity, out var fxEntity))
            {
                ref var fx = ref fxWorld.Value.GetPool<T>().Get(fxEntity);
                ref var d = ref pools.Value.withDurationPool.Value.GetOrAdd(fxEntity);
                d.SetDuration(duration);
                d.remainingTime = d.duration;

                if (position.HasValue || scale.HasValue || rotation.HasValue)
                {
                    ref var t = ref pools.Value.withTransformPool.Value.GetOrAdd(fxEntity);
                    t.position = position ?? t.position;
                    t.scale = scale ?? t.scale;
                    t.rotation = rotation ?? t.rotation;
                }
                return ref fx;
            }

            return ref Add<T>(packedEntity, duration, position, scale, rotation);
        }


        public bool Has<T>(int fxEntity) where T : struct, IEntityFallowFX
        {
            var pool = fxWorld.Value.GetPool<T>();
            return pool.Has(fxEntity);
        }

        public bool Has<T>(EcsPackedEntityWithWorld packedEntity, out int fxEntity) where T : struct, IEntityFallowFX
        {
            fxEntity = -1;
            var pool = fxWorld.Value.GetPool<T>();
            foreach (var entity in pools.Value.entityModifierFilter.Value)
            {
                if (pool.Has(entity) && packedEntity.EqualsTo(pools.Value.entityModifierFilter.Pools.Inc2.Get(entity).entity))
                {
                    fxEntity = entity;
                    return true;
                }
            }

            return false;
        }

        public ref T Get<T>(int fxEntity) where T : struct, IEntityFallowFX
        {
            var pool = fxWorld.Value.GetPool<T>();
            return ref pool.Get(fxEntity);
        }

        public ref T Get<T>(EcsPackedEntityWithWorld packedEntity) where T : struct, IEntityFallowFX
        {
            var pool = fxWorld.Value.GetPool<T>();
            foreach (var fxEntity in pools.Value.entityModifierFilter.Value)
            {
                ref var fxType = ref pools.Value.entityModifierFilter.Pools.Inc2.Get(fxEntity);
                if (pool.Has(fxEntity) && packedEntity.EqualsTo(fxType.entity) && fxType.entity.Unpack(out _, out _))
                {
                    return ref pool.Get(fxEntity);
                }
            }

            throw new NullReferenceException($"Entity {packedEntity} not found in {typeof(T).Name} pool");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Remove<T>(int fxEntity) where T : struct, IEntityFallowFX
        {
            if (Has<T>(fxEntity))
            {
                pools.Value.needRemovePool.Value.SafeAdd(fxEntity);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Remove<T>(EcsPackedEntityWithWorld packedEntity) where T : struct, IEntityFallowFX
        {
            if (Has<T>(packedEntity, out var fxEntity))
            {
                pools.Value.needRemovePool.Value.SafeAdd(fxEntity);
            }
        }

        public void RemoveAll(EcsPackedEntityWithWorld packedEntity)
        {
            foreach (var fxEntity in pools.Value.entityModifierFilter.Value)
            {
                if (packedEntity.EqualsTo(pools.Value.entityModifierFilter.Pools.Inc2.Get(fxEntity).entity))
                {
                    pools.Value.needRemovePool.Value.SafeAdd(fxEntity);
                }
            }
        }
    }
}