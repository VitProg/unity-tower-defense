using System;

namespace td.features.projectile.attributes
{
    [Serializable]
    public struct SlowingAttribute
    {
        // blue
        public float speedMultipler;
        public float duration;
    }
}