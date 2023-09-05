#if UNITY_EDITOR
using td.features.enemy;
using UnityEditor;
#endif
using System;
using System.Runtime.CompilerServices;
using Leopotam.EcsProto.QoL;
using td.features.enemy.components;
using td.features.eventBus;
using td.features.shard;
using td.features.shard.components;
using td.features.state.interfaces;
using td.utils.di;
using UnityEngine.UIElements;
using td.utils;
using UnityEngine;


namespace td.features.infoPanel {
    [Serializable]
    public class InfoPanel_State : IStateExtension {
        [DI] private readonly EventBus events;
        private static Type evType = typeof(Event_InfoPanel_StateChanged);
        private Event_InfoPanel_StateChanged ev;

#region Private Fields

        private bool visible;
        private string title;
        private string priceTitle;
        private uint price;
        private string timeTitle;
        private uint time;
        private string before;
        private string after;
        private Shard shard;
        private bool hasEnemy;
        private Enemy enemy;

#endregion

#region Getters

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public bool GetVisible() => visible;
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public bool HasShard() => shard.Quantity > 0;
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public ref Shard GetShard() => ref shard;
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public bool HasEnemy() => hasEnemy && enemy._id_ > 0;
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public ref Enemy GetEnemy() => ref enemy;
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public string GetTitle() => title;
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public string GetBefore() => before;
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public string GetAfter() => after;
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public uint GetPrice() => price;
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public string GetPriceTitle() => priceTitle;
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public uint GetTime() => time;
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public string GetTimeTitle() => timeTitle;

#endregion

#region Setters

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetVisible(bool value) {
            if (visible == value) return;
            visible = value;
            ev.visible = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UnsetShard() {
            if (!HasShard()) return;
            shard = default;
            ev.shard = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetShard(ref Shard value) {
            if (shard.IsEquals(ref value, false)) {
                return;
            }

            shard = value;
            ev.shard = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UnsetEnemy() {
            if (!HasEnemy()) return;
            hasEnemy = false;
            enemy = default;
            ev.enemy = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetEnemy(ref Enemy value) {
            if (enemy._id_ == value._id_ || enemy.IsEquals(ref value)) {
                if (!hasEnemy) {
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
        public void SetTitle(string value) {
            if (title == value) return;
            title = value;
            ev.title = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetBefore(string value) {
            if (before == value) return;
            before = value;
            ev.before = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetAfter(string value) {
            if (after == value) return;
            after = value;
            ev.after = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetPriceTitle(string value) {
            if (priceTitle == value) return;
            priceTitle = value;
            ev.priceTitle = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetPrice(uint value) {
            if (price == value) return;
            price = value;
            ev.price = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetPrice(uint costValue, string costTitleValue) {
            SetPrice(costValue);
            SetPriceTitle(costTitleValue);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetTimeTitle(string value) {
            if (timeTitle == value) return;
            timeTitle = value;
            ev.timeTitle = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetTime(uint value) {
            if (time == value) return;
            time = value;
            ev.price = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetTime(uint timeValue, string timeTitleValue) {
            SetTime(timeValue);
            SetTimeTitle(timeTitleValue);
        }

#endregion

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Type GetEventType() => evType;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Refresh() => ev.All();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear() {
            visible = false;
            title = null;
            priceTitle = null;
            price = 0;
            before = null;
            after = null;
            shard = default;
            hasEnemy = false;
            enemy = default;
            ev.All();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool SendChanges() {
            if (ev.IsEmpty()) return false;
            events.unique.GetOrAdd<Event_InfoPanel_StateChanged>() = ev;
            ev = default;
            return true;
        }

#if UNITY_EDITOR
        public string GetStateName() => "Info Panel";
        public void DrawStateProperties(VisualElement root) {
            EditorUtils.DrawProperty("Visible", visible);
            EditorUtils.DrawProperty("Cost", price);
            EditorUtils.DrawProperty("Cost Title", priceTitle);
            EditorUtils.DrawProperty("Time", time);
            EditorUtils.DrawProperty("Time Title", timeTitle);
            EditorUtils.DrawProperty("Before Text", before);
            EditorUtils.DrawProperty("After Text", after);
            if (HasShard()) {
                EditorUtils.DrawTitle("Shard:");
                EditorGUI.indentLevel++;
                Shard_EditorUtils.DrawShard(ref shard, ServiceContainer.Get<Shard_Calculator>());
                EditorGUI.indentLevel--;
            }

            if (HasEnemy()) {
                EditorUtils.DrawTitle("Enemy:");
                EditorGUI.indentLevel++;
                Enemy_EditorUtils.DrawEnemy(enemy);
                EditorGUI.indentLevel--;
            }
        }
#endif
    }
}
