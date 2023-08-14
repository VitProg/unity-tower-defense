using System;
using Leopotam.EcsProto.QoL;

namespace td.features.projectile.lightning
{
    [Serializable]
    public struct Lightning
    {
        public ProtoPackedEntityWithWorld[] chainEntities;
        public int length;
        
        public float timeRemains;
        public float damageIntervalRemains;
        public bool started;

        // public float findNeighborsTimeRemains;
    }
}