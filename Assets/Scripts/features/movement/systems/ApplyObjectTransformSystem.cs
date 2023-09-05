using System;
using Leopotam.EcsProto.QoL;
using td.features._common;
using td.utils;
using td.utils.ecs;
using UnityEngine;

namespace td.features.movement.systems
{
    public class ApplyObjectTransformSystem : ProtoIntervalableRunSystem
    {
        [DI] private Movement_Aspect aspect;
        [DI] private Movement_Service movementService;
        [DI] private Common_Service common;

        public override void IntervalRun(float _)
        {
            foreach (var entity in aspect.itObjectTransform)
            {
                ref var t = ref movementService.GetTransform(entity);

                if (!t.IsChanged()) continue;

                var got = common.GetGOTransform(entity);

                if (t.positionChanged) got.position = t.position.ToVector3();
                if (t.scaleChanged) got.localScale = t.GetScaleVector(got.localScale.z);
                if (t.rotationChanged)
                {
                    if (movementService.HasTargetBody(entity))
                    {
                        movementService.GetTargetBodyTransform(entity).rotation = t.rotation;
                    }
                    else
                    {
                        got.rotation = t.rotation;
                    }
                }

                t.ClearChangedStatus();
            }
        }

        public ApplyObjectTransformSystem(float interval, float timeShift, Func<float> getDeltaTime) : base(interval, timeShift, getDeltaTime)
        {

        }
    }
}
