using System;
using UnityEngine.Serialization;

namespace td.components.commands
{
    [Serializable]
    public struct LoadLevelOuterCommand
    {
        public uint levelNumber;
    }
}