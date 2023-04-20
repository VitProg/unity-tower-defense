using System;
using Mitfart.LeoECSLite.UniLeo.Providers;
using td.common;

namespace td.features.towers
{
    [Serializable]
    [GenerateProvider]
    public struct CannonTower
    {
        public float damage;
        public float fireRate;
        public float projectileSpeed;
        public float spread;
        public float fireCountdown;
    }
}