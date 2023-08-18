using System;
using System.Collections.Generic;
using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using Leopotam.EcsProto.Unity;
using td.features.eventBus.types;
using UnityEngine;
using Event = td.features.eventBus.types.Event;

namespace td.features.eventBus
{
    public class EventBus_Aspect : ProtoAspectInject
    {
        public ProtoPool<UniqueEvent> uniqueEventPool;
        public ProtoPool<GlobalEvent> globalEventPool;
        public ProtoPool<Event> eventPool;
        public ProtoPool<PersistEvent> persistEventPool;

        public readonly ProtoIt itEvent = new(It.Inc<Event>());
        public readonly Slice<Type> eventTypes = new(64);

        public readonly Slice<ProtoIt> itUniqueEvents = new(32);
        public readonly Dictionary<Type, int> itUniqueEventsHash = new(32);

        // public readonly ProtoItExc itGlobalEventForDelete = new(It.Inc<GlobalEvent>(), It.Exc<PersistEvent>());
        // public readonly ProtoItExc itUniqueEventForDelete = new(It.Inc<UniqueEvent>(), It.Exc<PersistEvent>());
        
        public bool release;
        
        private static readonly Type PoolType = typeof(ProtoPool<>);
        private static readonly Type UniqueEventType = typeof(IUniqueEvent);

        public override void Init(ProtoWorld world)
        {
            // Debug.Log("EventBus_Aspect.Init() " + world);
            base.Init(world);

            for (var idx = 0; idx < eventTypes.Len(); idx++)
            {
                var evType = eventTypes.Get(idx);
                
                var isUnique = false;
                foreach (var i in evType.GetInterfaces())
                {
                    if (i != UniqueEventType) continue;
                    isUnique = true;
                    break;
                }
                var capacity = isUnique ? 2 : 128;
                
                if (world.HasPool(evType)) continue;
                var pool = (IProtoPool)Activator.CreateInstance(PoolType.MakeGenericType(evType), capacity);
                world.AddPool(pool);
#if UNITY_EDITOR
                Debug.Log($"A pool has been created for {EditorExtensions.GetCleanTypeName(evType)}");
#endif

                if (!isUnique)
                {
                    var it = new ProtoIt(new [] { evType });
                    var itIdx = itUniqueEvents.Len();
                    itUniqueEvents.Add(it);
                    itUniqueEventsHash.Add(evType, itIdx);
                    it.Init(world);
                }
            }
            
            release = true;
        }
    }
}