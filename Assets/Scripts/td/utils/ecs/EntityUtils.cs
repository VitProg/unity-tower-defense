using Leopotam.EcsLite;
using td.common;
using td.common.ecs;

namespace td.utils.ecs
{
    public static class EntityUtils
    {
        public static bool AddComponent<T>(IEcsSystems systems, int entity, T component) where T : struct =>
            AddComponent(systems.GetWorld(), entity, component);

        public static bool AddComponent<T>(EcsWorld world, int entity, T component) where T : struct
        {
            var pool = world.GetPool<T>();

            if (pool.Has(entity))
            {
                return false;
            }

            ref var resultComponent = ref pool.Add(entity);

            if (resultComponent is IEcsAutoMerge<T> merge)
            {
                merge.AutoMerge(ref component, resultComponent);
            }
            resultComponent = component;
            
            return true;
        }

        public static ref T AddComponent<T>(IEcsSystems systems, int entity) where T : struct =>
            ref AddComponent<T>(systems.GetWorld(), entity);
        
        public static ref T AddComponent<T>(EcsWorld world, int entity) where T : struct
        {
            var pool = world.GetPool<T>();
            if (pool.Has(entity))
            {
                return ref pool.Get(entity); // todo
            }

            return ref pool.Add(entity);
        }

        public static ref T GetComponent<T>(IEcsSystems systems, int entity) where T : struct =>
            ref GetComponent<T>(systems.GetWorld(), entity);
        
        public static ref T GetComponent<T>(EcsWorld world, int entity) where T : struct
        {
            var pool = world.GetPool<T>();
            return ref pool.Get(entity);
        }

        public static bool HasComponent<T>(IEcsSystems systems, int entity) where T : struct =>
            HasComponent<T>(systems.GetWorld(), entity);
        
        public static bool HasComponent<T>(EcsWorld world, int entity) where T : struct
        {
            var pool = world.GetPool<T>();
            return pool.Has(entity);
        }

        public static void DelComponent<T>(IEcsSystems systems, int entity) where T : struct =>
            DelComponent<T>(systems.GetWorld(), entity);
        
        public static void DelComponent<T>(EcsWorld world, int entity) where T : struct
        {
            var pool = world.GetPool<T>();
            pool.Del(entity);
        }
    }
}