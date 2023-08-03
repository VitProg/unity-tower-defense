using System;
using System.Collections.Generic;
using Leopotam.EcsLite;
using UnityEngine;
// #if ENABLE_IL2CPP
// using System;
// using Unity.IL2CPP.CompilerServices;
// #endif

namespace td.utils.ecs
{
    /**
     * This is a copy from EcsSystems class but with interval run systems flow
     */
// #if ENABLE_IL2CPP
//     [Il2CppSetOption (Option.NullChecks, false)]
//     [Il2CppSetOption (Option.ArrayBoundsChecks, false)]
// #endif
    public class EcsRunSystemsWithInterval : IEcsSystems
    {
        readonly EcsWorld _defaultWorld;
        readonly Dictionary<string, EcsWorld> _worlds;
        readonly List<IEcsSystem> _allSystems;
        readonly List<IEcsRunSystem> _runSystems;
        readonly List<IEcsRunSystem> _runSystemsWithInterval;
        readonly List<IntervalForRunSystemWithIntervar> _intervalsForRunSystemsWithInterval;
        readonly List<IEcsPostRunSystem> _postRunSystems;
        readonly object _shared;
#if DEBUG
        protected bool _inited;
#endif

        public EcsRunSystemsWithInterval (EcsWorld defaultWorld, object shared = null) {
            _defaultWorld = defaultWorld;
            _shared = shared;
            _worlds = new Dictionary<string, EcsWorld> (8);
            _allSystems = new List<IEcsSystem> (128);
            _runSystems = new List<IEcsRunSystem> (128);
            _runSystemsWithInterval = new List<IEcsRunSystem>(128);
            _intervalsForRunSystemsWithInterval = new List<IntervalForRunSystemWithIntervar>(128);
            _postRunSystems = new List<IEcsPostRunSystem> (128);
        }

        public virtual T GetShared<T> () where T : class {
            return _shared as T;
        }

        public virtual IEcsSystems AddWorld (EcsWorld world, string name) {
#if DEBUG && !LEOECSLITE_NO_SANITIZE_CHECKS
            if (_inited) { throw new System.Exception ("Cant add world after initialization."); }
            if (world == null) { throw new System.Exception ("World cant be null."); }
            if (string.IsNullOrEmpty (name)) { throw new System.Exception ("World name cant be null or empty."); }
            if (_worlds.ContainsKey (name)) { throw new System.Exception ($"World with name \"{name}\" already added."); }
#endif
            _worlds[name] = world;
            return this;
        }

        public virtual EcsWorld GetWorld (string name = null) {
            if (name == null) {
                return _defaultWorld;
            }
            _worlds.TryGetValue (name, out var world);
            return world;
        }

        public virtual Dictionary<string, EcsWorld> GetAllNamedWorlds () {
            return _worlds;
        }

        public virtual IEcsSystems Add (IEcsSystem system) {
#if DEBUG && !LEOECSLITE_NO_SANITIZE_CHECKS
            if (_inited) { throw new System.Exception ("Cant add system after initialization."); }
#endif
            _allSystems.Add (system);
            if (system is IEcsRunSystem runSystem) {
                _runSystems.Add (runSystem);
            }
            if (system is IEcsPostRunSystem postRunSystem) {
                _postRunSystems.Add (postRunSystem);
            }
            return this;
        }

        public virtual IEcsSystems Add (IEcsRunSystem system, float interval, float timeShift = 0f) {
#if DEBUG && !LEOECSLITE_NO_SANITIZE_CHECKS
            if (_inited) { throw new System.Exception ("Cant add system after initialization."); }
#endif
            _allSystems.Add (system);

            _runSystemsWithInterval.Add(system);
            _intervalsForRunSystemsWithInterval.Add(new IntervalForRunSystemWithIntervar
            {
                countdown = timeShift,
                interval = interval
            });
            return this;
        }

        public virtual List<IEcsSystem> GetAllSystems () {
            return _allSystems;
        }

        public virtual void Init () {
#if DEBUG && !LEOECSLITE_NO_SANITIZE_CHECKS
            if (_inited) { throw new System.Exception ("Already initialized."); }
#endif
            foreach (var system in _allSystems) {
                if (system is IEcsPreInitSystem initSystem) {
                    initSystem.PreInit (this);
#if DEBUG && !LEOECSLITE_NO_SANITIZE_CHECKS
                    var worldName = EcsSystems.CheckForLeakedEntities (this);
                    if (worldName != null) { throw new System.Exception ($"Empty entity detected in world \"{worldName}\" after {initSystem.GetType ().Name}.PreInit()."); }
#endif
                }
            }
            foreach (var system in _allSystems) {
                if (system is IEcsInitSystem initSystem) {
                    initSystem.Init (this);
#if DEBUG && !LEOECSLITE_NO_SANITIZE_CHECKS
                    var worldName = EcsSystems.CheckForLeakedEntities (this);
                    if (worldName != null) { throw new System.Exception ($"Empty entity detected in world \"{worldName}\" after {initSystem.GetType ().Name}.Init()."); }
#endif
                }
            }
#if DEBUG
            _inited = true;
#endif
        }

        public virtual void Run()
        {
            float deltaTime = Time.deltaTime;
#if DEBUG && !LEOECSLITE_NO_SANITIZE_CHECKS
            if (!_inited) { throw new System.Exception ("Cant run without initialization."); }
#endif
            for (int i = 0, iMax = _runSystems.Count; i < iMax; i++) {
                _runSystems[i].Run (this);
#if DEBUG && !LEOECSLITE_NO_SANITIZE_CHECKS
                var worldName = EcsSystems.CheckForLeakedEntities (this);
                if (worldName != null) { throw new System.Exception ($"Empty entity detected in world \"{worldName}\" after {_runSystems[i].GetType ().Name}.Run()."); }
#endif
            }

            for (int i = 0, iMax = _runSystemsWithInterval.Count; i < iMax; i++)
            {
                var intervals = _intervalsForRunSystemsWithInterval[i];
                intervals.countdown -= deltaTime;
                if (intervals.countdown < 0.0001f)
                {
                    _runSystemsWithInterval[i].Run(this);
                    intervals.countdown = intervals.interval;
                }
                _intervalsForRunSystemsWithInterval[i] = intervals;
#if DEBUG && !LEOECSLITE_NO_SANITIZE_CHECKS
                var worldName = EcsSystems.CheckForLeakedEntities (this);
                if (worldName != null) { throw new System.Exception ($"Empty entity detected in world \"{worldName}\" after {_runSystems[i].GetType ().Name}.Run()."); }
#endif
            }
            
            for (int i = 0, iMax = _postRunSystems.Count; i < iMax; i++) {
                _postRunSystems[i].PostRun (this);
#if DEBUG && !LEOECSLITE_NO_SANITIZE_CHECKS
                var worldName = EcsSystems.CheckForLeakedEntities (this);
                if (worldName != null) { throw new System.Exception ($"Empty entity detected in world \"{worldName}\" after {_postRunSystems[i].GetType ().Name}.PostRun()."); }
#endif
            }
        }

        public virtual void Destroy () {
            for (var i = _allSystems.Count - 1; i >= 0; i--) {
                if (_allSystems[i] is IEcsDestroySystem destroySystem) {
                    destroySystem.Destroy (this);
#if DEBUG && !LEOECSLITE_NO_SANITIZE_CHECKS
                    var worldName = EcsSystems.CheckForLeakedEntities (this);
                    if (worldName != null) { throw new System.Exception ($"Empty entity detected in world \"{worldName}\" after {destroySystem.GetType ().Name}.Destroy()."); }
#endif
                }
            }
            for (var i = _allSystems.Count - 1; i >= 0; i--) {
                if (_allSystems[i] is IEcsPostDestroySystem postDestroySystem) {
                    postDestroySystem.PostDestroy (this);
#if DEBUG && !LEOECSLITE_NO_SANITIZE_CHECKS
                    var worldName = EcsSystems.CheckForLeakedEntities (this);
                    if (worldName != null) { throw new System.Exception ($"Empty entity detected in world \"{worldName}\" after {postDestroySystem.GetType ().Name}.PostDestroy()."); }
#endif
                }
            }
            _worlds.Clear ();
            _allSystems.Clear ();
            _runSystems.Clear ();
            _postRunSystems.Clear ();
#if DEBUG
            _inited = false;
#endif
        }

        private struct IntervalForRunSystemWithIntervar
        {
            public float interval;
            public float countdown;
        }
    }
    
    public class Intervalable : IEcsRunSystem
    {
        private readonly IEcsRunSystem system;
        private float countdown;
        private readonly float interval;
        private readonly Func<float> getDeltaTime;

        public Intervalable(IEcsRunSystem system, float interval, float timeShift,
            Func<float> getDeltaTime)
        {
            this.system = system;
            this.interval = interval;
            this.countdown = timeShift;
            this.getDeltaTime = getDeltaTime;
        }
            
        public void Run(IEcsSystems systems)
        {
            countdown -= getDeltaTime();

            if (countdown < 0.0001f)
            {
                countdown = interval;
                system.Run(systems);
            }
        }
    }

    public abstract class EcsIntervalableRunSystem : IEcsRunSystem
    {
        protected float countdown;
        protected readonly float interval;
        protected float deltaTime;
        protected readonly Func<float> getDeltaTime;
        protected readonly bool withInterval;

        public EcsIntervalableRunSystem(float interval, float timeShift, Func<float> getDeltaTime)
        {
            this.interval = interval;
            this.countdown = timeShift;
            this.getDeltaTime = getDeltaTime;
            withInterval = !Mathf.Approximately(interval, 0f);
        }

        protected virtual float GetNewInterval()
        {
            return interval;
        }
            
        public void Run(IEcsSystems systems)
        {
            var dt = getDeltaTime();
            countdown -= dt;
            deltaTime += dt;

            if (!(countdown < 0.0001f) && withInterval) return;
            
            IntervalRun(systems, deltaTime);
            countdown = GetNewInterval();
            deltaTime = 0;
        }

        public abstract void IntervalRun(IEcsSystems systems, float dt);
    }
}