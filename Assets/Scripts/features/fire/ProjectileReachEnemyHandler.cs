using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.components.commands;
using td.components.events;
using td.components.flags;
using td.features.fire.projectiles;
using td.features.impactsEnemy;
using td.utils.ecs;

namespace td.features.fire
{
    public class ProjectileReachEnemyHandler : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<ReachingTargetEvent, IsProjectile, FireTarget>, Exc<IsDisabled>> entities;

        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            
            foreach (var projectileEntity in entities.Value)
            {
                ref var projectile = ref world.GetComponent<IsProjectile>(projectileEntity);
                ref var fireTarget = ref world.GetComponent<FireTarget>(projectileEntity);

                if (fireTarget.TargetEntity.Unpack(world, out var targetEntity))
                {
                    if (world.TryGetComponent<DamageProjectile>(projectileEntity, out var damageProjectile))
                    {
                        systems.SendOuter(new TakeDamageOuter()
                        {
                            TargetEntity = fireTarget.TargetEntity,
                            damage = damageProjectile.damage,
                        });
                    }

                    if (world.TryGetComponent<ExplosiveProjectile>(projectileEntity, out var explosiveProjectile))
                    {
                        // todo
                    }
                    
                    if (world.TryGetComponent<LightningProjectile>(projectileEntity, out var lightningProjectile))
                    {
                        // todo
                    }

                    if (world.TryGetComponent<SlowingProjectile>(projectileEntity, out var slowingProjectile))
                    {
                        world.MergeComponent(targetEntity, new SpeedDebuff()
                        {
                            duration = slowingProjectile.duration,
                            speedMultipler = slowingProjectile.speedMultipler
                        });   
                    }
                    
                    if (world.TryGetComponent<PoisonProjectile>(projectileEntity, out var poisonProjectile))
                    {
                        world.MergeComponent(targetEntity, new PoisonDebuff()
                        {
                            damage = poisonProjectile.damageInterval,
                            duration = poisonProjectile.duration,
                            damageInterval = poisonProjectile.damageInterval
                        });   
                    }
                }

                world.AddComponent<IsDisabled>(projectileEntity);
                world.AddComponent<RemoveGameObjectCommand>(projectileEntity);
            }
        }
    }
}