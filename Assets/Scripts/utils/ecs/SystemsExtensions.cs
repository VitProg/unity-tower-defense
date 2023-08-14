using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;

namespace td.utils.ecs
{
    public static class Extensions
    {
        public static IProtoSystem PostDel<T>(this ProtoSystems systems, string worldName = null) where T : struct
        {
#if DEBUG && !LEOECSLITE_NO_SANITIZE_CHECKS
            if (systems.World(worldName) == null)
            {
                throw new System.Exception(
                    $"Requested world \"{(string.IsNullOrEmpty(worldName) ? "[default]" : worldName)}\" not found.");
            }
#endif
            var system = new PostRunDelSystem<T>(systems.World(worldName));
            systems.AddSystem(system);
            return system;
        }
    }

    public sealed class PostRunDelSystem<T> : IProtoInitSystem, IProtoPostRunSystem where T : struct
    {
        readonly ProtoWorld _world;
        ProtoIt _it;
        ProtoPool<T> _pool;

        public PostRunDelSystem(ProtoWorld world)
        {
            _world = world;
        }

        public void Init(IProtoSystems systems)
        {
            var t = typeof(T);
            _pool = (ProtoPool<T>)_world.Pool(t);
            _it = new(new[] { t });
            _it.Init(_world);
        }

        public void PostRun()
        {
            foreach (var e in _it)
            {
                _pool.Del(e);
            }
        }
    }
}