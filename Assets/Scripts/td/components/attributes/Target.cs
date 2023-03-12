using System;
using UnityEngine;

namespace td.components.attributes
{
    [Serializable]
    public struct Target //: IEcsAutoReset<MovableComponent>
    {
        public Vector2 target;
        public float gap;
    }
}