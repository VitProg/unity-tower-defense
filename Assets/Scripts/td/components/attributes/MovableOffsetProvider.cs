using System;
using Mitfart.LeoECSLite.UniLeo.Providers;
using UnityEngine;

namespace td.components.attributes
{
    [Serializable]
    public struct MovableOffset
    {
        public Vector2 offset;
    }
    
    public class MovableOffsetProvider : EcsProvider<MovableOffset>
    {
    }
}