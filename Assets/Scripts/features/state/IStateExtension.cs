using System;
using UnityEngine.UIElements;

namespace td.features.state
{
    public interface IStateExtension
    {
        // IStateChangedEvent GetDirtEvent();
        Type GetEventType();
        void SendChanges();
        void Clear();
        void Refresh();
#if UNITY_EDITOR
        void DrawStateProperties(VisualElement root);
#endif
    }
}