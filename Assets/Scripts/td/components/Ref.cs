using System;

namespace td.components
{
    [Serializable]
    public struct Ref<T>
    {
        public T reference;
    }
}