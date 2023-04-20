using System;

namespace td.components.refs
{
    [Serializable]
    public struct Ref<T>
    {
        public T reference;
    }
}