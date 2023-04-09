using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.components.commands;
using td.components.events;
using td.features.enemyImpacts;
using td.features.impactsEnemy;
using td.utils.ecs;

namespace td.features.fire
{
    public class ProjectileReachTargetHandler : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<ReachingTargetEvent, IsProjectile, FireTarget>> entities;

        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            
            foreach (var entity in entities.Value)
            {
                ref var projectile = ref world.GetComponent<IsProjectile>(entity);
                ref var fireTarget = ref world.GetComponent<FireTarget>(entity);

                if (fireTarget.TargetEntity.Unpack(world, out _))
                {
                    systems.SendOuter(new TakeDamageOuterCommand()
                    {
                        TargetEntity = fireTarget.TargetEntity,
                        damage = projectile.damage,
                    });
                }
                world.AddComponent<RemoveGameObjectCommand>(entity);
            }
        }
    }
}