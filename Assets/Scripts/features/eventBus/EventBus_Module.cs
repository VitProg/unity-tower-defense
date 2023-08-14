using System;
using System.Collections.Generic;
using Leopotam.EcsProto;
using td.features.eventBus.subServices;
using td.features.eventBus.systems;
using td.utils.ecs;
using UnityEngine;

namespace td.features.eventBus
{
    public class EventBus_Module : IProtoModule
    {
        private EventBus_Aspect aspect;
        private static Type protoPoolType = typeof(ProtoPool<>);

        public EventBus_Module()
        {
            aspect = new EventBus_Aspect();
        }

        public void Init(IProtoSystems systems)
        {
            // systems.World().AddAspect(aspect);
            
            systems
                .AddSystem(new EventBus_System())
                // .AddSystem(new EventBus_СleaningSystem())
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
                // Debug.Log($" - {evType.Name} event");
                // if (aspect.eventTypes.Contains(evType))
                // {
                    // throw new Exception($"Тип события {evType.Name} уже зарегистрирован в аспекте EventBus_Aspect");
                // }
                //todo проверить на уникальность
                aspect.eventTypes.Add(evType);
            }
        }
    }
}