using System;
using Leopotam.EcsProto;
using td.features.eventBus;
using td.utils.ecs;
using UnityEngine;

namespace td.features.state
{
    public class State_Module : IProtoModuleWithEvents
    {
        private readonly State state;
        private readonly State_Aspect aspect;
        
        public State_Module()
        {
            // Debug.Log($"{GetType().Name} Init");
            
            aspect = new State_Aspect();
            
            state = new State();
            state.SetGameSpeed(1f);
        }
        
        public void Init(IProtoSystems systems)
        {
            systems
                .AddSystem(new State_ExtensionsInit_System())
                .AddSystem(new State_SendChanges_System())
                //
                .AddService(state, true)
                ;
        }

        public IProtoAspect[] Aspects()
        {
            return new IProtoAspect[]
            {
                aspect,
            };
        }

        public IProtoModule[] Modules()
        {
            return null;
        }

        public Type[] Events()
        {
            var count = aspect.extensions.Len();
            
            var events = new[]
            {
                typeof(Event_StateChanged),
            };
            Array.Resize(ref events, count + 1);
            
            for (var idx = 0; idx < count; idx++)
            {
                var evType = aspect.extensions.Get(idx).GetEventType();
                events[idx + 1] = evType;
            }

            return events;
        }

        public void AddStateExtensions(Slice<IStateExtension> extensions)
        {
            for (var idx = 0; idx < extensions.Len(); idx++)
            {
                aspect.AddEx(extensions.Get(idx));
            }
            extensions.Clear();
        }
    }
}