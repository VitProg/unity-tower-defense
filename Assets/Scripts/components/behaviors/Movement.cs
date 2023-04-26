using System;
using td.common;
using UnityEngine;

namespace td.components.behaviors
{
    [Serializable]
    [GenerateProvider]
    public struct Movement //: IEcsAutoReset<MovableComponent>
    {
        public Vector2 vector;
        public float speed;
    }
}