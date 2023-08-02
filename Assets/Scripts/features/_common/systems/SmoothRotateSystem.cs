using System;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.features._common.components;
using td.features._common.flags;
using td.features.state;
using td.utils.ecs;
using UnityEngine;

namespace td.features._common.systems
{
    // todo try to rewrite for usync async - full rotate with one event call
    public class SmoothRotateSystem : EcsIntervalableRunSystem
    {
        private readonly EcsInject<IState> state;
        private readonly EcsInject<Common_Service> common;
        private readonly EcsFilterInject<Inc<ObjectTransform, IsSmoothRotation>, ExcludeNotAlive> entities = default;
        
        public override void IntervalRun(IEcsSystems systems, float deltaTime)
        {
            foreach (var entity in entities.Value)
            {
                ref var transform = ref entities.Pools.Inc1.Get(entity);
                ref var smoothRotate = ref entities.Pools.Inc2.Get(entity);
                // var transform = smoothRotate.targetBody.transform;

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
                    transform.SetRotation(smoothRotate.to);
                    common.Value.RemoveSmoothRotation(entity);
                }
                else
                {
                    smoothRotate.time += smoothRotate.angularSpeed * deltaTime * state.Value.GameSpeed;

                    var newRotate = Quaternion.Lerp(
                        smoothRotate.from,
                        smoothRotate.to,
                        smoothRotate.time
                    );

                    transform.SetRotation(newRotate);
                }
            }
        }

        public SmoothRotateSystem(float interval, float timeShift, Func<float> getDeltaTime) : base(interval, timeShift, getDeltaTime)
        {
        }
    }
}