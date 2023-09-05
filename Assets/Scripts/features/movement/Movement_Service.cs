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
        
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasTargetBody(int entity) => aspect.refTargetBodyPool.Has(entity);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref RefTargetBody GetRefTargetBody(int entity) => ref aspect.refTargetBodyPool.GetOrAdd(entity);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GameObject GetTargetBodyGO(int entity) => aspect.refTargetBodyPool.Get(entity).targetBody;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Transform GetTargetBodyTransform(int entity) => aspect.refTargetBodyPool.Get(entity).targetBody.transform;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasTargetPoint(int entity) => aspect.targetPointPool.Has(entity);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref TargetPoint GetTargetPointPool(int entity) => ref aspect.targetPointPool.GetOrAdd(entity);
    }
}