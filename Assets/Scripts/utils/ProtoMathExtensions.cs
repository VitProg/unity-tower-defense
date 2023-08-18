using System.Runtime.CompilerServices;
using Leopotam.Types;

namespace td.utils
{
    public static class ProtoMathExtensions
    {
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public static Vec2f ToVec2f (this UnityEngine.Vector3 lhs) {
            return new(lhs.x, lhs.y);
        }
    }

}