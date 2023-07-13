using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.components.commands;
using td.components.events;
using td.components.flags;
using td.components.refs;
using td.features.impactsEnemy;
using td.features.projectiles.attributes;
using td.features.projectiles.explosion;
using td.features.projectiles.lightning;
using td.utils;
using td.utils.ecs;
using UnityEngine;

namespace td.features.projectiles
{
    public class ProjectileReachEnemyHandler : IEcsRunSystem
    {
        [Inject] private LightningLineService lightningLineService;
        [Inject] private ExplosionService explosionService;
        
        private readonly EcsFilterInject<Inc<ReachingTargetEvent, Projectile, ProjectileTarget>, Exc<IsDisabled, IsDestroyed>>
            entities = default;

        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            foreach (var projectileEntity in entities.Value)
            {
                ref var fireTarget = ref world.GetComponent<ProjectileTarget>(projectileEntity);

                if (fireTarget.targetEntity.Unpack(world, out var targetEntity))
                {
                    if (world.HasComponent<DamageAttribute>(projectileEntity))
                    {
                        ref var damageProjectile = ref world.GetComponent<DamageAttribute>(projectileEntity);
                        ref var takeDamage = ref systems.Outer<TakeDamageOuter>();
                        takeDamage.targetEntity = fireTarget.targetEntity;
                        takeDamage.damage = damageProjectile.damage;
                        takeDamage.type = DamageType.Casual;
                    }

                    if (world.HasComponent<ExplosiveAttribute>(projectileEntity))
                    {
                        ref var explosiveProjectile = ref world.GetComponent<ExplosiveAttribute>(projectileEntity);
                        var targetPosition = world.GetComponent<Ref<GameObject>>(targetEntity).reference.transform.position;
                        explosionService.SpawnExplosion(targetPosition, ref explosiveProjectile);
                    }

                    if (world.HasComponent<LightningAttribute>(projectileEntity))
                    {
                        ref var lightningProjectile = ref world.GetComponent<LightningAttribute>(projectileEntity);
                        lightningLineService.SpawnLightningLine(fireTarget.targetEntity, ref lightningProjectile);
                    }

                    if (world.HasComponent<SlowingAttribute>(projectileEntity))
                    {
                        ref var slowingProjectile = ref world.GetComponent<SlowingAttribute>(projectileEntity);
                        ref var speedDebuff = ref world.GetComponent<SpeedDebuff>(targetEntity);
                        
                        speedDebuff.duration = Mathf.Max(speedDebuff.duration, slowingProjectile.duration);
                        speedDebuff.speedMultipler = Mathf.Max(speedDebuff.speedMultipler, slowingProjectile.speedMultipler);
                    }

                    if (world.HasComponent<PoisonAttribute>(projectileEntity))
                    {
                        ref var poisonProjectile = ref world.GetComponent<PoisonAttribute>(projectileEntity);
                        ref var poisonDebuff = ref world.GetComponent<PoisonDebuff>(targetEntity);
                        
                        poisonDebuff.damage = Mathf.Max(poisonDebuff.damage, poisonProjectile.damageInterval);
                        poisonDebuff.duration = Mathf.Max(poisonDebuff.duration, poisonProjectile.duration);
                        poisonDebuff.damageInterval = Mathf.Min(poisonDebuff.damageInterval, poisonProjectile.damageInterval);
                    }

                    if (world.HasComponent<ShockingAttribute>(projectileEntity))
                    {
                        ref var shockingProjectile = ref world.GetComponent<ShockingAttribute>(projectileEntity);
                        
                        if (
                            !world.HasComponent<ShockingDebuff>(targetEntity) && 
                            RandomUtils.Bool(shockingProjectile.probability)
                        )
                        {
                            ref var shockingDebuff = ref world.GetComponent<ShockingDebuff>(targetEntity);
                            shockingDebuff.timeRemains = shockingProjectile.duration;
                        }
                    }
                }

                world.GetComponent<IsDisabled>(projectileEntity);
                world.GetComponent<RemoveGameObjectCommand>(projectileEntity);
            }
        }
    }
}