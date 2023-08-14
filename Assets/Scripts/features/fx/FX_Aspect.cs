using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using td.features._common.components;
using td.features.destroy.flags;
using td.features.fx.effects;
using td.features.fx.flags;
using td.features.fx.types;
using UnityEngine;

namespace td.features.fx
{
    public class FX_Aspect : ProtoAspectInject
    {
        public ProtoPool<IsPositionFX> isPositionPool;
        public ProtoPool<IsScreenFX> isScreenPool;
        public ProtoPool<IsEntityFallowFX> isEntityFallowPool;
        public ProtoPool<IsEntityModifierFX> isEntityModifierPool;
        
        public ProtoPool<WithDurationFX> withDurationPool;
        public ProtoPool<WithTargetEntityFX> withTargetEntityPool;
        public ProtoPool<WithTransformFX> withTransformPool;

        public ProtoPool<BlinkFX> blinkFXPool;
        public ProtoPool<ColdStatusFX> coldStatusFXPool;
        public ProtoPool<ElectroStatusFX> electroStatusFXPool;
        public ProtoPool<FireStatusFX> fireStatusFXPool;
        public ProtoPool<PoisonStatusFX> poisonStatusFXPool;
        public ProtoPool<HitFX> hitFXPool;
        
        public ProtoPool<IsDestroyed> isDestroyedPool;
        public ProtoPool<IsDisabled> isDisabledPool;
        
        public ProtoPool<NeedRemoveFX> needRemovePool;
        public ProtoPool<Ref<GameObject>> refGOPool;

        public ProtoItExc itEntityModifier = new(
            It.Inc<IsEntityModifierFX, WithTargetEntityFX, WithDurationFX>(),
            It.Exc<IsDestroyed, IsDisabled>()
        );

        public ProtoItExc itEentityFallow = new(
            It.Inc<IsEntityFallowFX, WithTargetEntityFX, WithDurationFX, WithTransformFX>(),
            It.Exc<IsDestroyed, IsDisabled>()
        );

        public ProtoItExc itPosition = new(
            It.Inc<IsPositionFX, WithTransformFX, WithDurationFX>(),
            It.Exc<IsDestroyed, IsDisabled>()
        );

        public ProtoItExc itScreen = new(
            It.Inc<IsScreenFX, WithTransformFX, WithDurationFX>(),
            It.Exc<IsDestroyed, IsDisabled>()
        );

        public ProtoItExc itWithDuration = new(
            It.Inc<WithDurationFX>(),
            It.Exc<IsDestroyed, IsDisabled>()
        );
        
        public ProtoItExc itNeedRemove = new(
            It.Inc<NeedRemoveFX>(),
            It.Exc<IsDestroyed, IsDisabled>()
        );
    }
}