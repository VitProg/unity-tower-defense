using System;
using td.features._common;

namespace td.features.projectile.attributes
{
    [Serializable]
    public struct DamageAttribute
    {
        public float damage;
        public DamageType type;
    }
}