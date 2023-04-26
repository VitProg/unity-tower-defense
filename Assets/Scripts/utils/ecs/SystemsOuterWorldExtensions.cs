using System;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace td.utils.ecs
{
    public static class SystemsOuterWorldExtensions
    {
        public static ref T Outer<T>(this IEcsSystems systems) where T : struct
        {
            var world = systems.GetWorld(Constants.Worlds.Outer);
            var entity = world.NewEntity();
            var pool = world.GetPool<T>();
            return ref pool.Add(entity);
        }

        public static ref T Outer<T>(this IEcsSystems systems, out int outerEntity) where T : struct
        {
            var world = systems.GetWorld(Constants.Worlds.Outer);
            var entity = world.NewEntity();
            var pool = world.GetPool<T>();
            outerEntity = entity;
            return ref pool.Add(entity);
        }

        public static ref T OuterSingle<T>(this IEcsSystems systems, bool rewrite = false) where T : struct
        {
            if (HasOuter<T>(systems))
            {
                if (!rewrite) return ref GetOuter<T>(systems);
                DelOuter<T>(systems);
                return ref Outer<T>(systems);
            }
            return ref Outer<T>(systems);
        }

        public static bool HasOuter<T>(this IEcsSystems systems) where T : struct
        {
            var world = systems.GetWorld(Constants.Worlds.Outer);
            var filter = world.Filter<T>().End();
            return filter.GetEntitiesCount() > 0;
        }
        
        public static ref T GetOuter<T>(this IEcsSystems systems) where T : struct
        {
            var world = systems.GetWorld(Constants.Worlds.Outer);
            var filter = world.Filter<T>().End();
            foreach (var entity in filter)
            {
                var pool = world.GetPool<T>();
                return ref pool.Get(entity);
            }

            throw new NullReferenceException($"Outer {(typeof(T).Name)} not found!");
        }
        
//         public static void Outer<T>(this IEcsSystems systems, T component) where T : struct
//         {
//             var outerWorld = systems.GetWorld(Constants.Worlds.Outer);
//             var outerEntity = outerWorld.NewEntity();
//             // Debug.Log($">> NewEntity Outer {outerEntity} - SystemOuterWorldExtensions");
//             var pool = outerWorld.GetPool<T>();
//             
//             pool.Add(outerEntity) = component;
//
// // #if DEBUG || UNITY_EDITOR
// //             // if (component is IEcsDoNotDebugLog<T> == false)
// //             // {
// //                 // Debug.Log($" -> {typeof(T)} {JsonUtility.ToJson(component)}");
// //             // }
// //             if (component is IEcsDoDebugLog<T> == false)
// //             {
// //                 // Debug.Log($" -> {typeof(T)} {JsonUtility.ToJson(component)}");
// //             }
// // #endif
//         }

        // public static bool SendSingleOuter<T>(this IEcsSystems systems, bool rewrite = true) where T : struct =>
        //     SendSingleOuter(systems, new T(), rewrite);
        //
        // public static bool SendSingleOuter<T>(this IEcsSystems systems, T component, bool rewrite = true)
        //     where T : struct
        // {
        //     var outerWorld = systems.GetWorld(Constants.Worlds.Outer);
        //     var filter = outerWorld.Filter<T>().End();
        //     var entity = FirstEntity(filter);
        //     
        //     if (entity != null)
        //     {
        //         if (!rewrite)
        //         {
        //             return false;
        //         }
        //         
        //         var pool = outerWorld.GetPool<T>();
        //         pool.Get((int)entity) = component;
        //
        //         return true;
        //     }
        //     else
        //     {
        //         systems.Outer(component);
        //         return true;
        //     }
        // }



        public static void DelOuter<T>(this IEcsSystems systems) where T : struct
        {
            var world = systems.GetWorld(Constants.Worlds.Outer);
            var entities = world.Filter<T>().End();
            foreach (var entity in entities)
            {
                world.DelEntity(entity);
            }
        }

        public static void CleanupOuter<T>(this IEcsSystems systems, EcsFilterInject<T> filter)
            where T : struct, IEcsInclude
        {
            var world = systems.GetWorld(Constants.Worlds.Outer);
            foreach (var entity in filter.Value)
            {
                world.DelEntity(entity);
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