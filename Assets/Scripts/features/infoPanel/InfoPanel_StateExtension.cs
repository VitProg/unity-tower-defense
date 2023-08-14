using System;
using System.Runtime.CompilerServices;
using Leopotam.EcsProto.QoL;
using td.features.enemy.components;
using td.features.eventBus;
using td.features.shard;
using td.features.shard.components;
using td.features.state;
using td.utils.di;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using td.utils;
using UnityEditor;
#endif

namespace td.features.infoPanel
{
    [Serializable]
    public class InfoPanel_StateExtension : IStateExtension
    {
        [DI] private readonly EventBus events;
        private static Type evType = typeof(Event_InfoPanel_StateChanged);
        private Event_InfoPanel_StateChanged ev;

        #region Private Fields
        private bool visible;
        private string title;
        private string costTitle;
        private uint cost;
        private string before;
        private string after;
        private bool hasShard;
        private Shard shard;
        private bool hasEnemy;
        private Enemy enemy;
        #endregion
        
        #region Getters
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public bool GetVisible() => visible;
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public bool HasShard() => hasShard && shard._id_ > 0;
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public ref Shard GetShard() => ref shard;
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public bool HasEnemy() => hasEnemy && enemy._id_ > 0;
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public ref Enemy GetEnemy() => ref enemy;
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public string GetTitle() => title;
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public string GetBefore() => before;
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public string GetAfter() => after;
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public uint GetCost() => cost;
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public string GetCostTitle() => costTitle;
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
        public void UnsetShard()
        {
            if (!HasShard()) return;
            hasShard = false;
            shard = default;
            ev.shard = true;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetShard(ref Shard value)
        {
            if (shard._id_ == value._id_ || shard.IsEquals(ref value))
            {
                if (!hasShard)
                {
                    hasShard = true;
                    ev.shard = true;
                    return;
                }
            }

            hasShard = true;
            // shard.SetFrom(ref value);
            shard = value;
            ev.shard = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UnsetEnemy()
        {
            if (!HasEnemy()) return;
            hasEnemy = false;
            enemy = default;
            ev.enemy = true;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetEnemy(ref Enemy value)
        {
            if (enemy._id_ == value._id_ || enemy.IsEquals(ref value))
            {
                if (!hasEnemy)
                {
                    hasEnemy = true;
                    ev.enemy = true;
                    return;
                }
            }
            
            hasEnemy = true;
            enemy = value; //?
            ev.enemy = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetTitle(string value)
        {
            if (title == value) return;
            title = value;
            ev.title = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetBefore(string value)
        {
            if (before == value) return;
            before = value;
            ev.before = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetAfter(string value)
        {
            if (after == value) return;
            after = value;
            ev.after = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetCostTitle(string value)
        {
            if (costTitle == value) return;
            costTitle = value;
            ev.costTitle = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetCost(uint value)
        {
            if (cost == value) return;
            cost = value;
            ev.cost = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetCost(uint costValue, string costTitleValue)
        {
            SetCost(costValue);
            SetTitle(costTitleValue);
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
            title = null;
            costTitle = null;
            cost = 0;
            before = null;
            after = null;
            hasShard = false;
            shard = default;
            hasEnemy = false;
            enemy = default;
            ev.All();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SendChanges()
        {
            if (!ev.IsEmpty())
            {
                events.unique.GetOrAdd<Event_InfoPanel_StateChanged>() = ev;
            }
            ev = default;
        }
        

#if UNITY_EDITOR
        public void DrawStateProperties(VisualElement root)
        {
            EditorUtils.DrawTitle("Info Panel State", true);
            EditorGUI.indentLevel++;
            EditorUtils.DrawProperty("Visible", visible);
            EditorUtils.DrawProperty("Cost", cost);
            EditorUtils.DrawProperty("Cost Title", costTitle);
            EditorUtils.DrawProperty("Before Text", before);
            EditorUtils.DrawProperty("After Text", after);
            if (HasShard())
            {
                EditorUtils.DrawTitle("Shard:");
                EditorGUI.indentLevel++;
                EditorUtils.DrawProperty(shard, ServiceContainer.Get<Shard_Calculator>());
                EditorGUI.indentLevel--;
            }
            if (HasEnemy())
            {
                EditorUtils.DrawTitle("Enemy:");
                EditorGUI.indentLevel++;
                EditorUtils.DrawProperty(enemy);
                EditorGUI.indentLevel--;
            }
            EditorGUI.indentLevel--;
        }
#endif
    }
}