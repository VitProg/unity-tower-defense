using UnityEngine;

namespace td.common.levelEvents
{
    public struct ShowTextEvent : ILevelEvent
    {
        public uint DelayBefore { get; set; }
        public uint HideAfter;
        public bool WaitEnd;
        public string Text;
        public float FontSize;
        public Color Color;
    }
}