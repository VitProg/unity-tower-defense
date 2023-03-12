using System;
using Mitfart.LeoECSLite.UniLeo.Providers;
using UnityEngine;

namespace td.components.links
{
    [Serializable]
    public struct TransformLink
    {
        public Transform transform;
    }
    
    public class TransformLinkProvider : EcsProvider<TransformLink>
    {
    }
}