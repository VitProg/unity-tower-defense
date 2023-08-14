using System;
using Leopotam.EcsProto.QoL;

namespace td.features.fx.types
{
    [Serializable]
    public struct WithTargetEntityFX
    {
        public ProtoPackedEntityWithWorld entity;
    }
    
    public interface IWithTargetEntityFX : IFX
    {
        
    }
}