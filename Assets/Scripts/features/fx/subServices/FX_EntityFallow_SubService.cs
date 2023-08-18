using System;
using System.Runtime.CompilerServices;
using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using Leopotam.EcsProto.Unity;
using td.features.eventBus;
using td.features.fx.events;
using td.features.fx.types;
using td.features.movement;
using td.utils.ecs;
using Unity.Mathematics;
using UnityEngine;

namespace td.features.fx.subServices
{
    public class FX_EntityFallow_SubService
    {
        // private readonly EcsWorldInject fxWorld = Constants.Worlds.FX;
        [DI(Constants.Worlds.FX)] private FX_Aspect aspect;
        [DI] private Movement_Service movementService;
        [DI] private EventBus events;

        public ref T Add<T>(
            ProtoPackedEntityWithWorld packedEntity,
            float? duration = null,
            Vector2? position = null,
            Vector2? scale = null,
            Quaternion? rotation = null
        ) where T : struct, IEntityFallowFX
        {
            var fxEntity = aspect.World().NewEntity();
            var pool = (ProtoPool<T>)aspect.World().Pool(typeof(T));
            
            ref var fx = ref pool.Add(fxEntity);

            aspect.isEntityFallowPool.Add(fxEntity);

            ref var p = ref aspect.withTransformPool.Add(fxEntity);
            
            p.SetPosition(position ?? Vector2.zero);
            p.SetScale(scale ?? Vector2.one);
            p.SetRotation(rotation ?? quaternion.identity);
            
            if (movementService.HasTransform(packedEntity))
            {
                ref var t = ref movementService.GetTransform(packedEntity);
                p.SetPosition(position ?? t.position);
            }
            else if (movementService.HasGameObject(packedEntity, true))
            {
                var go = movementService.GetGameObject(packedEntity)!;
                p.SetPosition(position ?? go.transform.position);
            }

            ref var fxDuration = ref aspect.withDurationPool.Add(fxEntity);
            fxDuration.SetDuration(duration);
            fxDuration.remainingTime = fxDuration.duration;

            ref var fxTargetEntity = ref aspect.withTargetEntityPool.Add(fxEntity);
            fxTargetEntity.entity = packedEntity;

            // todo add go for this effect

            events.global.Add<FX_Event_EnemyFallow_Spawned<T>>().Entity = aspect.World().PackEntityWithWorld(fxEntity);

            return ref fx;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T GetOrAdd<T>(
            ProtoPackedEntityWithWorld packedEntity,
            float? duration = null,
            Vector2? position = null,
            Vector2? scale = null,
            Quaternion? rotation = null
        ) where T : struct, IEntityFallowFX
        {
            if (Has<T>(packedEntity, out var fxEntity))
            {
                var pool = (ProtoPool<T>)aspect.World().Pool(typeof(T));
                ref var fx = ref pool.Get(fxEntity);
                
                ref var d = ref aspect.withDurationPool.GetOrAdd(fxEntity);
                d.SetDuration(duration);
                d.remainingTime = d.duration;

                if (position.HasValue || scale.HasValue || rotation.HasValue)
                {
                    ref var t = ref aspect.withTransformPool.GetOrAdd(fxEntity);
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
            var pool = (ProtoPool<T>)aspect.World().Pool(typeof(T));
            return pool.Has(fxEntity);
        }

        public bool Has<T>(ProtoPackedEntityWithWorld packedEntity, out int fxEntity) where T : struct, IEntityFallowFX
        {
            fxEntity = -1;
            var pool = (ProtoPool<T>)aspect.World().Pool(typeof(T));
            foreach (var entity in aspect.itEntityModifier)
            {
                if (pool.Has(entity) && packedEntity.EqualsTo(aspect.withTargetEntityPool.Get(entity).entity))
                {
                    fxEntity = entity;
                    return true;
                }
            }

            return false;
        }

        public ref T Get<T>(int fxEntity) where T : struct, IEntityFallowFX
        {
            var pool = (ProtoPool<T>)aspect.World().Pool(typeof(T));
            return ref pool.Get(fxEntity);
        }

        public ref T Get<T>(ProtoPackedEntityWithWorld packedEntity) where T : struct, IEntityFallowFX
        {
            var pool = (ProtoPool<T>)aspect.World().Pool(typeof(T));
            foreach (var fxEntity in aspect.itEntityModifier)
            {
                ref var target = ref aspect.withTargetEntityPool.Get(fxEntity);
                if (pool.Has(fxEntity) && packedEntity.EqualsTo(target.entity) && target.entity.Unpack(out _, out _))
                {
                    return ref pool.Get(fxEntity);
                }
            }
#if UNITY_EDITOR
            throw new NullReferenceException($"Entity {packedEntity} not found in {EditorExtensions.GetCleanTypeName(typeof(T))} pool");
#else
            throw new Exception("Entity not found in pool");
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Remove<T>(int fxEntity) where T : struct, IEntityFallowFX
        {
            if (Has<T>(fxEntity))
            {
                aspect.needRemovePool.GetOrAdd(fxEntity);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Remove<T>(ProtoPackedEntityWithWorld packedEntity) where T : struct, IEntityFallowFX
        {
            if (Has<T>(packedEntity, out var fxEntity))
            {
                aspect.needRemovePool.GetOrAdd(fxEntity);
            }
        }

        public void RemoveAll(ProtoPackedEntityWithWorld packedEntity)
        {
            foreach (var fxEntity in aspect.itEntityModifier)
            {
                if (packedEntity.EqualsTo(aspect.withTargetEntityPool.Get(fxEntity).entity))
                {
                    aspect.needRemovePool.GetOrAdd(fxEntity);
                }
            }
        }
    }
}