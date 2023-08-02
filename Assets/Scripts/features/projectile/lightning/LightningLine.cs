using System;
using Leopotam.EcsLite;

namespace td.features.projectile.lightning
{
    [Serializable]
    public struct LightningLine
    {
        public EcsPackedEntity[] chainEntities;
        public int length;
        
        public float timeRemains;
        public float damageIntervalRemains;
        public bool started;

        // public float findNeighborsTimeRemains;
    }
}