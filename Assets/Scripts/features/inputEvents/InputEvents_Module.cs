using System;
using Leopotam.EcsProto;
using td.features.eventBus;
using td.features.inputEvents.bus;
using td.features.inputEvents.systems;
using td.utils.ecs;

namespace td.features.inputEvents
{
    public class InputEvents_Module : IProtoModuleWithEvents
    {
        public void Init(IProtoSystems systems)
        {
            // Debug.Log($"{GetType().Name} Init");
            systems
                // .AddSystem(new InputEvents_CicleCollider_System())
                // .AddSystem(new InputEvents_HexCellCollider_System())
                .AddSystem(new InputEvents_HexCell_System())
                //
                .AddService(new InputEvents_Service(), true)
                ;
        }

        public IProtoAspect[] Aspects()
        {
            return new IProtoAspect[]
            {
                new InputEvents_Aspect()
            };
        }

        public IProtoModule[] Modules()
        {
            return null;
        }

        public Type[] Events() => Ev.E<Event_PointerDown, Event_PointerUp>();
    }
}