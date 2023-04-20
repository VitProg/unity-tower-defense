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

            ref var newComponent = ref pool.Add(entity);

            if (newComponent is IEcsAutoMerge<T> merge)
            {
                merge.AutoMerge(ref component, newComponent);
            }

            newComponent = component;

            return ref newComponent;
        }

        public static ref T MergeComponent<T>(this EcsWorld world, int entity, T component) where T : struct
        {
            var pool = world.GetPool<T>();

            if (!pool.Has(entity))
            {
                return ref AddComponent(world, entity, component);
            }

            ref var componentInPool = ref pool.Get(entity);
            
            if (componentInPool is IEcsAutoMerge<T> merge)
            {
                merge.AutoMerge(ref componentInPool, component);
                return ref componentInPool;
            }

            return ref AddComponent(world, entity, component);
        }
        
        public static ref T GetComponent<T>(this EcsWorld world, int entity, bool addIfNotExist = false) where T : struct {
            var pool = world.GetPool<T>();
            if (addIfNotExist && !pool.Has(entity))
            {
                return ref AddComponent<T>(world, entity);
            }
            return ref pool.Get(entity);
        }

        public static bool TryGetComponent<T>(this EcsWorld world, int entity, out T component) where T : struct
        {
            if (!HasComponent<T>(world, entity))
            {
                component = default;
                return false;
            }

            component = GetComponent<T>(world, entity);

            return true;
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