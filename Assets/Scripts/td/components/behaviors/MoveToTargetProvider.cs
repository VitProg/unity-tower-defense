using System;
using Mitfart.LeoECSLite.UniLeo.Providers;

namespace td.components.behaviors
{
    [Serializable]
    public struct MoveToTarget
    {
        public float speed;
    }
    
    public class MoveToTargetProvider : EcsProvider<MoveToTarget>
    {
    }

}