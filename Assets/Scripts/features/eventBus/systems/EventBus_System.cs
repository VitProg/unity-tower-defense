using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;

namespace td.features.eventBus.systems
{
    public class EventBus_System : IProtoRunSystem
    {
        [DI(Constants.Worlds.EventBus)] private EventBus service;
        [DI(Constants.Worlds.EventBus)] private EventBus_Aspect aspect;

        private Slice<int> remove = new(32);

        public void Run()
        {
            var world = aspect.World();
            
            foreach (var evEntity in aspect.itEvent)
            {
                for (var idx = 0; idx < aspect.eventTypes.Len(); idx++)
                {
                    var evType = aspect.eventTypes.Get(idx);
                    var pool = aspect.World().Pool(evType);
                    if (pool.Has(evEntity))
                    {
                        var processed = false;
                        var eventData = pool.Raw(evEntity);
                        if (aspect.globalEventPool.Has(evEntity))
                        {
                            processed = service.global.Process(evType, eventData) || processed;
                        }
                        if (aspect.uniqueEventPool.Has(evEntity))
                        {
                            processed = service.unique.Process(evType, eventData) || processed;
                        }

                        if (processed && !aspect.persistEventPool.Has(evEntity))
                        {
                            remove.Add(evEntity);
                        }
                    }
                }

                var count = remove.Len();
                for (var idx = 0; idx < count; idx++)
                {
                    world.DelEntity(remove.Get(idx));
                }
                remove.Clear();
            }
        }
    }
}