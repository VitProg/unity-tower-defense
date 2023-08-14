using System;
using Leopotam.EcsProto;

namespace td.features.eventBus
{
    public interface IProtoModuleWithEvents : IProtoModule
    {
        public Type[] Events();
    }
}