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
        private readonly EcsFilterInject<Inc<MoveToTarget>> entities = default;

        public void Run(IEcsSystems systems)
        {
            var mousePosition = Input.mousePosition;
            
            Debug.Assert(Camera.main != null, "Camera.main != null");
            
            var worldPos = (Vector2)Camera.main.ScreenToWorldPoint(mousePosition);

            foreach (var entity in entities.Value)
            {
                if (EntityUtils.HasComponent<Target>(systems, entity))
                {
                    EntityUtils.GetComponent<Target>(systems, entity).target = worldPos;
                }
                else
                {
                    EntityUtils.AddComponent(systems, entity, new Target()
                    {
                        target = worldPos,
                        gap = Constants.DefaultGap,
                    });
                }
            }
        }
    }
}