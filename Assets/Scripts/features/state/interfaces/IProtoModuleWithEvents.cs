using Leopotam.EcsProto;

namespace td.features.state.interfaces
{
    public interface IProtoModuleWithStateEx : IProtoModule
    {
        public IStateExtension StateEx();
    }
}