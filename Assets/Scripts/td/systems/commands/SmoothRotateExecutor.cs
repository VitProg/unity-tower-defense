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
        private readonly EcsFilterInject<Inc<SmoothRotateCommand, GameObjectLink>> entities = default;
        
        public void Run(IEcsSystems systems)
        {
            foreach (var entity in entities.Value)
            {
                ref var smoothRotate = ref entities.Pools.Inc1.Get(entity);
                ref var gameObjectLink = ref entities.Pools.Inc2.Get(entity);
                var transform = gameObjectLink.gameObject.transform;

                var isStarted = smoothRotate.Time <= 0.0001f;

                if (gameObjectLink.gameObject.transform.rotation.eulerAngles == smoothRotate.To.eulerAngles ||
                    (
                        isStarted &&
                        (
                            Quaternion.Angle(gameObjectLink.gameObject.transform.rotation, smoothRotate.To) < smoothRotate.Threshold ||
                            smoothRotate.AngularSpeed > 99f
                        )
                    ))
                {
                    transform.rotation = smoothRotate.To;
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

                    transform.rotation = newRotate;
                }
            }
        }
    }
}