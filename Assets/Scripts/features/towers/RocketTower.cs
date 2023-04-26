using System;
using td.common;
using UnityEngine;

namespace td.features.towers
{
    [Serializable]
    [GenerateProvider]
    public struct RocketTower
    {
        public float damage;
        public float damageRange;
        public float fireRate;
        [HideInInspector] public float fireCountdown;
        public float turnSpeed;
        public float maxEnergy;
        public float maxSpeed;
    }
}