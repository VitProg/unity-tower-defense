using UnityEngine;

namespace td.features.projectile.explosion
{
    public struct Explosion
    {
        public Vector2 position;
        public float progress;
        public float currentDiameter;
        public float lastCalcDiameter;
        public float diameterIncreaseSpeed;
    }
}