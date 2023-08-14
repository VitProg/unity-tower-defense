using Leopotam.EcsProto;
using td.utils.ecs;
using UnityEngine;

namespace td.features.inputEvents
{
    public class InputEvents_Module : IProtoModule
    {
        public void Init(IProtoSystems systems)
        {
            // Debug.Log($"{GetType().Name} Init");
            systems
                .AddSystem(new InputEvents_System())
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
    }
}