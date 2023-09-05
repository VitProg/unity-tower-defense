using System;
using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using td.utils;
using UnityEngine;

namespace td.features.eventBus.systems {
    public class EventBus_System : IProtoRunSystem {
        [DI(Constants.Worlds.EventBus)] private EventBus_Aspect aspect;
        [DI] private EventBus eventBus;

        private readonly Slice<int> remove = new(32);

        public void Run() {
            var world = aspect.World();
            remove.Clear();

            // try {
                foreach (var evEntity in aspect.itEvent) {
                    var evData = aspect.World().Entities().Get(evEntity);
                    // Debug.Log("@ evData.Gen=" + evData.Gen);
                    // Debug.Log("@ evData.Mask.Len()=" + evData.Mask.Len());
                    
                    // for (aspect.itEvent.Begin(); aspect.itEvent.Next();) {
                    // var evEntity = aspect.itEvent.Entity();
                    for (var idx = 0; idx < aspect.eventTypes.Len(); idx++) {
                        var evType = aspect.eventTypes.Get(idx);
                        var pool = aspect.World().Pool(evType);
                        if (pool.Has(evEntity)) {
                            var processed = false;
                            var eventData = pool.Raw(evEntity);
                            if (aspect.globalEventPool.Has(evEntity)) {
#if EVENTBUS_DEBUG
                            Debug.Log($"EventBus:Global:Process {evType.Name} {eventData}");
#endif
                                processed = eventBus.global.Process(evType, eventData) || processed;
                            }

                            if (aspect.uniqueEventPool.Has(evEntity)) {
#if EVENTBUS_DEBUG
                            Debug.Log($"EventBus:Unique:Process {evType.Name} {eventData}");
#endif
                                processed = eventBus.unique.Process(evType, eventData) || processed;
                            }

                            if (processed && !aspect.persistEventPool.Has(evEntity)) {
#if EVENTBUS_DEBUG
                            Debug.Log($"EventBus:REMOVE event entity {evType.Name}");
#endif
                                remove.Add(evEntity);
                                // world.DelEntity(evEntity);
                            }
                        }
                    }
                }
            // }
            // catch (Exception ex) {
                // Debug.Log("EX:" + ex);
                // throw ex;
            // }
            // finally {
                // aspect.itEvent.End();
            // }


            var count = remove.Len();
            for (var idx = 0; idx < count; idx++) {
                world.DelEntity(remove.Get(idx));
            }
            remove.Clear();
        }
    }
}
