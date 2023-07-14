using System;
using System.Collections.Generic;
using System.Reflection;
using td.utils.ecs;
using UnityEngine;

namespace td.features.eventBus
{
    public class EventBus
    {
        #region fields

        private readonly Dictionary<Type, List<WeakReference<IBaseEventReceiver>>> receivers;
        private readonly Dictionary<string, WeakReference<IBaseEventReceiver>> receiverHashToReference;
        private readonly Dictionary<Type, List<IEvent>> delayedEvents;

        #endregion

        #region constructors

        public EventBus()
        {
            receivers = new Dictionary<Type, List<WeakReference<IBaseEventReceiver>>>();
            receiverHashToReference = new Dictionary<string, WeakReference<IBaseEventReceiver>>();
            delayedEvents = new Dictionary<Type, List<IEvent>>();
        }

        #endregion

        #region public methods

        public void Subscribe<T>(IEventReceiver<T> receiver) where T : struct, IEvent
        {
            var eventType = typeof(T);
            if (!receivers.ContainsKey(eventType))
                receivers[eventType] = new List<WeakReference<IBaseEventReceiver>>();

            if (!receiverHashToReference.TryGetValue(receiver.Id, out WeakReference<IBaseEventReceiver> reference))
            {
                reference = new WeakReference<IBaseEventReceiver>(receiver);
                receiverHashToReference[receiver.Id] = reference;
            }

            receivers[eventType].Add(reference);

            // if (delayedEvents.TryGetValue(eventType, out var events))
            // {
            //     foreach (var @event in events)
            //     {
            //         Debug.Log("FROM DELAYED");
            //         receiver.OnEvent((T)@event);
            //     }
            //     events.Clear();
            //     delayedEvents.Remove(eventType);
            // }
        }

        public void Unsubscribe<T>(IEventReceiver<T> receiver) where T : struct, IEvent
        {
            var eventType = typeof(T);
            if (!receivers.ContainsKey(eventType) || !receiverHashToReference.ContainsKey(receiver.Id))
                return;

            var reference = receiverHashToReference[receiver.Id];

            receivers[eventType].Remove(reference);

            var weakRefCount = 0;
            // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
            foreach (var r in receivers)
            {
                // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
                foreach (var x in r.Value)
                {
                    weakRefCount += x == reference ? 1 : 0;
                }
            }
            if (weakRefCount == 0)
                receiverHashToReference.Remove(receiver.Id);
        }

        public void Send<T>(T @event, bool important = true) where T : struct, IEvent
        {
            var eventType = typeof(T);

            if (!receivers.ContainsKey(eventType))
            {
                // if (important)
                // {
                //     Debug.Log("EVENT add to delayed");
                //
                //     if (delayedEvents.TryGetValue(eventType, out var events))
                //     {
                //         events.Add(@event);
                //     }
                //     else
                //     {
                //         delayedEvents.Add(eventType, new List<IEvent> { @event });
                //     }
                // }

                return;
            }
            // delayedEvents.Remove(eventType);

            var references = receivers[eventType];
            for (var i = references.Count - 1; i >= 0; i--)
            {
                if (references[i].TryGetTarget(out var receiver))
                {
                    ((IEventReceiver<T>)receiver).OnEvent(@event);
                }
            }
        }

        #endregion
    }
}