using System;
using System.Runtime.CompilerServices;
using td.utils;
using UnityEngine;

namespace td.features.movement.components
{
    [Serializable]
    public struct Movement
    {
        public Vector2 from;
        public Vector2 target;
        public float fromToTargetDistanse;
        public Vector2 nextTarget;
        public float targetToNextDistanse;
        public float gapSqr;
        public float speed;
        public Vector2 speedV;
        public bool resetAnchoredPositionZ;
        public bool speedOfGameAffected;

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public void SetSpeed(float s, Vector2 v)
        {
            speed = s;
            var n = v.normalized;
            speedV.x = s * n.x;
            speedV.y = s * n.y;
        }        
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public void SetSpeed(float s)
        {
            speed = s;
            var toTarget = target - from;
            if (!toTarget.IsZero())
            {
                toTarget.Normalize();
                speedV.x = toTarget.x * s;
                speedV.y = toTarget.y * s;
               
            }
            else
            {
                speedV.x = 0;
                speedV.y = 0;
            }
        }
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public void SetSpeed(float s, float x, float y)
        {
            speed = s;
            if (Mathf.Abs(x + y) > 1f)
            {
                SetSpeed(s, new Vector2(x, y));
                return;
            }
            speedV.x = s * x;
            speedV.y = s * y;
        }
        
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public void SetSpeed(float s, Quaternion rotation)
        {
            speed = s;
            var n = Mathf.Abs(rotation.x + rotation.y) > 1f
                ? rotation.normalized * Vector2.up // sqrt
                : rotation * Vector2.up;
            speedV.x = s * n.x;
            speedV.y = s * n.y;
        }        
        
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public void SetSpeed(Vector2 s)
        {
            speed = s.magnitude;
            speedV.x = s.x;
            speedV.y = s.y;
        }

        public void SetGap(float gap)
        {
            gapSqr = gap * gap;
        }       
        
        public void SetSqrGap(float sqrGap)
        {
            gapSqr = sqrGap;
        }
    }
}