using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using td.features._common.components;
using UnityEngine;

namespace td.features._common
{
    public class Common_Aspect : ProtoAspectInject
    {
        public ProtoPool<Ref<GameObject>> refGoPool = default;

    }
}