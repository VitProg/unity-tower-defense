using System;
using Mitfart.LeoECSLite.UniLeo.Providers;
using td.common;

namespace td.components.flags
{
    [Serializable]
    public struct IsTower
    {
        public float radius;
        
    }
    
    public class IsTowerProvider : MyEcsProvider<IsTower>
    {
    }
}