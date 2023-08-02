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
        // private readonly int evenOdd;
        // private readonly int jobThreshold;
        // private readonly int desiredJobCount;
        // private bool passOdd = true;
        // private readonly float wantFPS;

        // protected override float GetNewInterval()
        // {
        //     // var count = commonPools.Value.objectTransformFilter.Value.GetEntitiesCount();
        //     var newInterval = base.GetNewInterval();
        //
        //     var fps = 1f / Time.smoothDeltaTime;
        //     var d = 1f;
        //
        //     if (fps < 60f)
        //     {
        //         // d = 60 / fps * 1.15f;
        //         d = fps * 0.0184f;
        //     }
        //
        //     if (d > 1f) newInterval *= d;
        //     // if (evenOdd > -1 && count > evenOdd) newInterval *= 0.5f;
        //
        //     // Debug.Log($"ApplyTransform: {count}; {newInterval}; {fps}; {d}");
        //
        //     return newInterval;
        // }

        public ApplyObjectTransformSystem(float interval, float timeShift, Func<float> getDeltaTime/*, int evenOdd = -1,
            int jobThreshold = -1, int desiredJobCount = 3*/) : base(interval, timeShift, getDeltaTime)
        {
            // this.evenOdd = evenOdd;
            // this.jobThreshold = jobThreshold;
            // this.desiredJobCount = desiredJobCount;
        }

        public override void IntervalRun(IEcsSystems systems, float _)
        {
            var count = commonPools.Value.objectTransformFilter.Value.GetEntitiesCount();

            // if (jobThreshold > -1 && count > jobThreshold)
            // {
            //     InJob(count);
            // }
            // else
            // {
                InMainThread(count);
            // }


            // Debug.Log("ApplyTransform: " + count);
        }

        private void InMainThread(int count)
        {
            // passOdd = !passOdd;
            // var checkEvenOdd = evenOdd > -1 && count > 20;
            // var start = 0;//checkEvenOdd && passOdd ? 1 : 0;
            // var step = 1;//checkEvenOdd ? 2 : 1;
            var arr = commonPools.Value.objectTransformFilter.Value.GetRawEntities();
            for (var index = 0; index < count; index += 1)
            {
                var entity = arr[index];

                ref var t = ref common.Value.GetTransform(entity);

                if (!ObjectTransform.IsChanged(t)) continue;

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

        /*private void InJob(int count)
        {
            var arraySize = count;
            var jobDatum = new NativeArray<JobItem>(arraySize, Allocator.TempJob);
            var jobTransforms = new TransformAccessArray(arraySize, desiredJobCount);

            var arr = commonPools.Value.objectTransformFilter.Value.GetRawEntities();
            var datumIndex = 0;
            for (var index = 0; index < count; index++)
            {
                var entity = arr[index];

                ref var t = ref common.Value.GetTransform(entity);

                if (!ObjectTransform.IsChanged(t)) continue;

                var got = common.Value.GetGOTransform(entity);
                var hasTargetBody = common.Value.HasTargetBody(entity);

                var chackMain = t.positionChanged || t.scaleChanged || (t.rotationChanged && !hasTargetBody);

                if (chackMain)
                {
                    var jobItem = new JobItem()
                    {
                        bp = t.positionChanged,
                        br = t.rotationChanged,
                        bs = t.scaleChanged,
                    };
                    if (t.positionChanged)
                    {
                        jobItem.px = t.position.x;
                        jobItem.py = t.position.y;
                    }

                    if (t.scaleChanged)
                    {
                        jobItem.sx = t.scale.x;
                        jobItem.sy = t.scale.y;
                    }

                    if (t.rotationChanged && !hasTargetBody)
                    {
                        jobItem.rx = t.rotation.x;
                        jobItem.ry = t.rotation.y;
                        jobItem.rz = t.rotation.z;
                        jobItem.rw = t.rotation.w;
                    }

                    if (datumIndex >= arraySize)
                    {
                        arraySize = Mathf.RoundToInt(arraySize * 1.5f);
                        jobDatum.ResizeArray(arraySize);
                        jobTransforms.ResizeArray(arraySize);
                    }

                    jobDatum[datumIndex] = jobItem;
                    jobTransforms.Add(got);
                    datumIndex++;
                }

                if (t.rotationChanged && hasTargetBody)
                {
                    if (datumIndex >= arraySize)
                    {
                        arraySize = Mathf.RoundToInt(arraySize * 1.5f);
                        jobDatum.ResizeArray(arraySize);
                        jobTransforms.ResizeArray(arraySize);
                    }

                    var tb = common.Value.GetTargetBodyTransform(entity);
                    jobDatum[datumIndex] = new JobItem()
                    {
                        bp = false,
                        bs = false,
                        br = true,
                        rx = t.rotation.x,
                        ry = t.rotation.y,
                        rz = t.rotation.z,
                        rw = t.rotation.w,
                    };
                    jobTransforms.Add(tb);
                    datumIndex++;
                }

                t.ClearChangedStatus();
            }

            var newJob = new Job()
            {
                datum = jobDatum,
                count = (uint)datumIndex - 1,
            };

            var jobHandle = newJob.Schedule(jobTransforms);
            jobHandle.Complete();

            jobDatum.Dispose();
        }
    }

    [BurstCompile]
    internal struct JobItem
    {
        public bool bp;
        public float px;
        public float py;

        public bool br;
        public bool hasBody;
        public float rx;
        public float ry;
        public float rz;
        public float rw;

        public bool bs;
        public float sx;
        public float sy;
    }

    [BurstCompile]
    internal struct Job : IJobParallelForTransform
    {
        [NativeDisableParallelForRestriction] public NativeArray<JobItem> datum;
        [NativeDisableParallelForRestriction] public uint count;

        public void Execute(int index, TransformAccess transform)
        {
            if (index >= count) return;

            var item = datum[index];

            if (item.br)
            {
                transform.rotation = new Quaternion(datum[index].rx, datum[index].ry, datum[index].rz, datum[index].rw);
            }

            if (item.bp)
            {
                transform.position = new Vector3(item.px, item.py, transform.position.z);
            }

            if (item.bs)
            {
                transform.localScale = new Vector3(item.sx, item.sy, 1f);
            }
        }*/
    }
}
