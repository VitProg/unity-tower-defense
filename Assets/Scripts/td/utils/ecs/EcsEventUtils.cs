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
        #region SEND

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

        #endregion

        #region SendSingle

        
        public static void SendSingle<T>(IEcsSystems systems, bool rewrite = true) where T : struct =>
            SendSingle<T>(systems.GetWorld(Constants.Ecs.EventWorldName));

        public static void SendSingle<T>(EcsWorld eventsWorld, bool rewrite = true) where T : struct
        {
            Send(eventsWorld, new T());
        }

        public static void SendSingle<T>(IEcsSystems systems, T eventData, bool rewrite = true) where T : struct =>
            SendSingle(systems.GetWorld(Constants.Ecs.EventWorldName), eventData);
        
        public static bool SendSingle<T>(EcsWorld eventsWorld, T eventData, bool rewrite = true) where T : struct
        {
            var filter = eventsWorld.Filter<T>().End();
            var entity = FirstEntity(filter);

            if (entity != null)
            {
                if (!rewrite)
                {
                    return false;
                }
                var pool = eventsWorld.GetPool<T>();
                pool.Get((int)entity) = eventData;

                return true;
            }
            else
            {
                Send(eventsWorld, eventData);
                return true;
            }
            
            
        }

        #endregion

        #region CleanupEvent

        public static void CleanupEvent<T>(EcsWorld eventsWorld) where T : struct
        {
            var entities = eventsWorld.Filter<T>().End();
            foreach (var entity in entities)
            {
                eventsWorld.DelEntity(entity);
            }
        }

        public static void CleanupEvent<T>(IEcsSystems systems, EcsFilterInject<T> filter)
            where T : struct, IEcsInclude =>
            CleanupEvent(systems.GetWorld(Constants.Ecs.EventWorldName), filter);

        public static void CleanupEvent<T>(EcsWorld eventsWorld, EcsFilterInject<T> filter)
            where T : struct, IEcsInclude
        {
            foreach (var entity in filter.Value)
            {
                eventsWorld.DelEntity(entity);
            }
        }

        #endregion

        #region FirstEntity

        public static int? FirstEntity<T>(EcsFilterInject<T> filter) where T : struct, IEcsInclude =>
            FirstEntity(filter.Value);

        public static int? FirstEntity(EcsFilter filter)
        {
            foreach (var entity in filter)
            {
                return entity;
            }

            return null;
        }

        #endregion
    }
}