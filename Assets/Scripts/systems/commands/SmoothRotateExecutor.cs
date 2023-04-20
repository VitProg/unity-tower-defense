using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.components;
using td.components.commands;
using td.components.refs;
using UnityEngine;

namespace td.systems.commands
{
    public class SmoothRotateExecutor : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<SmoothRotation, Ref<GameObject>>> entities = default;
        
        public void Run(IEcsSystems systems)
        {
            foreach (var entity in entities.Value)
            {
                ref var smoothRotate = ref entities.Pools.Inc1.Get(entity);
                ref var gameObjectLink = ref entities.Pools.Inc2.Get(entity);
                var transform = gameObjectLink.reference.transform;

                var isStarted = smoothRotate.Time <= Constants.ZeroFloat;

                if (gameObjectLink.reference.transform.rotation.eulerAngles == smoothRotate.To.eulerAngles ||
                    (
                        isStarted &&
                        (
                            Quaternion.Angle(gameObjectLink.reference.transform.rotation, smoothRotate.To) < smoothRotate.Threshold ||
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