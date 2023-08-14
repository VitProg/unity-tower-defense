using Leopotam.EcsProto;
using td.features.state;
using UnityEngine;

namespace td.features.costPopup
{
    public class CostPopup_Module : IProtoModuleWithStateEx
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

        public IStateExtension StateEx() => new CostPopup_StateExtension();
    }
}