using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using td.features.destroy.flags;

namespace td.features.destroy
{
    public class Destroy_Aspect : ProtoAspectInject
    {
        public ProtoPool<IsDestroyed> isDestroyedPool;
        public ProtoPool<IsDisabled> isDisabledPool;
        public ProtoPool<IsHidden> isHiddenPool;
        
        public ProtoPool<IsOnlyOnLevel> isOnlyOnLevelPool;
    }
}