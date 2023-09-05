using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using td.features._common.components;
using td.features.destroy.flags;
using UnityEngine;

namespace td.features.destroy
{
    public class Destroy_Aspect : ProtoAspectInject
    {
        public ProtoPool<IsDestroyed> isDestroyedPool;
        public ProtoPool<IsDisabled> isDisabledPool;
        public ProtoPool<IsHidden> isHiddenPool;
        
        public ProtoPool<IsOnlyOnLevel> isOnlyOnLevelPool;
        
        public readonly ProtoItExc itOnlyOnLevel = new ProtoItExc(
        It.Inc<IsOnlyOnLevel, Ref<GameObject>>(),
        It.Exc<IsDestroyed, IsDisabled>()
        );
    }
}