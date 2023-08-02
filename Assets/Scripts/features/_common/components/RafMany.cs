using System;

namespace td.features._common.components
{
    [Serializable]
    public struct RefMany<T>
    {
        public T[] references;
        public int count;
    }
}