using System;
using UnityEngine.UIElements;

namespace td.features.state.interfaces
{
    public interface IStateExtension
    {
        // IStateChangedEvent GetDirtEvent();
        Type GetEventType();
        bool SendChanges();
        void Clear();
        void Refresh();
#if UNITY_EDITOR
        string GetStateName();
        void DrawStateProperties(VisualElement root);
#endif
    }
}