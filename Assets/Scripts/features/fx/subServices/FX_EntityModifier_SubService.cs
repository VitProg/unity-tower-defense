using System;
using System.Runtime.CompilerServices;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.features.fx.types;
using td.utils.ecs;

namespace td.features.fx.subServices
{
    public class FX_EntityModifier_SubService
    {
        private readonly EcsWorldInject fxWorld = Constants.Worlds.FX;
        private readonly EcsInject<FX_Pools> pools;

        public ref T Add<T>(
            EcsPackedEntityWithWorld packedEntity,
            float? duration = null 
        ) where T : struct, IEntityModifierFX
        {
            var fxEntity = fxWorld.Value.NewEntity();

            ref var fx = ref fxWorld.Value.GetPool<T>().Add(fxEntity);
            
            pools.Value.isEntityModifierPool.Value.Add(fxEntity);
            
            ref var fxDuration = ref pools.Value.withDurationPool.Value.Add(fxEntity);
            fxDuration.SetDuration(duration);
            fxDuration.remainingTime = fxDuration.duration;
            
            ref var fxTargetEntity = ref pools.Value.withTargetEntityPool.Value.Add(fxEntity);
            fxTargetEntity.entity = packedEntity;

            // todo add go for this effect
            
            return ref fx;
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public ref T GetOrAdd<T>(
            EcsPackedEntityWithWorld packedEntity,
            float? duration = null 
        ) where T : struct, IEntityModifierFX
        {
            if (Has<T>(packedEntity, out var fxEntity))
            {
                ref var t = ref fxWorld.Value.GetPool<T>().Get(fxEntity);
                ref var d = ref pools.Value.withDurationPool.Value.GetOrAdd(fxEntity);
                d.SetDuration(duration);
                d.remainingTime = d.duration;
                return ref t;
            }
            return ref Add<T>(packedEntity, duration);
        }

        public bool Has<T>(EcsPackedEntityWithWorld packedEntity, out int fxEntity) where T : struct, IEntityModifierFX
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
        
        public ref T Get<T>(EcsPackedEntityWithWorld packedEntity) where T : struct, IEntityModifierFX
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

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public void Remove<T>(EcsPackedEntityWithWorld packedEntity) where T : struct, IEntityModifierFX
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