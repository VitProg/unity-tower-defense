using System;
using Leopotam.EcsLite;
using td.common;
using td.common.ecs;
using UnityEngine;

namespace td.components.commands
{
    [Serializable]
    public struct SmoothRotateCommand: IEcsAutoReset<SmoothRotateCommand>, IEcsAutoMerge<SmoothRotateCommand>
    {
        public Quaternion From;
        public Quaternion To;
        public float AngularSpeed;
        public float Time;
        public float Threshold;

        public void AutoReset(ref SmoothRotateCommand c)
        {
            c.Time = 0f;
            c.AngularSpeed = 5f;
            c.Threshold = 30f;
        }

        public void AutoMerge(ref SmoothRotateCommand result, SmoothRotateCommand def)
        {
            if (result.AngularSpeed <= 0f)
            {
                result.AngularSpeed = def.AngularSpeed;
            }
            if (result.Time <= 0f)
            {
                result.Time = def.Time;
            }
            if (result.Threshold <= 0f)
            {
                result.Threshold = def.Threshold;
            }
        }
    }
}