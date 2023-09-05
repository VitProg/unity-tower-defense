#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.Runtime.CompilerServices;
using Leopotam.EcsProto.QoL;
using td.features.eventBus;
using td.features.state;
using td.features.state.interfaces;
using td.utils;
using UnityEngine.UIElements;

namespace td.features.priceTimePopup
{

    [Serializable]
    public class PriceTimePopup_State : IStateExtension
    {
        [DI] private readonly EventBus events;
        private static Type evType = typeof(Event_PriceTimePopup_StateChanged);
        private Event_PriceTimePopup_StateChanged ev;

        #region Private Fields
        private bool visible;
        private uint price;
        private uint time;
        private string title;
        private bool isFine;
        private uint targetId;
        #endregion
        
        #region Getters
        public bool GetVisible() => visible;
        public uint GetPrice() => price;
        public uint GetTime() => time;
        public string GetTitle() => title;
        public bool GetIsFine() => isFine;
        public bool HasTargetId() => targetId > 0;
        public uint GetTargetId() => targetId;
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
        
        public void SetTime(uint value)
        {
            if (time == value) return;
            time = value;
            ev.time = true;
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
        
        public void SetTargetId(uint value)
        {
            if (targetId == value) return;
            targetId = value;
            ev.targetId = true;
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
        public bool SendChanges()
        {
            if (ev.IsEmpty()) return false;
            events.unique.GetOrAdd<Event_PriceTimePopup_StateChanged>() = ev;
            ev = default;
            return true;
        }

#if UNITY_EDITOR
        public string GetStateName() => "Price & Time Popup";
        public void DrawStateProperties(VisualElement root)
        {
            EditorUtils.DrawProperty("Visible", visible);
            EditorUtils.DrawProperty("Title", title);
            EditorUtils.DrawProperty("Price", price);
            EditorUtils.DrawProperty("Time", time);
            EditorUtils.DrawProperty("IsFine", isFine);
            EditorUtils.DrawProperty("TargetId", targetId);
        }
#endif
    }
}