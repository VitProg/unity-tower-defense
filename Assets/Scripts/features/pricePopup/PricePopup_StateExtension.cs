#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.Runtime.CompilerServices;
using Leopotam.EcsProto.QoL;
using td.features.eventBus;
using td.features.state;
using td.utils;
using UnityEngine.UIElements;

namespace td.features.pricePopup
{

    [Serializable]
    public class PricePopup_StateExtension : IStateExtension
    {
        [DI] private readonly EventBus events;
        private static Type evType = typeof(Event_PricePopup_StateChanged);
        private Event_PricePopup_StateChanged ev;

        #region Private Fields
        private bool visible;
        private uint price;
        private string title;
        private bool isFine;
        #endregion
        
        #region Getters
        public bool GetVisible() => visible;
        public uint GetPrice() => price;
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

        public void SetPrice(uint value)
        {
            if (price == value) return;
            price = value;
            ev.price = true;
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
            price = 0;
            title = "";
            isFine = false;
            ev.All();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SendChanges()
        {
            if (!ev.IsEmpty())
            {
                events.unique.GetOrAdd<Event_PricePopup_StateChanged>() = ev;
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
            EditorUtils.DrawProperty("Cost", price);
            EditorUtils.DrawProperty("IsFine", isFine);
            EditorGUI.indentLevel--;
        }
#endif
    }
}