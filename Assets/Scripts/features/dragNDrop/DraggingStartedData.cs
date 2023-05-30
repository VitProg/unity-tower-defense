using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace td.features.dragNDrop
{
    public struct DraggingStartedData
    {
        public Vector3 startedPosition;
        public double startedTime;
        public string startedLayer;
        public Transform parentContainer;
    }
}