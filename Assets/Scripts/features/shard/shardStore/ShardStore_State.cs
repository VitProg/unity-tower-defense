#if UNITY_EDITOR
using UnityEditor;
using UnityEngine.UIElements;
#endif
using System;
using System.Runtime.CompilerServices;
using Leopotam.EcsProto.QoL;
using td.features.eventBus;
using td.features.shard.components;
using td.features.state.interfaces;
using td.utils;
using td.utils.di;

namespace td.features.shard.shardStore
{

    [Serializable]
    public class ShardStore_State : IStateExtension
    {
        private const int Max = 6;
        
        [DI] private readonly EventBus events;
        private static Type evType = typeof(Event_ShardStore_StateChanged);
        private Event_ShardStore_StateChanged ev;

        #region Private Fields
        private readonly ShardStore_Item[] items = new ShardStore_Item[Max];
        private int count = 0;
        private int hoveredIndex = -1;
        private int lastHoveredIndex = -1;
        private bool visible;
        private float x;
        private byte level = 1;
        #endregion
        
        #region Getters
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public bool GetVisible() => visible;
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public byte GetLevel() => level;
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public int GetCount() => count;
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public float GetX() => x;
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public ref ShardStore_Item GetItem(int idx) => ref items[idx];
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public bool HasItem(int index) => index >= 0 && index < count;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasItem(ShardTypes shardType)
        {
            for (var idx = 0; idx < count; idx++) {
                if (items[idx].shardType == shardType) return true;
            }
            return false;
        }       
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetItemIndex(ShardTypes shardType)
        {
            for (var idx = 0; idx < count; idx++) {
                if (items[idx].shardType == shardType) return idx;
            }
            return -1;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public bool HasHovered() => HasItem(hoveredIndex);
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public int GetHoveredIndex() => hoveredIndex;
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public int GetLastHoveredIndex() => lastHoveredIndex;
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
        public void ToggleVisible() => SetVisible(!visible);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void IncreaseLevel(int relative) => SetLevel(Math.Clamp(level + relative, 1, 10));
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void IncreaseLevel() => SetLevel(Math.Clamp(level + 1, 1, 10));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ReduceLevel(int relative) => SetLevel(Math.Clamp(level - relative, 1, 10));
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ReduceLevel() => SetLevel(Math.Clamp(level - 1, 1, 10));
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetLevel(int value) => SetLevel((byte)value);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetLevel(byte value)
        {
            if (FloatUtils.IsEquals(level, value) || level == 0 || level > 10) return;
            level = value;
            ev.level = true;
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
            count = 0;
            ev.items = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool AddItem(ref ShardStore_Item item)
        {
            if (count + 1 >= Max) return false;
            for (var idx = 0; idx < count; idx++) {
                if (items[idx].shardType == item.shardType) return false;
            }
            items[count] = item;
            count++;
            ev.items = true;
            return true;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ItemUpdated(ShardTypes shardType)
        {
            if (count + 1 >= Max) return false;
            for (var idx = 0; idx < count; idx++) {
                if (items[idx].shardType == shardType)
                {
                    ev.items = true;
                    return true;
                }
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool SetHoveredIndex(int index = -1)
        {
            if (hoveredIndex == index || index < -1 || index >= count) return false;
            lastHoveredIndex = hoveredIndex;
            hoveredIndex = index;
            ev.hoveredIndex = true;
            return true;
        }       
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool SetHovered(ShardTypes shardType)
        {
            var idx = GetItemIndex(shardType);
            if (idx < 0 || idx >= count) return false;
            return SetHoveredIndex(idx);
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
        public bool SendChanges()
        {
            if (ev.IsEmpty()) return false;
            events.unique.GetOrAdd<Event_ShardStore_StateChanged>() = ev;
            ev = default;
            return true;
        }

#if UNITY_EDITOR
        public string GetStateName() => "Shard Store";
        public void DrawStateProperties(VisualElement root)
        {
            EditorUtils.DrawProperty("Visible", visible);
            EditorUtils.DrawProperty("Level", level);
            EditorUtils.DrawProperty("Hovered Index", $"{hoveredIndex} / ${lastHoveredIndex}");
            EditorUtils.DrawProperty("Position X", x);
            if (EditorUtils.FoldoutBegin("shard_store_items", $"Items ({count})"))
            {
                for (var idx = 0; idx < count; idx++)
                {
                    var item = items[idx];
                    EditorUtils.DrawProperty("type", item.shardType.ToString());
                    Shard_EditorUtils.DrawShard(ref item.shard, ServiceContainer.Get<Shard_Calculator>(), $"shard_store_items[${idx}]");
                    EditorGUILayout.Space(5);
                }

                EditorUtils.FoldoutEnd();
            }
        }
#endif
    }
}