using System.Runtime.InteropServices;
using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using Leopotam.Types;
using td.features.destroy;
using td.features.movement;
using td.monoBehaviours;
using td.utils;
using td.utils.di;
using UnityEngine;
#if UNITY_EDITOR
using Leopotam.EcsProto.Unity;
#endif

namespace td.features.ecsConverter
{
    public abstract class BaseEntity_Converter
    {
        public abstract ProtoWorld World(); 
        
        public int? GetEntity(GameObject gameObject)
        {
            if (gameObject.TryGetComponent<EcsEntity>(out var e))
            {
                if (
                    e.packedEntity != null &&
                    e.packedEntity.Value.Unpack(out _, out var entityTest)
                )
                {
                    return entityTest;
                }
            }

            return null;
        }

        protected void Convert(GameObject gameObject, int entity)
        {
            if (gameObject.TryGetComponent<EcsEntity>(out var e))
            {
                if (
                    e.packedEntity != null &&
                    e.packedEntity.Value.Unpack(out var checkWorld, out var entityTest)
                ) {
                    if (checkWorld != World()) throw new InvalidComObjectException("GameObject already linked in another ecs world");
                    if (entityTest != entity) throw new InvalidComObjectException("GameObject already linked with other ecs entity");
                }
                e.packedEntity = World().PackEntityWithWorld(entity);
            }
            else
            {
                gameObject.AddComponent<EcsEntity>().packedEntity = World().PackEntityWithWorld(entity);
            }

            ServiceContainer.Get<Movement_Service>().GetRefGameObject(entity).reference = gameObject;
            ServiceContainer.Get<Destroy_Service>().RemoveDestroyedMarks(entity);
            
            var movementService = ServiceContainer.Get<Movement_Service>();
            ref var transform = ref movementService.GetTransform(entity);
            transform.SetPosition(gameObject.transform.position);
            transform.SetRotation(gameObject.transform.rotation);
            transform.SetScale(gameObject.transform.localScale);
            transform.ClearChangedStatus();
            
#if UNITY_EDITOR && DEBUG
            if (!gameObject.GetComponent<ProtoEntityDebugView>())
            {
                var entityObserver = gameObject.AddComponent<ProtoEntityDebugView>();
                entityObserver.Entity = entity;
                entityObserver.World = World();
            }
#endif
        }
    }
}