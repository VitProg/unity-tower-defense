using System;
using System.Collections.Generic;
using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using Leopotam.EcsProto.Unity;
using td.features.eventBus.components;
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
        public ProtoPool<EventLifetime> eventLifetimePool;

        public readonly ProtoIt itEvent = new(It.Inc<Event>());
        public readonly ProtoItExc itNonPersistEvents = new(
            It.Inc<Event>(), It.Exc<PersistEvent>()
        );

        
        public readonly Slice<Type> eventTypes = new(128);
        
        public readonly HashSet<Type> persistEventTypes = new(32);
        public readonly HashSet<Type> globalEventTypes = new(64);
        public readonly HashSet<Type> uniqueEventTypes = new(64);


        public readonly Slice<ProtoIt> sliceItUniqueEvents = new(32);
        public readonly Dictionary<Type, int> sliceItUniqueEventsHash = new(32);

        public bool release;

        private static readonly Type PoolType = typeof(ProtoPool<>);
        
        public override void Init(ProtoWorld world)
        {
            for (var idx = 0; idx < eventTypes.Len(); idx++)
            {
                var evType = eventTypes.Get(idx);

                var isUnique = uniqueEventTypes.Contains(evType);
                var capacity = isUnique ? 2 : 128;
                
                if (world.HasPool(evType)) continue;
                var pool = (IProtoPool)Activator.CreateInstance(PoolType.MakeGenericType(evType), capacity);
                world.AddPool(pool);
            }
            
            base.Init(world);
        }

        public override void PostInit() {
            for (var idx = 0; idx < eventTypes.Len(); idx++) {
                var evType = eventTypes.Get(idx);
                var isUnique = uniqueEventTypes.Contains(evType);
                if (!isUnique)
                {
                    var it = new ProtoIt(new [] { evType });
                    var itIdx = sliceItUniqueEvents.Len();
                    sliceItUniqueEvents.Add(it);
                    sliceItUniqueEventsHash.Add(evType, itIdx);
                    it.Init(World());
                }
            }

            base.PostInit();

            release = true;
        } 
    }
}