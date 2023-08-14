using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using td.features._common.components;
using td.features.destroy.flags;
using td.features.projectile.attributes;
using UnityEngine;

namespace td.features.projectile.lightning
{
    public class Lightning_Aspect : ProtoAspectInject
    {
        public ProtoPool<Lightning> lightningPool;
        public ProtoPool<Ref<LineRenderer>> refLineRendererPool;

        public ProtoItExc it = new ProtoItExc(
            It.Inc<Lightning, LightningAttribute, Ref<GameObject>>(),
            It.Exc<IsDestroyed, IsDisabled>()
        );
    }
}