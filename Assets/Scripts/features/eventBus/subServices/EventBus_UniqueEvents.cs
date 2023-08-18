using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using td.features.eventBus.@internal;
using td.features.eventBus.types;
using UnityEngine;

namespace td.features.eventBus.subServices
{
    public class EventBus_UniqueEvents
    {
        [DI(Constants.Worlds.EventBus)] private EventBus_Aspect aspect;
        private readonly Dictionary<Type, IEventListeners> eventListeners = new (25);
        
        private readonly Type persistEventType = typeof(IPersistEvent);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ProtoPool<T> GetPool<T>() where T : struct, IEvent => 
            (ProtoPool<T>)aspect.World().Pool(typeof(T));
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IProtoPool GetPool(Type evType) => 
            aspect.World().Pool(evType);

        public int GetOrAdd(Type evType, object evData)
        {
            var pool = GetPool(evType);

            if (pool.Len() == 0)
            {
                var evEntity = pool.World().NewEntity();
                pool.AddRaw(evEntity, evData);
                aspect.eventPool.Add(evEntity);
                aspect.uniqueEventPool.Add(evEntity);
                foreach (var i in evType.GetInterfaces())
                {
                    if (i != persistEventType) continue;
                    aspect.persistEventPool.Add(evEntity);
                    break;
                }
                return evEntity;
            }

            var evExistEntity = pool.Entities()[0];
            pool.SetRaw(evExistEntity, evData);
            return evExistEntity;
        }
        
        public ref T GetOrAdd<T>() where T : struct, IUniqueEvent
        {
            var pool = GetPool<T>();
            var evType = typeof(T);

            if (pool.Len() == 0)
            {
                var evEntity = pool.World().NewEntity();
                ref var ev = ref pool.Add(evEntity);
                aspect.eventPool.Add(evEntity);
                aspect.uniqueEventPool.Add(evEntity);
                foreach (var i in evType.GetInterfaces())
                {
                    if (i != persistEventType) continue;
                    aspect.persistEventPool.Add(evEntity);
                    break;
                }
                return ref ev;
            }
            
            ref var evExist = ref pool.Get(pool.Entities()[0]);
            return ref evExist;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Del<T>() where T : struct, IUniqueEvent
        {
            Del(typeof(T));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Del(Type evType)
        {
            var pool = GetPool(evType);
            var count = pool.Len();
            
            if (pool.Len() == 0) return;

#if DEBUG
            if (count > 1) throw new Exception($"Unique event pool contain more then one entity");
#endif
            aspect.World().DelEntity(pool.Entities()[0]);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Has<T>() where T : struct, IUniqueEvent
        {
            if (!aspect.release) return false;
            var pool = GetPool<T>();
            return pool.Len() > 0;
        }
        
        public bool HasListeners<T>() where T : struct, IGlobalEvent => eventListeners.ContainsKey(typeof(T));
        public bool HasListeners(Type evType) => eventListeners.ContainsKey(evType);

        public void ListenTo<T>(RefAction<T> action) where T : struct, IUniqueEvent
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool RemoveListener<T>(RefAction<T> action) where T : struct, IUniqueEvent
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
        public bool Process(Type evType, object eventData)
        {
            if (!eventListeners.ContainsKey(evType)) return false;
            eventListeners[evType].InvokeRaw(eventData);
            return true;
        }
    }
}