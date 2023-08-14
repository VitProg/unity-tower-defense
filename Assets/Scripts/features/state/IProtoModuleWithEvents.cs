using System;
using Leopotam.EcsProto;

namespace td.features.state
{
    public interface IProtoModuleWithStateEx : IProtoModule
    {
        public IStateExtension StateEx();
    }
}