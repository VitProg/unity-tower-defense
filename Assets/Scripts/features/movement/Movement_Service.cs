using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Leopotam.EcsProto.QoL;
using td.features._common.components;
using td.features.movement.components;
using td.features.movement.flags;
using td.utils.ecs;
using UnityEngine;

namespace td.features.movement
{
    public class Movement_Service
    {
        [DI] private Movement_Aspect aspect;
        
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public bool HasMovement(int entity) => aspect.movementPool.Has(entity);
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public ref Movement GetMovement(int entity) => ref aspect.movementPool.GetOrAdd(entity);
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public void RemoveMovement(int entity) => aspect.movementPool.Del(entity);

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public bool HasSmoothRotation(int entity) => aspect.isSmoothRotationPool.Has(entity);
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public ref IsSmoothRotation GetSmoothRotation(int entity) => ref aspect.isSmoothRotationPool.GetOrAdd(entity);
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public void RemoveSmoothRotation(int entity) => aspect.isSmoothRotationPool.Del(entity);
        
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public bool IsFreezed(int entity) => aspect.isFreezedPool.Has(entity);
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public void SetIsFreezed(int entity, bool value) => aspect.isFreezedPool.SetExistence(entity, value);
        
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public bool HasTransform(ProtoPackedEntityWithWorld packedEntity) => aspect.objectTransformPool.Has(packedEntity);
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public bool HasTransform(ProtoPackedEntity packedEntity) => aspect.objectTransformPool.Has(packedEntity);
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public bool HasTransform(int entity) => aspect.objectTransformPool.Has(entity);

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public ref ObjectTransform GetTransform(ProtoPackedEntityWithWorld packedEntity) => ref aspect.objectTransformPool.GetOrAdd(packedEntity);
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public ref ObjectTransform GetTransform(ProtoPackedEntity packedEntity) => ref aspect.objectTransformPool.GetOrAdd(packedEntity);
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public ref ObjectTransform GetTransform(int entity) => ref aspect.objectTransformPool.GetOrAdd(entity);

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public bool HasCustomMovement(int entity) => aspect.isCustomMovementPool.Has(entity);
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public void SetCustomMovement(int entity, bool value) => aspect.isCustomMovementPool.SetExistence(entity, value);

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
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasTargetBody(int entity) => aspect.refTargetBodyPool.Has(entity);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref RefTargetBody GetRefTargetBody(int entity) => ref aspect.refTargetBodyPool.GetOrAdd(entity);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GameObject GetTargetBodyGO(int entity) => aspect.refTargetBodyPool.Get(entity).targetBody;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Transform GetTargetBodyTransform(int entity) => aspect.refTargetBodyPool.Get(entity).targetBody.transform;

    }
}