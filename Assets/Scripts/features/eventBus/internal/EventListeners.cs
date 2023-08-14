using System;
using Leopotam.EcsProto;
using td.features.eventBus.subServices;

namespace td.features.eventBus.@internal
{
    internal interface IEventListeners
    {
        bool InvokeRaw(object eventData);
        void RemoveAll();
    }
    
    internal class EventListeners<T> : IEventListeners where T : struct
    {
        private Slice<RefAction<T>> listeners = new(10);
        
        public void ListenTo(RefAction<T> action)
        {
            listeners.Add(action);
        }

        public bool Remove(RefAction<T> action)
        {
            for (var idx = 0; idx < listeners.Len(); idx++)
            {
                var listener = listeners.Get(idx);
                if (listener == action)
                {
                    listeners.RemoveAt(idx);
                    return true;
                }
            }
            return false;
        }

        public void RemoveAll()
        {
            listeners.Clear();
        }

        public bool Invoke(ref T eventData)
        {
            var count = listeners.Len();
            if (count == 0) return false;
            for (var idx = 0; idx < count; idx++)
            {
                listeners.Get(idx).Invoke(ref eventData);
            }
            return true;
        }
        
        public bool InvokeRaw(object eventData)
        {
            var d = (T)eventData;
            return Invoke(ref d);
        }
    }
}