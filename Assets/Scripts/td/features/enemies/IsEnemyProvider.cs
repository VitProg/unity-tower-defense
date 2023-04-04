using System;
using Mitfart.LeoECSLite.UniLeo.Providers;
using UnityEngine;

namespace td.features.enemies
{
    [Serializable]
    public struct IsEnemy
    {
        [HideInInspector]
        public float distanceToKernel;
    }
    
    public class IsEnemyProvider : EcsProvider<IsEnemy>
    {
    }
}