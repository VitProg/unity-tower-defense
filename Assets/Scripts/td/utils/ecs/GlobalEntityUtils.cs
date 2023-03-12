using Leopotam.EcsLite;
using td.common;

namespace td.utils.ecs
{
    public static class GlobalEntityUtils
    {
        public static int GetEntity(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            var sharedData = systems.GetShared<SharedData>();
            sharedData.GlobalEntity.Unpack(world, out var globalEntity);
            return globalEntity;
        }

        public static bool AddComponent<T>(IEcsSystems systems, T commandData) where T : struct =>
            EntityUtils.AddComponent(systems, GetEntity(systems), commandData);

        public static ref T AddComponent<T>(IEcsSystems systems) where T : struct =>
            ref EntityUtils.AddComponent<T>(systems, GetEntity(systems));

        public static ref T GetComponent<T>(IEcsSystems systems) where T : struct =>
            ref EntityUtils.GetComponent<T>(systems, GetEntity(systems));

        public static bool HasComponent<T>(IEcsSystems systems) where T : struct =>
            EntityUtils.HasComponent<T>(systems, GetEntity(systems));

        public static void DelComponent<T>(IEcsSystems systems) where T : struct =>
            EntityUtils.DelComponent<T>(systems, GetEntity(systems));
    }
}
