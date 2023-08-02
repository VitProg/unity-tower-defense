using System;
using JetBrains.Annotations;

namespace td.features._common.components
{
    [Serializable]
    public struct Ref<T>
    {
        [CanBeNull] public T reference;
    }
}