using System;
using System.Runtime.CompilerServices;
using Leopotam.Types;
using Unity.Mathematics;
using UnityEngine;

namespace td.utils
{
    // ReSharper disable once InconsistentNaming
    public static class mathEx
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsZero(this int2 self) => self is { x: 0, y: 0 };  
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SqrMagnitude(this float2 self) => self.x * self.x + self.y * self.y;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Magnitude(this float2 self) => MathF.Sqrt(SqrMagnitude(self));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsZero(this float2 self) => FloatUtils.IsZero(self.x) && FloatUtils.IsZero(self.y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Normalize(this float2 self)
        {
            var n = math.normalize(self);
            self.x = n.x;
            self.y = n.y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 ToVector2(this float2 self) => new(self.x, self.y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 ToVector3(this float2 self) => new(self.x, self.y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2 ToFloat2(this Vector2 self) => new(self.x, self.y);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2 ToFloat2(this Vector3 self) => new(self.x, self.y);
        
        public static float2 Float2Up = new float2(0f, 1f);
        public static float3 Float3Up = new float3(0f, 1f, 0f);
        
    }
}