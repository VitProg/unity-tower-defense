using Leopotam.EcsProto;
using UnityEngine;

namespace td.features.spriteAnimator
{
    public class SpriteAnimator_Module : IProtoModule
    {
        public void Init(IProtoSystems systems)
        {
            // Debug.Log($"{GetType().Name} Init");
            //todo add service for agrregate all animators updates
        }

        public IProtoAspect[] Aspects()
        {
            return null;
        }

        public IProtoModule[] Modules()
        {
            return null;
        }
    }
}