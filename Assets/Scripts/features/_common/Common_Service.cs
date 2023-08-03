using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.features._common.bus;
using td.features._common.components;
using td.features._common.flags;
using td.features.goPool;
using td.monoBehaviours;
using td.utils.ecs;
using UnityEngine;
using Object = UnityEngine.Object;

namespace td.features._common
{
    public class Common_Service
    {
        private readonly EcsInject<Common_Pools> pools;
        private readonly EcsInject<IEventBus> events; // todo ??
        private readonly EcsInject<GameObjectPool_Service> poolServise;
        private readonly EcsWorldInject world = default;
        private readonly EcsWorldInject eventsWorld = Constants.Worlds.EventBus;


        public ref Ref<GameObject> GetRefGameObject(int entity) => ref pools.Value.refGoPool.Value.GetOrAdd(entity);
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public bool HasGameObject(EcsPackedEntity packedEntity, bool checkRef = false, bool checkRefActive = false) =>
            packedEntity.Unpack(world.Value, out var entity) && HasGameObject(entity, checkRef, checkRefActive);
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public bool HasGameObject(EcsPackedEntityWithWorld packedEntity, bool checkRef = false, bool checkRefActive = false) =>
            packedEntity.Unpack(out var w, out var entity) && w == world.Value && HasGameObject(entity, checkRef, checkRefActive);
        public bool HasGameObject(int entity, bool checkRef = false, bool checkRefActive = false)
        {
            if (!pools.Value.refGoPool.Value.Has(entity)) return false;
            if (!checkRef) return true;
            var go = GetGameObject(entity);
            return checkRefActive ? go && go.activeSelf : go;
        }

        [CanBeNull][MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GameObject GetGameObject(EcsPackedEntity packedEntity)
        {
            if (!packedEntity.Unpack(world.Value, out var entity)) throw new NullReferenceException($"Can't unpack entity {packedEntity}");
            return GetGameObject(entity);
        }
        [CanBeNull][MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GameObject GetGameObject(EcsPackedEntityWithWorld packedEntity)
        {
            if (!packedEntity.Unpack(out var w, out var entity)) throw new NullReferenceException($"Can't unpack entity {packedEntity}");
            if (w != world.Value) throw new Exception($"Can't unpack entity {packedEntity}");
            return GetGameObject(entity);
        }
        [CanBeNull][MethodImpl (MethodImplOptions.AggressiveInlining)]
        public GameObject GetGameObject(int entity)
        {
#if DEBUG && !LEOECSLITE_NO_SANITIZE_CHECKS
            if (!pools.Value.refGoPool.Value.Has(entity)) throw new NullReferenceException("Entity don't have referense to GameObject. Use HasGameObject method before");
#endif
            return pools.Value.refGoPool.Value.Get(entity).reference;
        }
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public Transform GetGOTransform(int entity) => GetGameObject(entity)!.transform;
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public Vector2 GetGOPosition(int entity) => GetGOTransform(entity).position;
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public Vector3 GetGO3DPosition(int entity) => GetGOTransform(entity).position;
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public Quaternion GetGORotation(int entity) => GetGOTransform(entity).rotation;
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public Vector2 GetGOScale(int entity) => GetGOTransform(entity).localScale;


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasTargetBody(int entity) => pools.Value.refTargetBodyPool.Value.Has(entity);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref RefTargetBody GetRefTargetBody(int entity) => ref pools.Value.refTargetBodyPool.Value.GetOrAdd(entity);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GameObject GetTargetBodyGO(int entity) => pools.Value.refTargetBodyPool.Value.Get(entity).targetBody;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Transform GetTargetBodyTransform(int entity) => pools.Value.refTargetBodyPool.Value.Get(entity).targetBody.transform;
        
        
        
        public void SafeDelete(int entity, bool checkGameObject = true)
        {
            if (checkGameObject && pools.Value.refGoPool.Value.Has(entity))
            {
                pools.Value.isDisabledPool.Value.SafeAdd(entity);
                events.Value.Entity.Add<Command_Remove>(entity, world.Value);
            }
            else
            {
                world.Value.DelEntity(entity);
            }
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public void RemoveDestroyedMarks(int entity)
        {
            pools.Value.isDisabledPool.Value.SafeDel(entity);
            pools.Value.isDestroyedPool.Value.SafeDel(entity);
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public void SetGroupSystemState(string groupName, bool enabled)
        {
            var entity = eventsWorld.Value.NewEntity ();
            ref var evt = ref pools.Value.ecsGroupSystemStatePool.Value.Add(entity);
            evt.Name = groupName;
            evt.State = enabled;
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public bool HasMovement(int entity) => pools.Value.movementPool.Value.Has(entity);
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public ref MovementToTarget GetMovement(int entity) => ref pools.Value.movementPool.Value.GetOrAdd(entity);
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public void RemoveMovement(int entity) => pools.Value.movementPool.Value.SafeDel(entity);

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public bool HasSmoothRotation(int entity) => pools.Value.smoothRotationCommandPool.Value.Has(entity);
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public ref IsSmoothRotation GetSmoothRotation(int entity) => ref pools.Value.smoothRotationCommandPool.Value.GetOrAdd(entity);
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public void RemoveSmoothRotation(int entity) => pools.Value.smoothRotationCommandPool.Value.SafeDel(entity);

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public bool IsHidden(int entity) => pools.Value.isHiddenPool.Value.Has(entity);
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public bool IsDisabled(int entity) => pools.Value.isDisabledPool.Value.Has(entity);
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public bool IsDestroyed(int entity) => pools.Value.isDestroyedPool.Value.Has(entity);
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public bool IsFreezed(int entity) => pools.Value.isFreezedPool.Value.Has(entity);
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public bool IsOnlyOnLevel(int entity) => pools.Value.onlyOnLevelPool.Value.Has(entity);

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public void SetIsHidden(int entity, bool value, bool changeGOActive = false)
        {
            pools.Value.isHiddenPool.Value.SetExistence(entity, value);
            if (changeGOActive && HasGameObject(entity))GetGameObject(entity)?.SetActive(!value);
        }
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public void SetIsDisabled(int entity, bool value) => pools.Value.isDisabledPool.Value.SetExistence(entity, value);
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public void SetIsDestroyed(int entity, bool value) => pools.Value.isDestroyedPool.Value.SetExistence(entity, value);
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public void SetIsFreezed(int entity, bool value) => pools.Value.isFreezedPool.Value.SetExistence(entity, value);
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public void SetIsOnlyOnLevel(int entity, bool value) => pools.Value.onlyOnLevelPool.Value.SetExistence(entity, value);

        public void RemoveImmediately(int entity)
        {
            if (HasGameObject(entity, true))
            {
                var go = GetGameObject(entity)!;
                var poolableObject = go.GetComponent<PoolableObject>();

                if (poolableObject != null)
                {
                    poolServise.Value.Release(poolableObject);
                    try
                    {
                        SetIsDisabled(entity, true);
                        SetIsDestroyed(entity, true);
                    }
                    catch
                    {
                        // ignored
                    }
                }
                else
                {
                    try
                    {
                        Object.Destroy(go);
                        world.Value.DelEntity(entity);
                    }
                    catch
                    {
                        // ignored
                    }
                }
            }
            else
            {
                world.Value.DelEntity(entity);
            }
        }
        public void RemoveImmediately(EcsPackedEntity packedEntity)
        {
            if (packedEntity.Unpack(world.Value, out var entity)) RemoveImmediately(entity);
        }
        public void RemoveImmediately(EcsPackedEntityWithWorld packedEntity)
        {
            if (packedEntity.Unpack(out _, out var entity)) RemoveImmediately(entity);
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public bool HasTransform(EcsPackedEntityWithWorld packedEntity) => pools.Value.objectTransformPool.Value.Has(packedEntity);
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public bool HasTransform(EcsPackedEntity packedEntity) => pools.Value.objectTransformPool.Value.Has(packedEntity);
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public bool HasTransform(int entity) => pools.Value.objectTransformPool.Value.Has(entity);

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public ref ObjectTransform GetTransform(EcsPackedEntityWithWorld packedEntity) => ref pools.Value.objectTransformPool.Value.GetOrAdd(packedEntity);
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public ref ObjectTransform GetTransform(EcsPackedEntity packedEntity) => ref pools.Value.objectTransformPool.Value.GetOrAdd(packedEntity);
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public ref ObjectTransform GetTransform(int entity) => ref pools.Value.objectTransformPool.Value.GetOrAdd(entity);
        
        public bool HasCustomMovement(int entity) => pools.Value.customMovementPool.Value.Has(entity);

        public void SetCustomMovement(int entity, bool value) => pools.Value.customMovementPool.Value.SetExistence(entity, value);
    }
}