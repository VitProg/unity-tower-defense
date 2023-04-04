using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.components.commands;
using td.components.events;
using td.features.enemyImpacts;
using td.features.fire.components;
using td.features.impactsEnemy;
using td.utils.ecs;

namespace td.features.fire
{
    public class ProjectileReachTargetHandler : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<ReachingTargetEvent>> entities = Constants.Ecs.EventsWorldName;

        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            
            foreach (var eventEntity in entities.Value)
            {
                var packedEntity = entities.Pools.Inc1.Get(eventEntity).TargetEntity;

                if (
                    !packedEntity.Unpack(world, out var projectileEntity) ||
                    EntityUtils.HasComponent<IsProjectile>(systems, projectileEntity) == false ||
                    EntityUtils.HasComponent<FireTarget>(systems, projectileEntity) == false
                )
                {
                    continue;
                }
                
                ref var projectile = ref EntityUtils.GetComponent<IsProjectile>(systems, projectileEntity);
                ref var fireTarget = ref EntityUtils.GetComponent<FireTarget>(systems, projectileEntity);

                if (fireTarget.TargetEntity.Unpack(world, out var enemyEntity))
                {
                    // var enemy = EntityUtils.GetComponent<IsEnemy>(world, enemyEntity);
                    // var enemyStats = EntityUtils.GetComponent<SpawnEnemyCommand>(world, enemyEntity);
                    //todo - send take damage command

                    EcsEventUtils.Send(systems, new TakeDamageCommand()
                    {
                        TargetEntity = fireTarget.TargetEntity,
                        damage = projectile.damage,
                    });
                }
                EntityUtils.AddComponent<RemoveGameObjectCommand>(world, projectileEntity);
            }
        }
    }
}