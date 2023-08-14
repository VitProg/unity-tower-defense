using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using td.features._common.components;
using td.features.destroy.flags;
using UnityEngine;

namespace td.features.level
{
    public class Level_Aspect : ProtoAspectInject
    {
        public readonly ProtoPool<IsOnlyOnLevel> IsOnlyOnLevelPool;
        public readonly ProtoPool<Ref<GameObject>> refGOPool;
        public readonly ProtoPool<IsDestroyed> isDestroyedPool;
        public readonly ProtoPool<IsDisabled> IsDisabledPool;

        public readonly ProtoItExc itOnlyOnLevel = new ProtoItExc(
            It.Inc<IsOnlyOnLevel, Ref<GameObject>>(),
            It.Exc<IsDestroyed, IsDisabled>()
        );
    }
}