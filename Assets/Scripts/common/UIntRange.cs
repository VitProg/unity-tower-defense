using System;

namespace td.common
{
    public struct UIntRange
    {
        public readonly uint min;
        public readonly uint max;

        public UIntRange(uint min, uint max)
        {
            if (max < min)
            {
                throw new ArgumentOutOfRangeException(nameof(max), "The maximum value cannot be less than the minimum!");
            }

            this.min = min;
            this.max = max;
        }
    }
}