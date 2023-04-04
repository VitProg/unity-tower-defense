using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.components.attributes;
using td.components.links;
using td.features.fire.components;
using td.utils.ecs;

namespace td.features.fire
{
    public class ProjectileTargetCorrectionSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<IsProjectile, FireTarget, Target, GameObjectLink>> entities = default;

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
                
                ref var targetGameObject = ref EntityUtils.GetComponent<GameObjectLink>(systems, targetEntity);
                target.target = targetGameObject.gameObject.transform.position;
            }
        }
    }
}