using System;
using System.Runtime.CompilerServices;
using td.features._common;
using td.features.state;
using _Shard = td.features.shard.components.Shard;
using _Enemy = td.features.enemy.components.Enemy;

namespace td.features.infoPanel
{
    [Serializable]
    public class InfoPanel_StateEx
    {
        private bool visible;
        private string title;
        private string costTitle;
        private uint cost;
        private string before;
        private string after;
        private _Shard? shard;
        private _Enemy? enemy;

        private readonly State state;

        public InfoPanel_StateEx(State state)
        {
            this.state = state;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ref InfoPanel_StateEvent GetEvent() => ref state.GetEvent().infoPanel;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            visible = false;
            title = null;
            costTitle = null;
            cost = 0;
            before = null;
            after = null;
            shard = null;
            enemy = null;
            GetEvent().All();
            // Debug.Log("InfoPanel cleared");
        }

        public bool Visible
        {
            get => visible;
            set
            {
                if (visible == value) return;
                visible = value;
                GetEvent().visible = true;
                // Debug.Log("InfoPanel visible: " + visible);
            }
        }

        public _Shard? Shard
        {
            get => shard;
            set
            {
                if (!shard.HasValue && !value.HasValue) return;
                if (value != null && shard != null && _Shard.IsEquals(shard.Value, value.Value)) return;
                shard = value;
                GetEvent().shard = true;
                // Debug.Log("InfoPanel set shard: " + value);
                if (shard.HasValue) Visible = true;
            }
        }

        // todo
        public _Enemy? Enemy
        {
            get => enemy;
            set
            {
                if (!enemy.HasValue && !value.HasValue) return;
                if (value != null && enemy != null && CommonUtils.IdsIsEquals(enemy.Value._id_, value.Value._id_)) return;
                enemy = value;
                GetEvent().enemy = true;
                if (enemy.HasValue) Visible = true;
            }
        }

        public string Title
        {
            get => title;
            set
            {
                if (title == value) return;
                title = value;
                GetEvent().title = true;
            }
        }

        public string Before
        {
            get => before;
            set
            {
                if (before == value) return;
                before = value;
                GetEvent().before = true;
            }
        }

        public string After
        {
            get => after;
            set
            {
                if (after == value) return;
                after = value;
                GetEvent().after = true;
            }
        }

        public uint Cost
        {
            get => cost;
            set
            {
                if (cost == value) return;
                cost = value;
                GetEvent().cost = true;
            }
        }

        public string CostTitle
        {
            get => costTitle;
            set
            {
                if (costTitle == value) return;
                costTitle = value;
                GetEvent().costTitle = true;
            }
        }

        public void SetCost(uint costValue, string costTitleValue)
        {
            Cost = costValue;
            Title = costTitleValue;
        }
    }
}