using UnityEngine;

namespace td.features.impactEnemy.components
{
    public struct ShockingDebuff
    {
        public float timeRemains;
        public Vector3 originalPosition;
        public float shiftPositionTimeRemains;
        public bool started;
    }
}