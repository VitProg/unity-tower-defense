using Leopotam.EcsProto;
using td.features.priceTimePopup.systems;
using td.features.state;
using td.features.state.interfaces;

namespace td.features.priceTimePopup
{
    public class PriceTimePopup_Module : IProtoModuleWithStateEx
    {
        public void Init(IProtoSystems systems)
        {
            systems
                .AddSystem(new PriceTimePopup_System())
                ;
        }

        public IProtoAspect[] Aspects()
        {
            return null;
        }

        public IProtoModule[] Modules()
        {
            return null;
        }

        public IStateExtension StateEx() => new PriceTimePopup_State();
    }
}