using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.components;
using td.components.behaviors;
using td.utils.ecs;
using UnityEngine;

namespace td.features.fire
{
    public class ProjectileTargetCorrectionSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<IsProjectile, FireTarget, LinearMovementToTarget, Ref<GameObject>>> entities = default;

        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            
            foreach (var entity in entities.Value)
            {
                ref var fireTarget = ref entities.Pools.Inc2.Get(entity);
                ref var target = ref entities.Pools.Inc3.Get(entity);

                if (!fireTarget.TargetEntity.Unpack(world, out var targetEntity))
                {
                    continue;
                }
                
                ref var targetGameObject = ref world.GetComponent<Ref<GameObject>>(targetEntity);
                target.target = targetGameObject.reference.transform.position;

                var a = world.GetComponent<Ref<GameObject>>(targetEntity);
                var b = 1;
            }
        }
    }
}