using System;
using NaughtyAttributes;

namespace td.common
{
    [Serializable]
    public struct Int2
    {
        [AllowNesting] public int x;
        [AllowNesting] public int y;

        public Int2(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public bool Empty => x == 0 && y == 0;
        public static Int2 Zero => new(0, 0);


        public static Int2 operator + (Int2 a, Int2 b) {
            return new Int2 (a.x + b.x, a.y + b.y);
        }

        public static Int2 operator - (Int2 a, Int2 b) {
            return new Int2 (a.x - b.x, a.y - b.y);
        }

        public static Int2 operator * (Int2 a, int multiplier) {
            return new Int2 (a.x * multiplier, a.y * multiplier);
        }

        public override string ToString()
        {
            return $"[{x};{y}]";
        }

        public static bool operator ==(Int2 a, Int2 b) => a.x == b.x && a.y == b.y;
        public static bool operator !=(Int2 a, Int2 b) => !(a == b);
    }
}