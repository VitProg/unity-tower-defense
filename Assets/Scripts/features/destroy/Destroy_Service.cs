using System.Runtime.CompilerServices;
using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using td.features.destroy.bus;
using td.features.eventBus;
using td.features.goPool;
using td.features.movement;
using td.monoBehaviours;
using td.utils.ecs;
using UnityEngine;

namespace td.features.destroy
{
    public class Destroy_Service
    {
        [DI] private Destroy_Aspect aspect;
        [DI] private Movement_Service movementService;
        [DI] private GOPool_Service goPoolService;
        [DI] private EventBus events;
        
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public bool IsHidden(int entity) => aspect.isHiddenPool.Has(entity);
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public bool IsDisabled(int entity) => aspect.isDisabledPool.Has(entity);
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public bool IsDestroyed(int entity) => aspect.isDestroyedPool.Has(entity);
        
        
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public void SetIsHidden(int entity, bool value, bool changeGOActive = false)
        {
            aspect.isHiddenPool.SetExistence(entity, value);
            if (changeGOActive && movementService.HasGameObject(entity))
            {
                movementService.GetGameObject(entity)?.SetActive(!value);
            }
        }
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public void SetIsDisabled(int entity, bool value) => aspect.isDisabledPool.SetExistence(entity, value);
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public void SetIsDestroyed(int entity, bool value) => aspect.isDestroyedPool.SetExistence(entity, value);
        
        public void SafeRemove(GameObject go)
        {
            var poolableObject = go.GetComponent<PoolableObject>();

            if (poolableObject != null)
            {
                goPoolService.Release(poolableObject);
            }
            else
            {
                Object.Destroy(go);
            }

        }

        public void SafeRemove(ProtoPackedEntityWithWorld packedEntity)
        {
            if (
                packedEntity.Unpack(out var world, out var entity) && 
                movementService.HasGameObject(entity, true)
            )
            {
                var go = movementService.GetGameObject(entity)!;
                var poolableObject = go.GetComponent<PoolableObject>();

                if (poolableObject != null)
                {
                    goPoolService.Release(poolableObject);
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
                        world.DelEntity(entity);
                    }
                    catch
                    {
                        // ignored
                    }
                }
            }
            else
            {
                world.DelEntity(entity);
            }
        }
        
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public void MarkAsRemoved(int entity, ProtoWorld world, bool checkGameObject = true) =>
            MarkAsRemoved(world.PackEntityWithWorld(entity), checkGameObject);
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public void MarkAsRemoved(ProtoPackedEntityWithWorld packedEntity, bool checkGameObject = true)
        {
            if (checkGameObject && movementService.HasGameObject(packedEntity) && packedEntity.Unpack(out var world, out var entity))
            {
                aspect.isDisabledPool.GetOrAdd(entity);
                events.global.Add<Command_Remove>().Entity = world.PackEntityWithWorld(entity);
            }
            else
            {
                packedEntity.DelEntity();
            }
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public void RemoveDestroyedMarks(int entity)
        {
            aspect.isDisabledPool.Del(entity);
            aspect.isDestroyedPool.Del(entity);
        }
        
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public bool IsOnlyOnLevel(int entity) => aspect.isOnlyOnLevelPool.Has(entity);

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public void SetIsOnlyOnLevel(int entity, bool value) => aspect.isOnlyOnLevelPool.SetExistence(entity, value);
    }
}