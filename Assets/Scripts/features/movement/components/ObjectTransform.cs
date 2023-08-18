using System.Runtime.CompilerServices;
using Leopotam.Types;
using UnityEngine;

namespace td.features.movement.components
{
    public struct ObjectTransform
    {
        public Vector2 position;
        public Vector2 lastPosition;
        public Quaternion rotation;
        public Vector2 lookVector;
        public Vector2 scale;
        
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
            lastPosition.x = position.x;
            lastPosition.y = position.y;
            position.x += x;
            position.y += y;
            positionChanged = true;
        }
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public void SetPosition(float x, float y)
        {
            lastPosition.x = position.x;
            lastPosition.y = position.y;
            position.x = x;
            position.y = y;
            positionChanged = true;
        }
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public void SetPosition(Vector2 p)
        {
            lastPosition.x = position.x;
            lastPosition.y = position.y;
            position.x = p.x;
            position.y = p.y;
            positionChanged = true;
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public void SetRotation(Quaternion r)
        {
            rotation = r;
            lookVector = rotation * Vector2.up;
            rotationChanged = true;
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public void SetScale(float s)
        {
            scale.x = s;
            scale.y = s;
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public void SetScale(Vector2 s)
        {
            scale.x = s.x;
            scale.y = s.y;
        }

        public float ScaleScalar => MathFast.Max(scale.x, scale.y);

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public Vector3 GetScaleVector(float z = 1f) => new Vector3(scale.x, scale.y, z);
    }
}