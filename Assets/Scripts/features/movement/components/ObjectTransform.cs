using System;
using System.Runtime.CompilerServices;
using Leopotam.Types;
using td.utils;
using Unity.Mathematics;
using UnityEngine;

namespace td.features.movement.components
{
    [Serializable]
    public struct ObjectTransform
    {
        public float2 position;
        public float2 lastPosition;
        public quaternion rotation;
        public float2 lookVector;
        public float2 scale;
        
        public bool positionChanged;
        public bool rotationChanged;
        public bool scaleChanged;

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public bool IsChanged() => positionChanged || rotationChanged || scaleChanged;

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public void ClearChangedStatus()
        {
            positionChanged = false;
            rotationChanged = false;
            scaleChanged = false;
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public void Move(float x, float y)
        {
            if (FloatUtils.IsZero(x) && FloatUtils.IsZero(y)) return;
            lastPosition.x = position.x;
            lastPosition.y = position.y;
            position.x += x;
            position.y += y;
            positionChanged = true;
        }
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public void SetPosition(float x, float y)
        {
            if (FloatUtils.IsEquals(x, position.x) && FloatUtils.IsEquals(y, position.y)) return;
            lastPosition.x = position.x;
            lastPosition.y = position.y;
            position.x = x;
            position.y = y;
            positionChanged = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetPosition(float2 p) => SetPosition(p.x, p.y);
        
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public void SetPosition(Vector2 p) => SetPosition(p.x, p.y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetRotation(Quaternion r) => SetRotation((quaternion)r);
        
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public void SetRotation(quaternion r)
        {
            rotation = r;
            lookVector = math.mul(rotation, mathEx.Float3Up).xy;
            // lookVector = rotation * mathEx.Float2Up;
            rotationChanged = true;
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public void SetScale(float s)
        {
            if (FloatUtils.IsEquals(s, scale.x) && FloatUtils.IsEquals(s, scale.y)) return;
            scale.x = s;
            scale.y = s;
            scaleChanged = true;
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public void SetScale(Vector2 s)
        {
            if (FloatUtils.IsEquals(s.x, scale.x) && FloatUtils.IsEquals(s.y, scale.y)) return;
            scale.x = s.x;
            scale.y = s.y;
            scaleChanged = true;
        }

        public float ScaleScalar => MathFast.Max(scale.x, scale.y);

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public Vector3 GetScaleVector(float z = 1f) => new Vector3(scale.x, scale.y, z);
    }
}