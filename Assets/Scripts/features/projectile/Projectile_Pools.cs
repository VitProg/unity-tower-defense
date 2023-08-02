using Leopotam.EcsLite.Di;
using td.features.projectile.attributes;
using td.features.projectile.components;
using td.features.projectile.explosion;
using td.features.projectile.lightning;

namespace td.features.projectile
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class Projectile_Pools
    {
        public readonly EcsPoolInject<Projectile> projectilePool = default;
        public readonly EcsPoolInject<ProjectileTarget> projectileTargetPool = default;
        public readonly EcsPoolInject<DamageAttribute> damageAttributePool = default;
        public readonly EcsPoolInject<ExplosiveAttribute> explosiveAttributePool = default;
        public readonly EcsPoolInject<LightningAttribute> lightningAttributePool = default;
        public readonly EcsPoolInject<PoisonAttribute> poisonAttributePool = default;
        public readonly EcsPoolInject<ShockingAttribute> shockingAttributePool = default;
        public readonly EcsPoolInject<SlowingAttribute> slowingAttributePool = default;
        public readonly EcsPoolInject<Explosion> explosionPool = default;
        public readonly EcsPoolInject<LightningLine> lightningLinePool = default;
    }
}