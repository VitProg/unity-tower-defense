using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using td.features.state;
using td.utils;

namespace td.features.shard.shardStore
{

    [Serializable]
    public class ShardStore_State
    {
        private const int Max = 6;

        private readonly List<ShardStore_Item> items = new(6);
        private bool visible;
        private float x;

        private State state;

        public ShardStore_State(State state)
        {
            this.state = state;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ref ShardStore_StateEvent GetEvent() => ref state.GetEvent().shardStore;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            visible = false;
            x = 0;
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
        public void AddItem(ref ShardStore_Item item)
        {
            if (items.Contains(item)) return;
            items.Add(item);
            GetEvent().items = true;
        }

        public List<ShardStore_Item> Items => items;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UpdateItems() => GetEvent().items = true;

        public bool Visible
        {
            get => visible;
            set
            {
                if (visible == value) return;
                visible = value;
                GetEvent().visible = true;
            }
        }

        public float X
        {
            get => x;
            set
            {
                if (FloatUtils.IsEquals(x, value)) return;
                x = value;
                GetEvent().x = true;
            }
        }
    }
}