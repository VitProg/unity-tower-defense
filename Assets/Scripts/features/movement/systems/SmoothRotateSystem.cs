using System;
using Leopotam.EcsProto.QoL;
using td.features.state;
using td.utils;
using td.utils.ecs;
using UnityEngine;

namespace td.features.movement.systems
{
    // todo try to rewrite for usync async - full rotate with one event call
    public class SmoothRotateSystem : ProtoIntervalableRunSystem
    {
        [DI] private Movement_Aspect aspect;
        [DI] private Movement_Service movementService;
        [DI] private State state;
        
        public override void IntervalRun(float deltaTime)
        {
            foreach (var entity in aspect.itSmoothRotation)
            {
                ref var transform = ref movementService.GetTransform(entity);
                ref var smoothRotate = ref aspect.isSmoothRotationPool.Get(entity);

                var isStarted = smoothRotate.time <= Constants.ZeroFloat;

                // todo
                if (((Quaternion)transform.rotation).eulerAngles == smoothRotate.to.eulerAngles ||
                    (
                        isStarted &&
                        (
                            Quaternion.Angle(transform.rotation, smoothRotate.to) < smoothRotate.threshold ||
                            smoothRotate.angularSpeed > 99f
                        )
                    ))
                {
                    transform.SetRotation(smoothRotate.to);
                    aspect.isSmoothRotationPool.Del(entity);
                }
                else
                {
                    smoothRotate.time += smoothRotate.angularSpeed * deltaTime * state.GetGameSpeed();

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