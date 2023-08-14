using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using td.features._common.components;
using td.features.destroy.flags;
using td.features.projectile.attributes;
using UnityEngine;

namespace td.features.projectile.explosion
{
    public class Explosion_Aspect : ProtoAspectInject
    {
        public ProtoPool<Explosion> explosionPool;
        public ProtoPool<Ref<ExplosionMonoBehaviour>> refExplosionMBPool;

        public ProtoItExc it = new ProtoItExc(
            It.Inc<Explosion, ExplosiveAttribute, Ref<GameObject>, Ref<ExplosionMonoBehaviour>>(),
            It.Exc<IsDestroyed, IsDisabled>()
        );
    }
}