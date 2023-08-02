using System;
using System.Runtime.CompilerServices;
using Leopotam.EcsLite;
// #if ENABLE_IL2CPP
// using System;
// using Unity.IL2CPP.CompilerServices;
// #endif

namespace td.utils.ecs
{
// #if ENABLE_IL2CPP
//     [Il2CppSetOption (Option.NullChecks, false)]
//     [Il2CppSetOption (Option.ArrayBoundsChecks, false)]
// #endif
    public static class WorldExtensions
    {
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public static bool Has<T>(this EcsPool<T> pool, EcsPackedEntity packedEntity) where T : struct =>
            packedEntity.Unpack(pool.GetWorld(), out var entity) && pool.Has(entity);
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public static bool Has<T>(this EcsPool<T> pool, EcsPackedEntityWithWorld packedEntity) where T : struct =>
            packedEntity.Unpack(out _, out var entity) && pool.Has(entity);

        public static ref T GetOrAdd<T>(this EcsPool<T> pool, EcsPackedEntity packedEntity) where T : struct
        {
            if (!packedEntity.Unpack(pool.GetWorld(), out var entity)) throw new Exception ($"Cant get \"{typeof (T).Name}\" component - not attached.");
            return ref pool.GetOrAdd<T>(entity);
        }
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public static ref T GetOrAdd<T>(this EcsPool<T> pool, EcsPackedEntityWithWorld packedEntity) where T : struct
        {
            if (!packedEntity.Unpack(out _, out var entity)) throw new Exception ($"Cant get \"{typeof (T).Name}\" component - not attached.");
            return ref pool.GetOrAdd<T>(entity);
        }
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public static ref T GetOrAdd<T>(this EcsPool<T> pool, int entity) where T : struct
        {
            if (pool.Has(entity)) return ref pool.Get(entity);
            return ref pool.Add(entity);
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public static ref T SafeAdd<T>(this EcsPool<T> pool, EcsPackedEntity packedEntity) where T : struct => ref pool.GetOrAdd(packedEntity);
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public static ref T SafeAdd<T>(this EcsPool<T> pool, EcsPackedEntityWithWorld packedEntity) where T : struct => ref pool.GetOrAdd(packedEntity);
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public static ref T SafeAdd<T>(this EcsPool<T> pool, int entity) where T : struct => ref pool.GetOrAdd(entity);

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public static void SetExistence<T>(this EcsPool<T> pool, int entity, bool existence) where T : struct
        {
            if (existence) SafeAdd(pool, entity);
            else SafeDel(pool, entity);
        }
        
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public static void SafeDel<T>(this EcsPool<T> pool, int entity) where T : struct
        {
            if (pool.Has(entity)) pool.Del(entity);
        }
        
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public static bool IsEmpty(this EcsFilter filter) => filter.GetEntitiesCount() <= 0;
        
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public static bool TryGetFirst(this EcsFilter filter, out int firstEntity)
        {
            if (IsEmpty(filter))
            {
                firstEntity = 1;
                return false;
            }
            firstEntity = filter.GetRawEntities()[0];
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetRawEntity(this EcsFilter filter, int index) => filter.GetRawEntities()[index];
    }
}