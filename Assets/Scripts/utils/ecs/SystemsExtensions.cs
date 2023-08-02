// #if ENABLE_IL2CPP
// using System;
// using Unity.IL2CPP.CompilerServices;
// #endif
using Leopotam.EcsLite;

namespace td.utils.ecs
{
// #if ENABLE_IL2CPP
    // [Il2CppSetOption (Option.NullChecks, false)]
    // [Il2CppSetOption (Option.ArrayBoundsChecks, false)]
// #endif
    public static class Extensions {
#if !LEOECSLITE_DI
        public static IEcsSystems AddGroup (this IEcsSystems systems, string groupName, bool defaultState, string eventWorldName, params IEcsSystem[] nestedSystems) {
            return systems.Add (new EcsGroupSystem (groupName, defaultState, eventWorldName, nestedSystems));
        }
#endif
        public static IEcsSystems PostDel<T> (this IEcsSystems systems, string worldName = null) where T : struct {
#if DEBUG && !LEOECSLITE_NO_SANITIZE_CHECKS
            if (systems.GetWorld (worldName) == null) { throw new System.Exception ($"Requested world \"{(string.IsNullOrEmpty (worldName) ? "[default]" : worldName)}\" not found."); }
#endif
            return systems.Add (new PostRunDelSystem<T> (systems.GetWorld (worldName)));
        }
    }

    
// #if ENABLE_IL2CPP
    // [Il2CppSetOption (Option.NullChecks, false)]
    // [Il2CppSetOption (Option.ArrayBoundsChecks, false)]
// #endif
    public sealed class PostRunDelSystem<T> : IEcsPostRunSystem where T : struct {
        readonly EcsFilter _filter;
        readonly EcsPool<T> _pool;

        public PostRunDelSystem (EcsWorld world) {
            _filter = world.Filter<T> ().End ();
            _pool = world.GetPool<T> ();
        }

        public void PostRun (IEcsSystems systems) {
            foreach (var entity in _filter) {
                _pool.Del (entity);
            }
        }
    }
}