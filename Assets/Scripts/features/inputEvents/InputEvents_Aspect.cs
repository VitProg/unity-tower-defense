using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using td.features._common.components;
using td.features.destroy.flags;
using td.features.inputEvents.components;
using td.features.movement;
using td.features.movement.components;

namespace td.features.inputEvents
{
    public class InputEvents_Aspect : ProtoAspectInject
    {
        public ProtoPool<ObjectCicleCollider> cicleColliderPool;
        public ProtoPool<RefMany<IInputEventsHandler>> refPointerHandlersPool;

        public ProtoItExc it = new ProtoItExc(
            It.Inc<ObjectTransform, ObjectCicleCollider, RefMany<IInputEventsHandler>>(),
            It.Exc<IsDestroyed, IsDisabled>()
        );
    }
}