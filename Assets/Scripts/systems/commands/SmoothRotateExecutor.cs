using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.components;
using td.components.commands;
using td.components.flags;
using td.components.refs;
using td.features.state;
using td.utils.ecs;
using UnityEngine;

namespace td.systems.commands
{
    public class SmoothRotateExecutor : IEcsRunSystem
    {
        [Inject] private State state;
        private readonly EcsFilterInject<Inc<SmoothRotation>, Exc<IsDestroyed>> entities = default;
        
        public void Run(IEcsSystems systems)
        {
            foreach (var entity in entities.Value)
            {
                ref var smoothRotate = ref entities.Pools.Inc1.Get(entity);
                var transform = smoothRotate.targetBody.transform;

                var isStarted = smoothRotate.time <= Constants.ZeroFloat;

                if (transform.rotation.eulerAngles == smoothRotate.to.eulerAngles ||
                    (
                        isStarted &&
                        (
                            Quaternion.Angle(transform.rotation, smoothRotate.to) < smoothRotate.threshold ||
                            smoothRotate.angularSpeed > 99f
                        )
                    ))
                {
                    transform.rotation = smoothRotate.to;
                    entities.Pools.Inc1.Del(entity);
                }
                else
                {
                    smoothRotate.time += smoothRotate.angularSpeed * Time.deltaTime * state.GameSpeed;

                    var newRotate = Quaternion.Lerp(
                        smoothRotate.from,
                        smoothRotate.to,
                        smoothRotate.time
                    );

                    transform.rotation = newRotate;
                }
            }
        }
    }
}