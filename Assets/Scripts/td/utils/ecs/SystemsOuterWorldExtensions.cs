using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.common.ecs;
using UnityEngine;

namespace td.utils.ecs
{
    public static class SystemsOuterWorldExtensions
    {
        public static void SendOuter<T>(this IEcsSystems systems) where T : struct => SendOuter(systems, new T());
        
        public static void SendOuter<T>(this IEcsSystems systems, T component) where T : struct
        {
            var outerWorld = systems.GetWorld(Constants.Worlds.Outer);
            var outerEntity = outerWorld.NewEntity();
            var pool = outerWorld.GetPool<T>();
            
            pool.Add(outerEntity) = component;

#if DEBUG || UNITY_EDITOR
            // if (component is IEcsDoNotDebugLog<T> == false)
            // {
                // Debug.Log($" -> {typeof(T)} {JsonUtility.ToJson(component)}");
            // }
            if (component is IEcsDoDebugLog<T> == false)
            {
                Debug.Log($" -> {typeof(T)} {JsonUtility.ToJson(component)}");
            }
#endif
        }

        public static bool SendSingleOuter<T>(this IEcsSystems systems, bool rewrite = true) where T : struct =>
            SendSingleOuter(systems, new T(), rewrite);
        
        public static bool SendSingleOuter<T>(this IEcsSystems systems, T component, bool rewrite = true)
            where T : struct
        {
            var outerWorld = systems.GetWorld(Constants.Worlds.Outer);
            var filter = outerWorld.Filter<T>().End();
            var entity = FirstEntity(filter);
            
            if (entity != null)
            {
                if (!rewrite)
                {
                    return false;
                }
                
                var pool = outerWorld.GetPool<T>();
                pool.Get((int)entity) = component;

                return true;
            }
            else
            {
                systems.SendOuter(component);
                return true;
            }
        }

        public static void CleanupOuter<T>(this IEcsSystems systems) where T : struct
        {
            var outerWorld = systems.GetWorld(Constants.Worlds.Outer);
            var entities = outerWorld.Filter<T>().End();
            foreach (var entity in entities)
            {
                outerWorld.DelEntity(entity);
            }
        }

        public static void CleanupOuter<T>(this IEcsSystems systems, EcsFilterInject<T> filter)
            where T : struct, IEcsInclude
        {
            var outerWorld = systems.GetWorld(Constants.Worlds.Outer);
            foreach (var entity in filter.Value)
            {
                outerWorld.DelEntity(entity);
            }
        }

        private static int? FirstEntity(EcsFilter filter)
        {
            foreach (var entity in filter)
            {
                return entity;
            }
        
            return null;
        }
    }
}