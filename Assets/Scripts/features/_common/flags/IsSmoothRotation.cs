using System;
using Leopotam.EcsLite;
using UnityEngine;

namespace td.features._common.flags
{
    [Serializable]
    public struct IsSmoothRotation: IFlagComponent, IEcsAutoReset<IsSmoothRotation>
    {
        public Quaternion from;
        public Quaternion to;
        public float angularSpeed;
        public float time;
        public float threshold;
        // public GameObject targetBody;

        public void AutoReset(ref IsSmoothRotation c)
        {
            c.time = 0f;
            c.angularSpeed = 5f;
            c.threshold = 30f;
        }

        public void AutoMerge(ref IsSmoothRotation result, IsSmoothRotation def)
        {
            if (result.angularSpeed <= 0f)
            {
                result.angularSpeed = def.angularSpeed;
            }
            if (result.time <= 0f)
            {
                result.time = def.time;
            }
            if (result.threshold <= 0f)
            {
                result.threshold = def.threshold;
            }
        }
    }
}