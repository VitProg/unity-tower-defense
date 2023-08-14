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

        public ProtoIt itEvent = new(It.Inc<Event>());
        public Slice<Type> eventTypes = new(64);

        public Slice<ProtoIt> itUniqueEvents = new(32);
        public Dictionary<Type, int> itUniqueEventsHash = new(32);

        public ProtoItExc itEventForDelete = new(It.Inc<Event>(), It.Exc<PersistEvent>());
        
        public bool release;
        
        private static readonly Type poolType = typeof(ProtoPool<>);
        private static readonly Type uniqueEventType = typeof(IUniqueEvent);

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
                    if (i != uniqueEventType) continue;
                    isUnique = true;
                    break;
                }
                var capacity = isUnique ? 2 : 128;
                
                if (world.HasPool(evType)) continue;
                var pool = (IProtoPool)Activator.CreateInstance(poolType.MakeGenericType(evType), capacity);
                world.AddPool(pool);
                Debug.Log($"A pool has been created for {EditorExtensions.GetCleanTypeName(evType)}");

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