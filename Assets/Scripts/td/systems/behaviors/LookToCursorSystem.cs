using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.components.attributes;
using td.components.behaviors;
using td.utils.ecs;
using UnityEngine;

namespace td.systems.behaviors
{
    public class LookToCursorSystem: IEcsRunSystem
    {
        [EcsPool] private EcsPool<Target> targetPointPool;
        
        private readonly EcsFilterInject<Inc<Movement, Position>> entities = default;

        public void Run(IEcsSystems systems)
        {
            var mousePosition = Input.mousePosition;
            
            Debug.Assert(Camera.main != null, "Camera.main != null");
            
            var worldPos = (Vector2)Camera.main.ScreenToWorldPoint(mousePosition);

            foreach (var entity in entities.Value)
            {
                ref var movable = ref entities.Pools.Inc1.Get(entity);
                ref var positionLink = ref entities.Pools.Inc2.Get(entity);
                //ref var targetPoint = ref entities.Pools.Inc3.Get(entity);
                
                movable.vector = worldPos - positionLink.position;
                movable.vector.Normalize();

                if (targetPointPool.Has(entity))
                {
                    targetPointPool.Get(entity).target = worldPos;
                }
                else
                {
                    ref var targetPoint = ref targetPointPool.Add(entity);
                    targetPoint.target = worldPos;
                    targetPoint.gap = Constants.DefaultGap;
                } 
                // Debug.Log($"LookToCursorSystem: movable.vector: {movable.vector} for entity #{entity}");
            }
        }
    }
}