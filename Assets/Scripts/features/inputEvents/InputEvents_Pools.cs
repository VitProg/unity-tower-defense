using Leopotam.EcsLite.Di;
using td.features._common;
using td.features._common.components;

namespace td.features.inputEvents
{
    public class InputEvents_Pools
    {
        public readonly EcsPoolInject<ObjectCicleCollider> cicleColliderPool = default;
        public readonly EcsPoolInject<RefMany<IInputEventsHandler>> refPointerHandlers = default;
        
        public readonly EcsFilterInject<Inc<ObjectTransform, ObjectCicleCollider, RefMany<IInputEventsHandler>>, ExcludeNotAlive> filter = default;
    }
}