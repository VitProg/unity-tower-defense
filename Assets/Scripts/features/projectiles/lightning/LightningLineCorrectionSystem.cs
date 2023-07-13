using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.components.flags;
using td.components.refs;
using td.features.enemies.components;
using td.utils.ecs;
using UnityEngine;

namespace td.features.projectiles.lightning
{
    /**
     * Corrects trajectory - updates line renderer vertex coordinates according to the coordinates of target enemies
     */
    public class LightningLineCorrectionSystem : IEcsRunSystem
    {
        [InjectWorld] private EcsWorld world;
        
        private readonly EcsFilterInject<Inc<LightningLine, Ref<GameObject>>, Exc<IsDisabled, IsDestroyed>> entities = default; 

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in entities.Value)
            {
                ref var lightningLine = ref entities.Pools.Inc1.Get(entity);
                var lightningLineGO = entities.Pools.Inc2.Get(entity).reference;

                var lineRenderer = lightningLineGO.GetComponent<LineRenderer>();

                lineRenderer.positionCount = lightningLine.length;

                var count = 0;
                for (var index = 0; index < lightningLine.chainEntities.Length && index < lightningLine.length; index++)
                {
                    var chainPackedEntity = lightningLine.chainEntities[index];

                    if (!chainPackedEntity.Unpack(world, out var chainEntity)) continue;

                    if (!world.HasComponent<Enemy>(chainEntity) ||
                        world.HasComponent<IsDisabled>(chainEntity) ||
                        world.HasComponent<IsDestroyed>(chainEntity)) continue;

                    var position = (Vector2)world.GetComponent<Ref<GameObject>>(chainEntity).reference.transform.position;
                    
                    lineRenderer.SetPosition(index, position);
                    count++;
                }


                lineRenderer.positionCount = count;
            }
        }
    }
}