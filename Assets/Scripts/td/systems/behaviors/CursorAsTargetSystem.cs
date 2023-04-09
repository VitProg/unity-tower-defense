using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.components.behaviors;
using td.utils.ecs;
using UnityEngine;

namespace td.systems.behaviors
{
    public class CursorAsTargetSystem: IEcsRunSystem
    {
        [EcsWorld] private EcsWorld world;
        
        private readonly EcsFilterInject<Inc<LinearMovementToTarget>> entities = default;

        public void Run(IEcsSystems systems)
        {
            var mousePosition = Input.mousePosition;
            
            Debug.Assert(Camera.main != null, "Camera.main != null");
            
            var worldPos = (Vector2)Camera.main.ScreenToWorldPoint(mousePosition);

            foreach (var entity in entities.Value)
            {
                if (world.HasComponent<TargetPoint>(entity))
                {
                    world.GetComponent<TargetPoint>(entity).Target = worldPos;
                }
                else
                {
                    world.AddComponent(entity, new TargetPoint()
                    {
                        Target = worldPos,
                        Gap = Constants.DefaultGap,
                    });
                }
            }
        }
    }
}