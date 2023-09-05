using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using JetBrains.Annotations;
using Leopotam.EcsProto.QoL;
using td.features._common.components;
using td.utils.ecs;

namespace td.features._common
{
    public class Common_Service
    {
        [DI] private Common_Aspect aspect;
        
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public ref Ref<GameObject> GetRefGameObject(int entity) => ref aspect.refGoPool.GetOrAdd(entity);
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public bool HasGameObject(ProtoPackedEntityWithWorld packedEntity, bool checkRef = false, bool checkRefActive = false) =>
            packedEntity.Unpack(out var w, out var entity) && HasGameObject(entity, checkRef, checkRefActive);
        public bool HasGameObject(int entity, bool checkRef = false, bool checkRefActive = false)
        {
            if (!aspect.refGoPool.Has(entity)) return false;
            if (!checkRef) return true;
            var go = GetGameObject(entity);
            return checkRefActive ? go && go.activeSelf : go;
        }

        [CanBeNull][MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GameObject GetGameObject(ProtoPackedEntityWithWorld packedEntity)
        {
            var check = packedEntity.Unpack(out var w, out var entity);
#if UNITY_EDITOR
            if (!check) throw new NullReferenceException($"Can't unpack entity {packedEntity}");
#endif
            return GetGameObject(entity);
        }
        [CanBeNull][MethodImpl (MethodImplOptions.AggressiveInlining)]
        public GameObject GetGameObject(int entity)
        {
#if DEBUG && !LEOECSLITE_NO_SANITIZE_CHECKS
            if (!aspect.refGoPool.Has(entity)) throw new NullReferenceException("Entity don't have referense to GameObject. Use HasGameObject method before");
#endif
            return aspect.refGoPool.Get(entity).reference;
        }
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public Transform GetGOTransform(int entity) => GetGameObject(entity)!.transform;
    }
}