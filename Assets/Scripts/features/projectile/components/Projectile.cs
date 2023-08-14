using System;
using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using td.features._common;

namespace td.features.projectile.components
{
    [Serializable]
    public struct Projectile : IProtoAutoReset<Projectile>
    {
        public const string Type = "projectile";
        
        public void AutoReset(ref Projectile c)
        {
            c = default;
            c._id_ = CommonUtils.ID(Type);
        }
        
        // ReSharper disable once InconsistentNaming   
        public uint _id_;
        
        public ProtoPackedEntityWithWorld whoFired;
    }
}