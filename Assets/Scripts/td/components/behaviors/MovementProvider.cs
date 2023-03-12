using System;
using Mitfart.LeoECSLite.UniLeo.Providers;
using UnityEngine;

namespace td.components.behaviors
{
    [Serializable]
    public struct Movement //: IEcsAutoReset<MovableComponent>
    {
        public Vector2 vector;
        public float speed;
    }

    public class MovementProvider : EcsProvider<Movement>
    {
    }
}