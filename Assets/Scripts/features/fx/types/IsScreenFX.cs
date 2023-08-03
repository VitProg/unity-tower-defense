using System;
using UnityEngine;

namespace td.features.fx.types
{
    [Serializable]
    public struct IsScreenFX
    {
        public Vector2 position;
        public float scale;
    }

    public interface IScreenFX : IFX
    {
        
    }
}