using System.Runtime.CompilerServices;
using Leopotam.Types;

namespace td.features.fx.types
{
    public struct WithDurationFX
    {
        public bool withDuration;
        public float duration;
        internal float remainingTime;

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public void SetDuration(float? d)
        {
            withDuration = d.HasValue;
            var nd = d ?? 0f;
            remainingTime = d.HasValue
                ? MathFast.Max(0f, remainingTime + (d.Value - nd))
                : 0f;
            duration = nd;
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public void AddDuration(float? d)
        {
            if (!d.HasValue) return;
            SetDuration(duration + d.Value);
        }
        
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public void AddOrClearDuration(float? d)
        {
            if (!d.HasValue)
            {
                withDuration = false;
                duration = 0f;
                remainingTime = 0;
                return;
            }
            SetDuration(duration + d.Value);
        }
    }
}