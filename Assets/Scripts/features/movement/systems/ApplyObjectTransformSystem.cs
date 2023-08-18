using System;
using Leopotam.EcsProto.QoL;
using td.utils.ecs;

namespace td.features.movement.systems
{
    public class ApplyObjectTransformSystem : ProtoIntervalableRunSystem
    {
        [DI] private Movement_Aspect aspect;
        [DI] private Movement_Service movementService;

        public override void IntervalRun(float _)
        {
            foreach (var entity in aspect.itObjectTransform)
            {
                ref var t = ref movementService.GetTransform(entity);

                if (!t.IsChanged()) continue;

                var got = movementService.GetGOTransform(entity);

                if (t.positionChanged) got.position = t.position;
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
