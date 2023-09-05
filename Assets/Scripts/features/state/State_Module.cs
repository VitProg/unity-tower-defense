using System;
using Leopotam.EcsProto;
using td.features.eventBus;
using td.features.state.bus;
using td.features.state.interfaces;
using td.features.state.systems;
using td.utils.ecs;

namespace td.features.state
{
    public class State_Module : IProtoModuleWithEvents
    {
        private readonly State state;
        private readonly State_Aspect aspect;
        
        public State_Module()
        {   
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

        private Type[] events = {
            typeof(Event_StageSomeChanged),
            typeof(Event_StateChanged),
        };

        public Type[] Events()
        {
            var count = aspect.extensions.Len();
            
            Array.Resize(ref events, count + 2);
            
            for (var idx = 0; idx < count; idx++)
            {
                var evType = aspect.extensions.Get(idx).GetEventType();
                events[idx + 2] = evType;
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