using System;
using Mitfart.LeoECSLite.UniLeo.Providers;
using td.features.enemies;

namespace td.components.flags
{
    [Serializable]
    public struct IsBuilding
    {
    }
    
    public class IsBuildingProvider : EcsProvider<IsEnemy>
    {
    }
}