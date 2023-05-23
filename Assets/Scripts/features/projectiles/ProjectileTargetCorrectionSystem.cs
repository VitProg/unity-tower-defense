using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.components.behaviors;
using td.components.commands;
using td.components.flags;
using td.components.refs;
using td.utils.ecs;
using UnityEngine;

namespace td.features.projectiles
{
    public class ProjectileTargetCorrectionSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<
            Inc<Projectile, ProjectileTarget, LinearMovementToTarget, Ref<GameObject>>, 
            Exc<IsDestroyed, RemoveGameObjectCommand, IsDisabled>
        > projectileEntities = default;

        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            
            foreach (var projectileEntity in projectileEntities.Value)
            {
                ref var fireTarget = ref projectileEntities.Pools.Inc2.Get(projectileEntity);
                ref var target = ref projectileEntities.Pools.Inc3.Get(projectileEntity);

                if (
                    !fireTarget.TargetEntity.Unpack(world, out var targetEntity) ||
                    world.HasComponent<IsDestroyed>(targetEntity) ||
                    world.HasComponent<IsDestroyed>(targetEntity) ||
                    world.HasComponent<IsDestroyed>(targetEntity)
                )
                {
                    world.GetComponent<RemoveGameObjectCommand>(projectileEntity);
                    world.GetComponent<IsDisabled>(projectileEntity);
                    continue;
                }
                
                ref var targetGameObject = ref world.GetComponent<Ref<GameObject>>(targetEntity);
                if (targetGameObject.reference != null && targetGameObject.reference.activeSelf)
                {
                    target.target = targetGameObject.reference.transform.position;
                }
                else
                {
                    // world.GetComponent<RemoveGameObjectCommand>(targetEntity);
                }
            }
        }
    }
}