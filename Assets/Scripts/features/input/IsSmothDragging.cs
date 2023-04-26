using System;

namespace td.features.input
{
    [Serializable]
    public struct IsSmoothDragging
    {
        public float speed;
        public bool removeLinearMovementWhenFinished;
    }
}