using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using td.features._common.components;
using td.features._common.interfaces;
using td.features.destroy.flags;
using td.features.inputEvents.components;
using td.features.movement.components;

namespace td.features.inputEvents
{
    public class InputEvents_Aspect : ProtoAspectInject
    {
        public ProtoPool<CicleCollider> cicleColliderPool;
        public ProtoPool<HexCellCollider> hexCellColliderPool;
        public ProtoPool<RefMany<IInputEventsHandler>> refPointerHandlersPool;

        public ProtoItExc itCicleCollider = new ProtoItExc(
            It.Inc<ObjectTransform, CicleCollider, RefMany<IInputEventsHandler>>(),
            It.Exc<IsDestroyed, IsDisabled>()
        );
        
        public ProtoItExc itHexCellCollider = new ProtoItExc(
            It.Inc<ObjectTransform, HexCellCollider, RefMany<IInputEventsHandler>>(),
            It.Exc<IsDestroyed, IsDisabled>()
        );
    }
}