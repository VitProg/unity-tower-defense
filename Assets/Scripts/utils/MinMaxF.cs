using System;

namespace td.utils
{
    [Serializable]
    public struct MinMaxF
    {
        public float min;
        public float max;

        public MinMaxF(float min, float max)
        {
            this.min = min;
            this.max = max;
        }
    }
}