using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.components;
using td.components.commands;
using td.components.links;
using UnityEngine;

namespace td.systems.commands
{
    public class SmoothRotateExecutor : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<SmoothRotateCommand, TransformLink>> entities = default;
        
        public void Run(IEcsSystems systems)
        {
            foreach (var entity in entities.Value)
            {
                ref var smoothRotate = ref entities.Pools.Inc1.Get(entity);
                ref var transformLink = ref entities.Pools.Inc2.Get(entity);

                var isStarted = smoothRotate.Time <= 0.0001f;

                if (transformLink.transform.rotation == smoothRotate.To ||
                    (
                        isStarted &&
                        (
                            Quaternion.Angle(transformLink.transform.rotation, smoothRotate.To) < smoothRotate.Threshold ||
                            smoothRotate.AngularSpeed > 99f
                        )
                    ))
                {
                    transformLink.transform.rotation = smoothRotate.To;
                    entities.Pools.Inc1.Del(entity);
                }
                else
                {
                    smoothRotate.Time += smoothRotate.AngularSpeed * Time.deltaTime;

                    var newRotate = Quaternion.Lerp(
                        smoothRotate.From,
                        smoothRotate.To,
                        smoothRotate.Time
                    );

                    transformLink.transform.rotation = newRotate;
                }
            }
        }
    }
}