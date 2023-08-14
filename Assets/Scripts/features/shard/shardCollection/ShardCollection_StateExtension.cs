using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Leopotam.EcsProto.QoL;
using td.features._common;
using td.features.eventBus;
using td.features.shard.components;
using td.features.shard.mb;
using td.features.state;
using td.utils.di;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using td.utils;
using UnityEditor;
#endif

namespace td.features.shard.shardCollection
{

    [Serializable]
    public class ShardCollection_StateExtension : IStateExtension
    {
        private const int Max = 6;
        
        [DI] private readonly EventBus events;
        private static Type evType = typeof(Event_ShardCollection_StateChanged);
        private Event_ShardCollection_StateChanged ev;
        
        #region Private Fields
        private readonly List<Shard> items = new(Max);
        private byte maxItems = Max;
        private ShardUIButton hoveredItem = null;
        #endregion
        
        #region Getters
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public byte GetMaxItems() => maxItems;
        [MethodImpl(MethodImplOptions.AggressiveInlining)] [CanBeNull] public ShardUIButton GetHoveredItem() => hoveredItem;
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public IReadOnlyList<Shard> GetItems() => items;
        #endregion
        
        #region Setters
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddItem(ref Shard shard)
        {
            if (items.Count + 1 > GetMaxItems()) return;
            shard._id_ = shard._id_ > 0 ? shard._id_ : CommonUtils.ID("shard-collection");
            items.Add(shard);
            ev.items = true;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UpdateItems() => ev.items = true;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetMaxItems(byte value)
        {
            if (maxItems == value) return;
            maxItems = value;
            //todo покрыть сценарий, когда maxItem < items.Count
            ev.maxItems = true;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetHoveredItem([CanBeNull] ShardUIButton value)
        {
            if (hoveredItem == value) return;
            hoveredItem = value;
            ev.hoveredItem = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveItem(ref Shard shard)
        {
            if (items.Remove(shard))
            {
                ev.items = true;
            }
        }
        
        public bool UpdateItem(ref Shard shard)
        {
            var id = shard._id_;
            for (var index = 0; index < items.Count; index++)
            {
                if (CommonUtils.IdsIsEquals(items[index]._id_, id))
                {
                    items[index] = shard;
                    ev.items = true;
                    return true;
                }
            }
            return false;
        }
        
        public bool RemoveItem(uint id)
        {
            for (var index = 0; index < items.Count; index++)
            {
                if (CommonUtils.IdsIsEquals(items[index]._id_, id))
                {
                    items.RemoveAt(index);
                    ev.items = true;
                    return true;
                }
            }
            return false;
        }
        #endregion

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Type GetEventType() => evType;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Refresh() => ev.All();
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            items.Clear();
            items.Capacity = Max;
            ev.All();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SendChanges()
        {
            if (!ev.IsEmpty())
            {
                events.unique.GetOrAdd<Event_ShardCollection_StateChanged>() = ev;
            }
            ev = default;
        }
        
#if UNITY_EDITOR
        public void DrawStateProperties(VisualElement root)
        {
            EditorUtils.DrawTitle("Shard Collection State", true);
            EditorGUI.indentLevel++;
            EditorUtils.DrawProperty("MaxItems", maxItems);
            if (EditorUtils.FoldoutBegin("shard_collection_items", $"Items ({items.Count})"))
            {
                foreach (var item in items)
                {
                    EditorUtils.DrawProperty(item, ServiceContainer.Get<Shard_Calculator>());
                }
            }
            EditorGUI.indentLevel--;
        }
#endif
    }
}