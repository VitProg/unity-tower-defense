using System;
using Leopotam.EcsLite;

namespace td.features.fx.types
{
    [Serializable]
    public struct WithTargetEntityFX
    {
        public EcsPackedEntityWithWorld entity;
    }
    
    public interface IWithTargetEntityFX : IFX
    {
        
    }
}