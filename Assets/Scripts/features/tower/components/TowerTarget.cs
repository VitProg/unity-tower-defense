using System;
using Leopotam.EcsProto.QoL;

namespace td.features.tower.components
{
    [Serializable]
    public struct TowerTarget
    {
        public ProtoPackedEntityWithWorld targetEntity;
    }
}