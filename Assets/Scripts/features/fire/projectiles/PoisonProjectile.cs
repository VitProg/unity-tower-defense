using System;
using UnityEngine.Serialization;

namespace td.features.fire.projectiles
{
    [Serializable]
    public struct PoisonProjectile
    {
        [FormerlySerializedAs("damage")] public float damageInterval;
        public float interval;
        public float duration;
    }
}