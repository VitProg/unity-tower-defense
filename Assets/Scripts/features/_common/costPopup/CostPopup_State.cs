using System;
using System.Runtime.CompilerServices;
using td.features.state;

namespace td.features._common.costPopup
{

    [Serializable]
    public class CostPopup_State
    {
        private bool visible;
        private uint cost;
        private string title;
        private bool isFine;

        private readonly State state;

        public CostPopup_State(State state)
        {
            this.state = state;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ref CostPopup_StateEvent GetEvent() => ref state.GetEvent().costPopup;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            visible = false;
            cost = 0;
            title = "";
            isFine = false;
            GetEvent().All();
        }

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

        public bool IsFine
        {
            get => isFine;
            set
            {
                if (isFine == value) return;
                isFine = value;
                GetEvent().isFine = true;
            }
        }
    }
}