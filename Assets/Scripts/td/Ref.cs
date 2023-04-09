using System;
using Mitfart.LeoECSLite.UniLeo.Providers;

namespace td
{
    [Serializable]
    public struct Ref<T>
    {
        public T reference;
    }
}