using Leopotam.EcsProto.QoL;
using td.features.eventBus.subServices;

namespace td.features.eventBus
{
    public class EventBus
    {
        [DI] public EventBus_GlobalEvents global;
        [DI] public EventBus_UniqueEvents unique;
    }
    
    public delegate void RefAction<T>(ref T obj) where T : struct;
    public delegate void GlobalListener(ref object obj);
}