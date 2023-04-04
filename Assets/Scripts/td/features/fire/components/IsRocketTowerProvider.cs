using System;
using Mitfart.LeoECSLite.UniLeo.Providers;
using UnityEngine;

namespace td.features.fire.components
{
    [Serializable]
    public struct IsRocketTower
    {
        public float damage;
        public float damageRange;
        public float fireRate;
        [HideInInspector] public float fireCountdown;
        public float turnSpeed;
        public float maxEnergy;
        public float maxSpeed;
    }
    
    public class IsRocketTowerProvider : EcsProvider<IsRocketTower> {}
}