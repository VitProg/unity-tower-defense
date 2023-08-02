using System;

namespace td.features.dragNDrop.flags
{
    [Serializable]
    public struct IsSmoothDragging
    {
        public float speed;
        public bool removeLinearMovementWhenFinished;
    }
}