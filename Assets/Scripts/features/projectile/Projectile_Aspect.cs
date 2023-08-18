using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using td.features.destroy.flags;
using td.features.movement.components;
using td.features.movement.flags;
using td.features.movement.systems;
using td.features.projectile.attributes;
using td.features.projectile.components;

namespace td.features.projectile
{
    public class Projectile_Aspect : ProtoAspectInject, IMovementAspect
    {
        public ProtoPool<Projectile> projectilePool;
        public ProtoPool<ProjectileTarget> projectileTargetPool;

        public ProtoPool<DamageAttribute> damageAttributePool;
        public ProtoPool<ExplosiveAttribute> explosiveAttributePool;
        public ProtoPool<LightningAttribute> lightningAttributePool;
        public ProtoPool<PoisonAttribute> poisonAttributePool;
        public ProtoPool<ShockingAttribute> shockingAttributePool;
        public ProtoPool<SlowingAttribute> slowingAttributePool;
        
        private ProtoPool<IsTargetReached> isTargetReachedPool;

        private ProtoItExc itProjectileMovement = It
            .Chain<Projectile>()
            .Inc<Movement>()
            .Inc<ObjectTransform>()
            .Exc<IsSmoothRotation>()
            .Exc<IsDestroyed>()
            .Exc<IsDisabled>()
            .Exc<IsHidden>()
            .Exc<IsFreezed>()
            .End();
/*            It.Inc<Movement, ObjectTransform, Projectile>(),
            It.Exc(
                typeof(IsDestroyed),
                typeof(IsDisabled),
                typeof(IsHidden),
                typeof(IsFreezed),
                typeof(IsSmoothRotation)
            )
        );*/

        public ProtoItExc itProjectileReachTarget = new ProtoItExc(
            It.Inc<Projectile, IsTargetReached, ObjectTransform, Movement>(),
            It.Exc<IsDestroyed, IsDisabled>()
        );

        public ProtoItExc GetIt() => itProjectileMovement;
        public ProtoPool<IsTargetReached> GetIsTargetReachedPool() => isTargetReachedPool;
    }
}