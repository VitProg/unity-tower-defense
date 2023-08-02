using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using td.features._common;
using td.features.shard.components;
using td.features.shard.mb;
using td.features.state;

namespace td.features.shardCollection
{

    [Serializable]
    public class ShardCollection_State
    {
        private const int Max = 6;

        private readonly List<Shard> items = new(Max);
        private byte maxItems = Max;
        private ShardUIButton hoveredItem = null;

        private readonly State state;

        public ShardCollection_State(State state)
        {
            this.state = state;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ref ShardCollection_StateEvent GetEvent() => ref state.GetEvent().shardCollection;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            ClearItems();
            GetEvent().All();
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ClearItems()
        {
            items.Clear();
            items.Capacity = Max;
            GetEvent().items = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddItem(ref Shard shard)
        {
            if (items.Count + 1 > MaxItems) return;
            // if (items.Contains(item)) return;
            shard._id_ = shard._id_ > 0 ? shard._id_ : CommonUtils.ID("shard-collection");
            items.Add(shard);
            GetEvent().items = true;
        }

        public List<Shard> Items => items;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UpdateItems() => GetEvent().items = true;

        public byte MaxItems
        {
            get => maxItems;
            set
            {
                if (maxItems == value) return;
                maxItems = value;
                //todo покрыть сценарий, когда maxItem < items.Count
                GetEvent().maxItems = true;
            }
        }

        [CanBeNull]
        public ShardUIButton HoveredItem
        {
            get => hoveredItem;
            set
            {
                if (hoveredItem == value) return;
                hoveredItem = value;
                GetEvent().hoveredItem = true;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveItem(ref Shard shard)
        {
            if (items.Remove(shard))
            {
                GetEvent().items = true;
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool RemoveItem(uint id)
        {
            for (var index = 0; index < items.Count; index++)
            {
                if (CommonUtils.IdsIsEquals(items[index]._id_, id))
                {
                    items.RemoveAt(index);
                    GetEvent().items = true;
                    return true;
                }
            }
            return false;
        }

        public bool UpdateItem(ref Shard shard)
        {
            var id = shard._id_;
            for (var index = 0; index < items.Count; index++)
            {
                if (CommonUtils.IdsIsEquals(items[index]._id_, id))
                {
                    items[index] = shard;
                    GetEvent().items = true;
                    return true;
                }
            }
            return false;
        }
    }
}