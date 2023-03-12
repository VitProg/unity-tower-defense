using System;
using Mitfart.LeoECSLite.UniLeo.Providers;
using UnityEngine;

namespace td.components.attributes
{
    [Serializable]
    public struct Position
    {
        public Vector2 position;
    }

    public class PositionProvider : EcsProvider<Position>
    {
    }
}