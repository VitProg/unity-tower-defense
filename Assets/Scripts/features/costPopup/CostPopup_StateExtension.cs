using System;
using System.Runtime.CompilerServices;
using Leopotam.EcsProto.QoL;
using td.features.eventBus;
using td.features.state;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor;
using td.utils;
#endif

namespace td.features.costPopup
{

    [Serializable]
    public class CostPopup_StateExtension : IStateExtension
    {
        [DI] private readonly EventBus events;
        private static Type evType = typeof(Event_CostPopup_StateChanged);
        private Event_CostPopup_StateChanged ev;

        #region Private Fields
        private bool visible;
        private uint cost;
        private string title;
        private bool isFine;
        #endregion
        
        #region Getters
        public bool GetVisible() => visible;
        public uint GetCost() => cost;
        public string GetTitle() => title;
        public bool GetIsFine() => isFine;
        #endregion
        
        #region Setters
        public void SetVisible(bool value)
        {
            if (visible == value) return;
            visible = value;
            ev.visible = true;
        }

        public void SetCost(uint value)
        {
            if (cost == value) return;
            cost = value;
            ev.cost = true;
        }

        public void SetTitle(string value)
        {
            if (title == value) return;
            title = value;
            ev.title = true;
        }

        public void SetIsFine(bool value)
        {
            if (isFine == value) return;
            isFine = value;
            ev.isFine = true;
        }
        #endregion

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Type GetEventType() => evType;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Refresh() => ev.All();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            visible = false;
            cost = 0;
            title = "";
            isFine = false;
            ev.All();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SendChanges()
        {
            if (!ev.IsEmpty())
            {
                events.unique.GetOrAdd<Event_CostPopup_StateChanged>() = ev;
            }
            ev = default;
        }

#if UNITY_EDITOR
        public void DrawStateProperties(VisualElement root)
        {
            EditorUtils.DrawTitle("Cost Popup State", true);
            EditorGUI.indentLevel++;
            EditorUtils.DrawProperty("Visible", visible);
            EditorUtils.DrawProperty("Title", title);
            EditorUtils.DrawProperty("Cost", cost);
            EditorUtils.DrawProperty("IsFine", isFine);
            EditorGUI.indentLevel--;
        }
#endif
    }
}