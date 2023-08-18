using System;
using System.Runtime.CompilerServices;
using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using Leopotam.EcsProto.Unity;
using td.features.fx.types;
using td.utils.ecs;

namespace td.features.fx.subServices
{
    public class FX_EntityModifier_SubService
    {
        [DI(Constants.Worlds.FX)] private FX_Aspect aspect;

        public ref T Add<T>(
            ProtoPackedEntityWithWorld packedEntity,
            float? duration = null 
        ) where T : struct, IEntityModifierFX
        {
            var fxEntity = aspect.World().NewEntity();

            var pool = (ProtoPool<T>)aspect.World().Pool(typeof(T));
            ref var fx = ref pool.Add(fxEntity);
            
            aspect.isEntityModifierPool.Add(fxEntity);
            
            ref var fxDuration = ref aspect.withDurationPool.Add(fxEntity);
            fxDuration.SetDuration(duration);
            fxDuration.remainingTime = fxDuration.duration;
            
            ref var fxTargetEntity = ref aspect.withTargetEntityPool.Add(fxEntity);
            fxTargetEntity.entity = packedEntity;

            // todo add go for this effect
            
            return ref fx;
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public ref T GetOrAdd<T>(
            ProtoPackedEntityWithWorld packedEntity,
            float? duration = null
        ) where T : struct, IEntityModifierFX
        {
            if (Has<T>(packedEntity, out var fxEntity))
            {
                var pool = (ProtoPool<T>)aspect.World().Pool(typeof(T));
                ref var t = ref pool.Get(fxEntity);
                ref var d = ref aspect.withDurationPool.GetOrAdd(fxEntity);
                d.SetDuration(duration);
                d.remainingTime = d.duration;
                return ref t;
            }
            return ref Add<T>(packedEntity, duration);
        }

        public bool Has<T>(ProtoPackedEntityWithWorld packedEntity, out int fxEntity) where T : struct, IEntityModifierFX
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
        
        public ref T Get<T>(ProtoPackedEntityWithWorld packedEntity) where T : struct, IEntityModifierFX
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
            throw new Exception($"Entity {packedEntity} not found in {EditorExtensions.GetCleanTypeName(typeof(T))} pool");
#else
            throw new Exception("Entity not found in pool");
#endif
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public void Remove<T>(ProtoPackedEntityWithWorld packedEntity) where T : struct, IEntityModifierFX
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