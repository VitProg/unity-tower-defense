using System;
using UnityEngine.Serialization;

namespace td.features.projectiles.attributes
{
    [Serializable]
    public struct LightningAttribute
    {
        // aquamarine
        public float duration;
        public float damage;
        public float damageReduction;
        public float damageInterval;
        public int chainReaction;
        public float chainReactionRadius;
        // public int chainRest;
    }
}