using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.components.attributes;
using td.components.behaviors;
using td.utils.ecs;
using UnityEngine;

namespace td.systems.behaviors
{
    public class CursorAsTargetSystem: IEcsRunSystem
    {
        [EcsWorld] private EcsWorld world;
        
        private readonly EcsFilterInject<Inc<MoveToTarget>> entities = default;

        public void Run(IEcsSystems systems)
        {
            var mousePosition = Input.mousePosition;
            
            Debug.Assert(Camera.main != null, "Camera.main != null");
            
            var worldPos = (Vector2)Camera.main.ScreenToWorldPoint(mousePosition);

            foreach (var entity in entities.Value)
            {
                if (world.HasComponent<Target>(entity))
                {
                    world.GetComponent<Target>(entity).target = worldPos;
                }
                else
                {
                    world.AddComponent(entity, new Target()
                    {
                        target = worldPos,
                        gap = Constants.DefaultGap,
                    });
                }
            }
        }
    }
}