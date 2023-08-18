using Leopotam.EcsProto;
using td.features.state;

namespace td.features.pricePopup
{
    public class PricePopup_Module : IProtoModuleWithStateEx
    {
        public void Init(IProtoSystems systems)
        {
            // Debug.Log($"{GetType().Name} Init");
            //
        }

        public IProtoAspect[] Aspects()
        {
            return null;
        }

        public IProtoModule[] Modules()
        {
            return null;
        }

        public IStateExtension StateEx() => new PricePopup_StateExtension();
    }
}