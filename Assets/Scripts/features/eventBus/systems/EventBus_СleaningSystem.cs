using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using UnityEngine;

namespace td.features.eventBus.systems
{
    public class EventBus_СleaningSystem : IProtoPostRunSystem
    {
        [DI(Constants.Worlds.EventBus)] private EventBus_Aspect aspect;
        
        public void PostRun()
        {
            foreach (var evEntity in aspect.itEventForDelete)
            {
                aspect.World().DelEntity(evEntity);
            }
        }
    }
}