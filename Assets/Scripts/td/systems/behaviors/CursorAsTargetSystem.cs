using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.components.attributes;
using td.components.behaviors;
using UnityEngine;

namespace td.systems.behaviors
{
    public class CursorAsTargetSystem: IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<MoveToTarget>> entities = default;
        private readonly EcsPoolInject<Target> targetPointPool = default;

        public void Run(IEcsSystems systems)
        {
            var mousePosition = Input.mousePosition;
            
            Debug.Assert(Camera.main != null, "Camera.main != null");
            
            var worldPos = (Vector2)Camera.main.ScreenToWorldPoint(mousePosition);

            foreach (var entity in entities.Value)
            {
                if (targetPointPool.Value.Has(entity))
                {
                    targetPointPool.Value.Get(entity).target = worldPos;
                }
                else
                {
                    ref var targetPoint = ref targetPointPool.Value.Add(entity);
                    targetPoint.target = worldPos;
                    targetPoint.gap = Constants.DefaultGap;
                }
            }
        }
    }
}