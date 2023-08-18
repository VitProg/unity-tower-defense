using System.Runtime.CompilerServices;
using Leopotam.Types;
using UnityEngine;

namespace td.features.fx.types
{
    public struct WithTransformFX
    {
        public Vector2 position;
        public Vector2 scale;
        public Quaternion rotation;
        
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetPosition(float x, float y)
        {
            position.x = x;
            position.y = y;
            positionChanged = true;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetPosition(Vector2 p)
        {
            position.x = p.x;
            position.y = p.y;
            positionChanged = true;
        }
        
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public void SetRotation(Quaternion r)
        {
            rotation = r;
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

    public interface IWithTransform
    {
        
    }
}