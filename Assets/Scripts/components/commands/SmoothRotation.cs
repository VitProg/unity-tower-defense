using System;
using Leopotam.EcsLite;
using td.common;
using td.common.ecs;
using UnityEngine;
using UnityEngine.Serialization;

namespace td.components.commands
{
    [Serializable]
    public struct SmoothRotation: IEcsAutoReset<SmoothRotation>, IEcsAutoMerge<SmoothRotation>
    {
        public Quaternion from;
        public Quaternion to;
        public float angularSpeed;
        public float time;
        public float threshold;

        public void AutoReset(ref SmoothRotation c)
        {
            c.time = 0f;
            c.angularSpeed = 5f;
            c.threshold = 30f;
        }

        public void AutoMerge(ref SmoothRotation result, SmoothRotation def)
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