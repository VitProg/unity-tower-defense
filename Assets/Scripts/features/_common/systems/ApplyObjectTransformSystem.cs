using System;
using Leopotam.EcsLite;
using td.features._common.components;
using td.utils.ecs;
using Unity.Burst;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Jobs;
using UnityEngine.Rendering;

namespace td.features._common.systems
{
    public class ApplyObjectTransformSystem : EcsIntervalableRunSystem
    {
        private readonly EcsInject<Common_Pools> commonPools;
        private readonly EcsInject<Common_Service> common;

        public override void IntervalRun(IEcsSystems systems, float _)
        {
            var filter = commonPools.Value.objectTransformFilter;
            var count = filter.Value.GetEntitiesCount();
            var arr = filter.Value.GetRawEntities();
            for (var index = 0; index < count; index += 1)
            {
                var entity = arr[index];

                ref var t = ref common.Value.GetTransform(entity);

                if (!t.IsChanged()) continue;

                var got = common.Value.GetGOTransform(entity);

                if (t.positionChanged) got.position = t.position;
                if (t.scaleChanged) got.localScale = t.GetScaleVector(got.localScale.z);
                if (t.rotationChanged)
                {
                    if (common.Value.HasTargetBody(entity))
                    {
                        common.Value.GetTargetBodyTransform(entity).rotation = t.rotation;
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
