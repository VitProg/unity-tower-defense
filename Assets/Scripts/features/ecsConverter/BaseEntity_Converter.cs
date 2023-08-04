using System.Runtime.InteropServices;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.features._common;
using td.monoBehaviours;
using UnityEngine;
#if UNITY_EDITOR
using Leopotam.EcsLite.UnityEditor;
#endif

namespace td.features.ecsConverter
{
    public abstract class BaseEntity_Converter
    {
        private readonly EcsWorldInject world;
        private readonly EcsInject<Common_Service> common;

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
            // if (world == null)
            // {
                // world = DI.GetWorld();
                // refGameObjectPool = world.GetPool<Ref<GameObject>>();
                // isDisabledPool = world.GetPool<IsDisabled>();
                // isDestroyedPool = world.GetPool<IsDestroyed>();
            // }
            
            if (gameObject.TryGetComponent<EcsEntity>(out var e))
            {
                if (
                    e.packedEntity != null &&
                    e.packedEntity.Value.Unpack(out var checkWorld, out var entityTest)
                ) {
                    if (checkWorld != world.Value) throw new InvalidComObjectException("GameObject already linked in another ecs world");
                    if (entityTest != entity) throw new InvalidComObjectException("GameObject already linked with other ecs entity");
                }
                e.packedEntity = world.Value.PackEntityWithWorld(entity);
            }
            else
            {
                gameObject.AddComponent<EcsEntity>().packedEntity = world.Value.PackEntityWithWorld(entity);
            }
            
            common.Value.GetRefGameObject(entity).reference = gameObject;
            common.Value.RemoveDestroyedMarks(entity);
            
            ref var transform = ref common.Value.GetTransform(entity);
            transform.SetPosition(gameObject.transform.position);
            transform.SetRotation(gameObject.transform.rotation);
            transform.SetScale(gameObject.transform.localScale);
            transform.ClearChangedStatus();
            
#if UNITY_EDITOR && DEBUG
            // if (!gameObject.GetComponent<EcsComponentsInfo>()) gameObject.AddComponent<EcsComponentsInfo>();
            if (!gameObject.GetComponent<EcsEntityDebugView>())
            {
                var entityObserver = gameObject.AddComponent<EcsEntityDebugView>();
                entityObserver.Entity = entity;
                entityObserver.World = world.Value;
            }
#endif
        }
    }
}