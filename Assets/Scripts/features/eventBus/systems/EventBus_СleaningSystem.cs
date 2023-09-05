using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using td.utils;
using UnityEngine;

namespace td.features.eventBus.systems
{
    public class EventBus_СleaningSystem : IProtoRunSystem
    {
        [DI(Constants.Worlds.EventBus)] private EventBus_Aspect aspect;
        [DI] private EventBus events;
        
        private readonly Slice<int> remove = new(32);

        public void Run() {
            remove.Clear();
            foreach (var evEntity in aspect.itNonPersistEvents) {
#if EVENTBUS_DEBUG
                Debug.Log($"Event Clear {eventEntity}");
#endif
                var evData = aspect.World().Entities().Get(evEntity);
                // Debug.Log("@ evData.Gen=" + evData.Gen);
                // Debug.Log("@ evData.Mask.Len()=" + evData.Mask.Len());
                
                ref var lifetime = ref aspect.eventLifetimePool.Get(evEntity);
                lifetime.frames++;
                if (lifetime.frames > 1) {
                    remove.Add(evEntity);
                }
            }

            var world = aspect.World();
            var count = remove.Len();
            for (var idx = 0; idx < count; idx++) {
                world.DelEntity(remove.Get(idx));
            }
            remove.Clear();
        }
    }
}