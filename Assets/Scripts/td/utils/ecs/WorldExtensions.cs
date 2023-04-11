using System;
using Leopotam.EcsLite;
using td.common;
using td.common.ecs;

namespace td.utils.ecs
{
    public static class WorldExtensions
    {
        public static ref T AddComponent<T>(this EcsWorld world, int entity) where T : struct =>
            ref AddComponent(world, entity, new T());
        
        public static ref T AddComponent<T>(this EcsWorld world, int entity, T component)
            where T : struct
        {
            var pool = world.GetPool<T>();

            if (pool.Has(entity))
            {
                pool.Del(entity);
            }

            ref var resultComponent = ref pool.Add(entity);

            if (resultComponent is IEcsAutoMerge<T> merge)
            {
                merge.AutoMerge(ref component, resultComponent);
            }

            resultComponent = component;

            return ref resultComponent;
        }

        public static ref T GetComponent<T>(this EcsWorld world, int entity, bool addIfNotExist = false) where T : struct {
            var pool = world.GetPool<T>();
            if (addIfNotExist && !pool.Has(entity))
            {
                return ref AddComponent<T>(world, entity);
            }
            return ref pool.Get(entity);
        }
        
        public static bool HasComponent<T>(this EcsWorld world, int entity) where T : struct
        {
            var pool = world.GetPool<T>();
            return pool.Has(entity);
        }

        public static void DelComponent<T>(this EcsWorld world, int entity) where T : struct
        {
            var pool = world.GetPool<T>();
            pool.Del(entity);
        }
        
        public static int GetEntitiesCount<T>(this EcsWorld world) where T : struct =>
            world.Filter<T>().End().GetEntitiesCount();
    }
}