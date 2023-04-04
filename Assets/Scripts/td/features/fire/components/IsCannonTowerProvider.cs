using System;
using Mitfart.LeoECSLite.UniLeo.Providers;

namespace td.features.fire.components
{
    [Serializable]
    public struct IsCannonTower
    {
        public float damage;
        public float fireRate;
        public float projectileSpeed;
        public float spread;
        public float fireCountdown;
    }
    
    public class IsCannonTowerProvider : EcsProvider<IsCannonTower> {}
}