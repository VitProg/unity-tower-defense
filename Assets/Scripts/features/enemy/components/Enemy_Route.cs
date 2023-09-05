using System;
using UnityEngine.Serialization;

namespace td.features.enemy.components
{
    [Serializable]
    public struct Enemy_Route
    {
        public int routeIdx;
        public int step;

        public void Deconstruct(out int routeIdx, out int step) {
            routeIdx = this.routeIdx;
            step = this.step;
        }
    }
}