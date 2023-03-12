using System.Collections.Generic;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.common.ecs;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace td.utils.ecs
{
    public static class EcsEventUtils
    {
        public static void Send<T>(IEcsSystems systems) where T : struct =>
            Send<T>(systems.GetWorld(Constants.Ecs.EventWorldName));
        
        public static void Send<T>(EcsWorld eventsWorld) where T : struct
        {
            Send(eventsWorld, new T());
        }

        public static void Send<T>(IEcsSystems systems, T eventData) where T : struct =>
            Send(systems.GetWorld(Constants.Ecs.EventWorldName), eventData);
        
        public static void Send<T>(EcsWorld eventsWorld, T eventData) where T : struct
        {
            var eventEntity = eventsWorld.NewEntity();
            var pool = eventsWorld.GetPool<T>();
            pool.Add(eventEntity) = eventData;

#if DEBUG || UNITY_EDITOR
            if (eventData is IEcsDoNotDebugLog<T> == false)
            {
                Debug.Log($" -> {typeof(T)} {JsonUtility.ToJson(eventData)}");
            }
#endif
        }

        public static void CleanupEvent<T>(EcsWorld eventsWorld) where T : struct
        {
            var entities = eventsWorld.Filter<T>().End();
            foreach (var entity in entities)
            {
                eventsWorld.DelEntity(entity);
            }
        }

        public static void CleanupEvent<T>(IEcsSystems systems, EcsFilterInject<T> filter) where T : struct, IEcsInclude =>
            CleanupEvent(systems.GetWorld(Constants.Ecs.EventWorldName), filter);
        
        public static void CleanupEvent<T>(EcsWorld eventsWorld, EcsFilterInject<T> filter) where T : struct, IEcsInclude
        {
            foreach (var entity in filter.Value)
            {
                eventsWorld.DelEntity(entity);
            }
        }

        public static int? Single<T>(EcsFilterInject<T> filter) where T : struct, IEcsInclude
        {
            foreach (var entity in filter.Value)
            {
                return entity;
            }

            return null;
        }
    }
}