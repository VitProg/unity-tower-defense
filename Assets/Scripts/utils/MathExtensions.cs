using System.Runtime.CompilerServices;
using Unity.Mathematics;

namespace td.utils
{
    public static class MathExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsZero(this int2 self) => self is { x: 0, y: 0 };  
        
        
    }
}