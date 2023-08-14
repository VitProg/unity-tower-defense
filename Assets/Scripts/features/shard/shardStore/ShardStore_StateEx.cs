using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Leopotam.EcsProto.QoL;
using td.features.eventBus;
using td.features.state;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using td.utils;
using UnityEditor;
#endif

namespace td.features.shard.shardStore
{

    [Serializable]
    public class ShardStore_StateEx : IStateExtension
    {
        private const int Max = 6;
        
        [DI] private readonly EventBus events;
        private static Type evType = typeof(Event_ShardStore_StateChanged);
        private Event_ShardStore_StateChanged ev;

        #region Private Fields
        private readonly List<ShardStore_Item> items = new(6);
        private bool visible;
        private float x;
        #endregion
        
        #region Getters
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public bool GetVisible() => visible;
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public float GetX() => x;
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public IReadOnlyList<ShardStore_Item> GetItems() => items;

        #endregion
        
        #region Setters
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetVisible(bool value)
        {
            if (visible == value) return;
            visible = value;
            ev.visible = true;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetX(float value)
        {
            if (FloatUtils.IsEquals(x, value)) return;
            x = value;
            ev.x = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ClearItems()
        {
            items.Clear();
            items.Capacity = Max;
            ev.items = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddItem(ref ShardStore_Item item)
        {
            if (items.Contains(item)) return;
            items.Add(item);
            ev.items = true;
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
            x = 0;
            ClearItems();
            ev.All();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UpdateItems() => ev.items = true;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SendChanges()
        {
            if (!ev.IsEmpty())
            {
                events.unique.GetOrAdd<Event_ShardStore_StateChanged>() = ev;
            }
            ev = default;
        }

#if UNITY_EDITOR

        public void DrawStateProperties(VisualElement root)
        {
            EditorUtils.DrawTitle("Shard Store State", true);
            EditorGUI.indentLevel++;
            EditorUtils.DrawProperty("Visible", visible);
            EditorUtils.DrawProperty("Position X", x);
            if (EditorUtils.FoldoutBegin("shard_store_items", $"Items ({items.Count})"))
            {
                foreach (var item in items)
                {
                    EditorUtils.DrawProperty("cost", item.cost);
                    EditorUtils.DrawProperty("type", item.shardType.ToString());
                }
            }
            EditorGUI.indentLevel--;
        }
#endif
    }
}