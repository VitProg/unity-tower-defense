using System;

namespace td.features.projectiles.attributes
{
    [Serializable]
    public struct SlowingAttribute
    {
        public float speedMultipler;
        public float duration;
    }
}