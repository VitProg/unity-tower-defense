using System;

namespace td.features.dragNDrop
{
    [Serializable]
    public struct IsSmoothDragging
    {
        public float speed;
        public bool removeLinearMovementWhenFinished;
    }
}