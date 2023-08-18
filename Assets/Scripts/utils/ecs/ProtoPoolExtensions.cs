using System;
using System.Runtime.CompilerServices;
using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;

namespace td.utils.ecs
{
    public static class ProtoPoolExtensions
    {
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public static ref T GetOrAdd<T> (this ProtoPool<T> pool, int entity) where T : struct {
            var added = !pool.Has (entity);
            return ref added ? ref pool.Add (entity) : ref pool.Get (entity);
        }     
        
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public static ref T GetOrAdd<T> (this ProtoPool<T> pool, ProtoPackedEntity packedEntity) where T : struct {
            var check = packedEntity.Unpack(pool.World(), out var entity);
#if UNITY_EDITOR
            if (!check) throw new System.Exception("Can't unpack packed entity");
#endif
            return ref GetOrAdd(pool, entity);
        }
        
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public static ref T GetOrAdd<T> (this ProtoPool<T> pool, ProtoPackedEntityWithWorld packedEntity) where T : struct {
            var check = packedEntity.Unpack(out var w, out var entity);
#if UNITY_EDITOR
            if (!check) throw new System.Exception("Can't unpack packed entity");
            if (!pool.World().Equals(w)) throw new Exception("Can't unpack packed entity with different world");
#endif
            return ref GetOrAdd(pool, entity);
        }
        
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public static void SetExistence<T>(this ProtoPool<T> pool, int entity, bool existence) where T : struct
        {
            if (existence) GetOrAdd(pool, entity);
            else pool.Del(entity);
        }
        
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public static bool Has<T>(this ProtoPool<T> pool, ProtoPackedEntity packedEntity) where T : struct =>
            packedEntity.Unpack(pool.World(), out var entity) && pool.Has(entity);
        
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public static bool Has<T>(this ProtoPool<T> pool, ProtoPackedEntityWithWorld packedEntity) where T : struct =>
            packedEntity.Unpack(out _, out var entity) && pool.Has(entity);

    }
}