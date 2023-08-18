using System;
using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using td.features.eventBus.types;
using td.features.wave.bus;
using td.utils;
using UnityEngine;

namespace td.features.eventBus.systems
{
    public class EventBus_СleaningSystem : IProtoPostRunSystem
    {
        [DI(Constants.Worlds.EventBus)] private EventBus_Aspect aspect;
        [DI] private EventBus events;
        
        private readonly Type persistEventType = typeof(IPersistEvent);
        private readonly Type globalEventType = typeof(IGlobalEvent);
        private readonly Type uniqueEventType = typeof(IUniqueEvent);
        
        private readonly string persistEventTypeName = typeof(IPersistEvent).FullName;
        private readonly string globalEventTypeName = typeof(IGlobalEvent).FullName;
        private readonly string uniqueEventTypeName = typeof(IUniqueEvent).FullName;
        
        public void PostRun()
        {
            var count = aspect.eventTypes.Len();
            for (var idx = 0; idx < count; idx++)
            {
                var evType = aspect.eventTypes.Get(idx);

                var isPersist = TypeUtils.HasInterface(evType, persistEventTypeName);
                
                if (isPersist) continue;
                
                var isGlobal = TypeUtils.HasInterface(evType, globalEventTypeName);
                var isUnique = TypeUtils.HasInterface(evType, uniqueEventTypeName);

                if (isGlobal && !events.global.HasListeners(evType))
                {
                    events.global.Clear(evType);
                }

                if (isUnique && !events.unique.HasListeners(evType))
                {
                    events.unique.Del(evType);
                }
            }
        }
    }
}