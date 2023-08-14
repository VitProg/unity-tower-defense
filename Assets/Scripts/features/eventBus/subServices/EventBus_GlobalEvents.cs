using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using Leopotam.EcsProto.Unity;
using td.features.eventBus.@internal;
using td.features.eventBus.types;
using UnityEngine;

namespace td.features.eventBus.subServices
{
    public class EventBus_GlobalEvents
    {
        [DI(Constants.Worlds.EventBus)] private EventBus_Aspect aspect;
        
        private readonly Dictionary<Type, IEventListeners> eventListeners = new (25);
        
        private readonly Type persistEventType = typeof(IPersistEvent);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ProtoPool<T> GetPool<T>() where T : struct, IEvent
        {
            return (ProtoPool<T>)aspect.World().Pool(typeof(T));
        }
        
        public ref T Add<T>() where T : struct, IGlobalEvent
        {
            var evType = typeof(T);
            var pool = GetPool<T>();
            var evEntity = pool.World().NewEntity();
            ref var ev = ref pool.Add(evEntity);
            aspect.eventPool.Add(evEntity);
            aspect.globalEventPool.Add(evEntity);
            foreach (var i in evType.GetInterfaces())
            {
                if (i != persistEventType) continue;
                aspect.persistEventPool.Add(evEntity);
                break;
            }

            // Debug.Log($"EventBus:Global:Send ${evType.Name}");
            
            return ref ev;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear<T>() where T : struct, IGlobalEvent
        {
            var pool = GetPool<T>();

            if (pool.Len() == 0) return;

            foreach (var evEntity in pool.Entities())
            {
                aspect.World().DelEntity(evEntity);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Del<T>(int evEntity) where T : struct, IGlobalEvent
        {
            var pool = GetPool<T>();

            var entities = pool.Entities();
            var count = pool.Len();
            for (var idx = 0; idx < count; idx++)
            {
                if (entities[idx] == evEntity)
                {
                    aspect.World().DelEntity(evEntity);
                    return;
                }
            }
        }        
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Del(int evEntity)
        {
            aspect.World().DelEntity(evEntity);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Has<T>() where T : struct, IGlobalEvent
        {
            if (!aspect.release) return false;
            var pool = GetPool<T>();
            return pool.Len() > 0;
        }
        
        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        // public int[] Entities<T>() where T : struct, IGlobalEvent
        // {
        //     var pool = GetPool<T>();
        //     return pool.Entities();
        // }        
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T Get<T>(int evEntity) where T : struct, IGlobalEvent
        {
            var pool = GetPool<T>();
            return ref pool.Get(evEntity);
        }
        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        // public ref T Get<T>(int idx) where T : struct, IGlobalEvent
        // {
        //     var pool = GetPool<T>();
        //     var evEntity = pool.Entities()[idx];
        //     return ref pool.Get(evEntity);
        // }
        
        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        // public int Count<T>() where T : struct, IGlobalEvent
        // {
        //     if (!aspect.release) return 0;
        //     var pool = GetPool<T>();
        //     return pool.Len();
        // }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ProtoIt It<T>() where T : struct, IGlobalEvent
        {
            var evType = typeof(T);
            if (!aspect.itUniqueEventsHash.TryGetValue(evType, out var itIdx))
            {
                throw new Exception($"Iterator for type {EditorExtensions.GetCleanTypeName(evType)} not found for unique event");
            }

            return aspect.itUniqueEvents.Get(itIdx);
        }

        public void ListenTo<T>(RefAction<T> action) where T : struct, IGlobalEvent 
        {
            var type = typeof(T);
            if (!eventListeners.TryGetValue(type, out var listeners))
            {
                listeners = new EventListeners<T>();
                eventListeners.Add(type, listeners);
            }

            var l = (EventListeners<T>)listeners;
            l.ListenTo(action);
        }

        public bool RemoveListener<T>(RefAction<T> action) where T : struct, IGlobalEvent
        {
            var type = typeof(T);
            if (!eventListeners.TryGetValue(type, out var listeners)) return false;
            var l = (EventListeners<T>)listeners;
            return l.Remove(action);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveAllListeners<T>() where T : struct, IGlobalEvent =>
            RemoveAllListeners(typeof(T));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveAllListeners(Type evType)
        {
            if (!eventListeners.TryGetValue(evType, out var listeners)) return;
            listeners.RemoveAll();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Process(Type evType, object evData)
        {
            if (!eventListeners.ContainsKey(evType)) return false;
            return eventListeners[evType].InvokeRaw(evData);
        }
    }
}