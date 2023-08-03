using System;
using td.features.fx.types;
using UnityEngine;

namespace td.features.fx.effects
{
    [Serializable]
    public struct FireFX : IPositionFX
    {
        public Color color;
    }

    public class SerializableAttribute : Attribute
    {
    }
}