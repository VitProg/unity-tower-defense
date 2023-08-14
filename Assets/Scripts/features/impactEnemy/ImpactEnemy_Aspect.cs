using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using td.features._common;
using td.features._common.components;
using td.features.destroy.flags;
using td.features.enemy.components;
using td.features.impactEnemy.components;
using UnityEngine;

namespace td.features.impactEnemy
{
    public class ImpactEnemy_Aspect : ProtoAspectInject
    {
        public ProtoPool<PoisonDebuff> poisonDebuffPool;
        public ProtoPool<ShockingDebuff> shockingDebuffPool;
        public ProtoPool<SpeedDebuff> speedDebuffPool;
        public ProtoPool<TakeDamage> takeDamagePool;

        public ProtoItExc itPoisonDebuff = new(
            It.Inc<PoisonDebuff, Enemy, Ref<GameObject>>(),
            It.Exc<IsDestroyed, IsDisabled>()
        );
        public ProtoItExc itShockingDebuff = new(
            It.Inc<ShockingDebuff, Enemy, Ref<GameObject>>(),
            It.Exc<IsDestroyed, IsDisabled>()
        );
        public ProtoItExc itSpeedDebuff = new(
            It.Inc<SpeedDebuff, Enemy, Ref<GameObject>>(),
            It.Exc<IsDestroyed, IsDisabled>()
        );
        public ProtoItExc itTakeDamage = new(
            It.Inc<TakeDamage, Enemy, Ref<GameObject>>(),
            It.Exc<IsDestroyed, IsDisabled>()
        );
    }
}