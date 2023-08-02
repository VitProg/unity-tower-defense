using System;
using Leopotam.EcsLite;

namespace td.features.tower.components
{
    // todo move to Tower
    [Serializable]
    public struct TowerTarget
    {
        public EcsPackedEntity targetEntity;
    }
}