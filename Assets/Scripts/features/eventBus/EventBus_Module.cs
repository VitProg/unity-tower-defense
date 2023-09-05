using System;
using System.Collections.Generic;
using Leopotam.EcsProto;
using td.features.eventBus.subServices;
using td.features.eventBus.systems;
using td.features.eventBus.types;
using td.utils;
using td.utils.ecs;

namespace td.features.eventBus
{
    public class EventBus_Module : IProtoModule
    {
        private EventBus_Aspect aspect;
        private static Type protoPoolType = typeof(ProtoPool<>);
        
        private readonly string persistEventTypeName = typeof(IPersistEvent).FullName;
        private readonly string globalEventTypeName = typeof(IGlobalEvent).FullName;
        private readonly string uniqueEventTypeName = typeof(IUniqueEvent).FullName;

        public EventBus_Module()
        {
            aspect = new EventBus_Aspect();
        }

        public void Init(IProtoSystems systems)
        {
            // systems.World().AddAspect(aspect);
            
            systems
                .AddSystem(new EventBus_System())
                .AddSystem(new EventBus_СleaningSystem())
                //
                .AddService(new EventBus_GlobalEvents(), true)
                .AddService(new EventBus_UniqueEvents(), true)
                .AddService(new EventBus(), true)
                ;
        }

        public IProtoAspect[] Aspects()
        {
            // return null;
            // Debug.Log("EventBus_Module.Asspects()");
            // Debug.Log("aspect = " + aspect);
            // Debug.Log("aspect.eventTypes = " + aspect.eventTypes);
            //
            return new IProtoAspect[]
            {
               aspect,
            };
        }

        public IProtoModule[] Modules()
        {
            return null;
        }

        public void AddEvents(List<Type> buildEvents)
        {
            // Debug.Log($"EventBus_Module.AddEvents(...{buildEvents.Count})");
            foreach (var evType in buildEvents) 
            {
                aspect.eventTypes.Add(evType);
                
                var isPersist = TypeUtils.HasInterface(evType, persistEventTypeName);
                var isGlobal = TypeUtils.HasInterface(evType, globalEventTypeName);
                var isUnique = TypeUtils.HasInterface(evType, uniqueEventTypeName);
                
                if (isPersist) aspect.persistEventTypes.Add(evType);
                if (isGlobal) aspect.globalEventTypes.Add(evType);
                if (isUnique) aspect.uniqueEventTypes.Add(evType);
            }
        }
    }
}