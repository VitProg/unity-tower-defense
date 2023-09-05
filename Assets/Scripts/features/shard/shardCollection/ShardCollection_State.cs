#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.Runtime.CompilerServices;
using Leopotam.EcsProto.QoL;
using td.features._common;
using td.features.eventBus;
using td.features.shard.components;
using td.features.state.interfaces;
using td.utils;
using td.utils.di;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

namespace td.features.shard.shardCollection {
    [Serializable]
    public class ShardCollection_State : IStateExtension {
        [DI] private readonly EventBus events;
        [DI] private readonly Shard_Service shardService;
        private static Type evType = typeof(Event_ShardCollection_StateChanged);
        private Event_ShardCollection_StateChanged ev;

#region Private Fields

        private readonly Shard[] items = new Shard[Constants.UI.Shard.MaxInCollection];
        private byte count = 0;
        private byte maxShards = Constants.UI.Shard.MinInCollection;
        private short hoveredIndex = -1;
        private short lastHoveredIndex = -1;
        private int draggableShardIndex = -1;
        private Shard combinedShard = default;
        private uint operationPrice = 0;
        private uint operationTime = 0;
        private OperationType operationType = OperationType.None;
        private ProtoPackedEntityWithWorld operationTargetEntity = default;
        private float2 operationTargetPosition = default;

#endregion

#region Getters

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public byte GetCount() => count;
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public byte GetMaxShards() => maxShards;
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public short GetHoveredIndex() => hoveredIndex;
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public short GetLastHoveredIndex() => lastHoveredIndex;
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public bool HasHovered() => hoveredIndex >= 0 && hoveredIndex < count;
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public bool HasItem(int index) => index >= 0 && index < count;
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public ref Shard GetItem(int index) => ref items[index];
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public bool IsDragging() => HasItem(draggableShardIndex);
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public int GetDraggableShardIndex() => draggableShardIndex;
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public ref Shard GetDraggableShard() => ref items[draggableShardIndex];
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public ref Shard GetCombinedShard() => ref combinedShard;
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public uint GetOperationPrice() => operationPrice;
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public uint GetOperationTime() => operationTime;
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public bool IsCombineOperation() => operationType == OperationType.Combine;
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public bool IsInsertOperation() => operationType == OperationType.Insert;
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public bool IsDropOperation() => operationType == OperationType.Drop;
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public bool IsAnyOperation() => operationType != OperationType.None;
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public OperationType GetOperationType() => operationType;
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public bool HasOperationTargetEntity() => operationTargetEntity.Unpack(out _, out _);
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public ProtoPackedEntityWithWorld GetOperationTargetEntity() => operationTargetEntity;

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public bool HasOperationTargetPosition() =>
            !FloatUtils.IsZero(operationTargetPosition.x) && !FloatUtils.IsZero(operationTargetPosition.y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public float2 GetOperationTargetPosition() => operationTargetPosition;

#endregion

#region Setters

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetMaxShards(int value) {
            var v = Math.Clamp(value, Constants.UI.Shard.MinInCollection, Constants.UI.Shard.MaxInCollection);
            if (v == maxShards) return;
            maxShards = (byte)v;
            ev.maxShards = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int AddItem(ref Shard shard) {
            if (count + 1 > maxShards) return -1;
            shard._id_ = shard._id_ > 0 ? shard._id_ : CommonUtils.ID("shard-collection");
            items[count] = shard;
            count++;
            ev.items = true;
            return count - 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UpdateItems() => ev.items = true;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetHoveredIndex(int index) {
            if (hoveredIndex == index) return;
            lastHoveredIndex = hoveredIndex;
            hoveredIndex = (short)index;
            ev.hoveredIndex = true;
        }

        public bool UpdateItem(ref Shard shard) {
            var id = shard._id_;
            for (var index = 0; index < count; index++) {
                if (CommonUtils.IdsIsEquals(items[index]._id_, id)) {
                    items[index] = shard;
                    ev.items = true;
                    return true;
                }
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool RemoveItem(ref Shard shard) => RemoveItem(shard._id_);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool RemoveItem(uint id) {
            for (var index = 0; index < count; index++) {
                if (CommonUtils.IdsIsEquals(items[index]._id_, id)) {
                    return RemoveItemAt(index);
                }
            }

            return false;
        }

        public bool RemoveItemAt(int removedIdx) {
            Debug.Log($">>> STATE RemoveItemAt ${removedIdx}");

            if (!HasItem(removedIdx)) return false;

            var removed = false;
            for (var idx = 0; idx < items.Length; idx++) {
                if (idx > removedIdx && idx - 1 >= 0) {
                    items[idx - 1] = items[idx];
                    removed = true;
                }
            }

            items[^1] = default;

            if (removed) {
                count--;
                ev.items = true;
                return true;
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetDraggableShardIndex(int index) {
            if (draggableShardIndex == index) return;
            draggableShardIndex = index;
            ev.draggableShard = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UnsetDraggableShardIndex() {
            draggableShardIndex = -1;
            ev.draggableShard = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UnsetOperation() {
            combinedShard = default;
            operationPrice = 0;
            operationTime = 0;
            operationType = OperationType.None;
            operationTargetEntity = default;
            operationTargetPosition = default;
            ev.operation = true;
            ev.operationTarget = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool CombineOperation(ref Shard shardA, ref Shard shardB, uint price, uint time, ProtoPackedEntityWithWorld? buildingEntity) {
            if (operationType == OperationType.Insert &&
                operationPrice == price &&
                operationTime == time &&
                (buildingEntity.HasValue && operationTargetEntity.EqualsTo(buildingEntity.Value))
                // todo: && Shard.CombineTwoShardsToNew(ref shardA, ref shardB).Equals(combinedShard)
            ) return false;
            
            Debug.Log($">>> ShardCollection_State.CombineOperation: price: {price}, time: {time}, buildingEntity: {buildingEntity}");

            UnsetOperation();
            operationPrice = price;
            operationTime = time;
            operationType = OperationType.Combine;
            combinedShard = Shard.CombineTwoShardsToNew(ref shardA, ref shardB);
            shardService.PrecalcAllData(ref combinedShard);
            operationTargetEntity = buildingEntity ?? default;

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool InsertOperation(uint price, uint time, ProtoPackedEntityWithWorld buildingEntity) {
            if (operationType == OperationType.Insert &&
                operationPrice == price &&
                operationTime == time &&
                operationTargetEntity.EqualsTo(buildingEntity)) return false;
            Debug.Log("operationType == OperationType.Insert = " + (operationType == OperationType.Insert));
            Debug.Log("operationPrice == price = " + (operationPrice == price));
            Debug.Log("operationTime == time = " + (operationTime == time));
            Debug.Log("operationTargetEntity.EqualsTo(buildingEntity) = " + operationTargetEntity.EqualsTo(buildingEntity));
            Debug.Log($">>> ShardCollection_State.InsertOperation: price: {price}, time: {time}, buildingEntity: {buildingEntity}");

            UnsetOperation();
            operationType = OperationType.Insert;
            operationPrice = price;
            operationTime = time;
            operationTargetEntity = buildingEntity;

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool DropOperation(uint price, float x, float y) {
            if (operationType == OperationType.Drop &&
                operationPrice == price &&
                FloatUtils.IsEquals(x, operationTargetPosition.x, 0.1f) &&
                FloatUtils.IsEquals(y, operationTargetPosition.y, 0.1f)) return false;
            
            Debug.Log($">>> ShardCollection_State.DropOperation: price: {price}, x: {x}, y: {y}");

            UnsetOperation();
            operationType = OperationType.Drop;
            operationPrice = price;
            operationTime = 0;
            operationTargetPosition.x = x;
            operationTargetPosition.y = y;

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetOperationTargetEntity(ProtoPackedEntityWithWorld entity) {
            if (entity.EqualsTo(operationTargetEntity)) return;
            operationTargetEntity = entity;
            ev.operationTarget = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UnsetOperationTargetEntity() {
            operationTargetEntity = default;
            ev.operationTarget = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetOperationTargetPosition(float x, float y) {
            if (FloatUtils.IsEquals(x, operationTargetPosition.x) && FloatUtils.IsEquals(y, operationTargetPosition.y)) return;
            operationTargetPosition.x = x;
            operationTargetPosition.y = y;
            ev.operationTarget = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UnsetOperationTargetPosition() {
            operationTargetPosition = default;
            ev.operationTarget = true;
        }

#endregion

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Type GetEventType() => evType;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Refresh() => ev.All();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear() {
            count = 0;
            maxShards = Constants.UI.Shard.MinInCollection;
            hoveredIndex = -1;
            lastHoveredIndex = -1;
            draggableShardIndex = -1;
            combinedShard = default;
            operationPrice = 0;
            operationTime = 0;
            operationType = OperationType.None;
            operationTargetEntity = default;
            operationTargetPosition = default;
            ev.All();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool SendChanges() {
            if (ev.IsEmpty()) return false;
            events.unique.GetOrAdd<Event_ShardCollection_StateChanged>() = ev;
            ev = default;
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void StopDragging() {
            if (!IsDragging()) return;
            UnsetDraggableShardIndex();
            UnsetOperation();
            UnsetOperationTargetPosition();
            UnsetOperationTargetEntity();
        }

#if UNITY_EDITOR
        public string GetStateName() => "Shard Collection State";
        public void DrawStateProperties(VisualElement root) {
            EditorUtils.DrawProperty("Max Shards", maxShards);
            if (EditorUtils.FoldoutBegin("shard_collection_items", $"Items ({count})")) {
                var calc = ServiceContainer.Get<Shard_Calculator>();
                for (var idx = 0; idx < count; idx++) {
                    Shard_EditorUtils.DrawShard($"[{idx}] Shard", ref items[idx], calc);
                }

                EditorUtils.FoldoutEnd();
            }

            EditorGUILayout.Space();

            EditorUtils.DrawTitle("[Operation]");

            EditorUtils.DrawProperty("Hovered Index", $"{hoveredIndex}  / {lastHoveredIndex}");
            EditorUtils.DrawProperty("Is Dragging", IsDragging());
            if (IsDragging()) EditorUtils.DrawProperty("Draggable Index", draggableShardIndex);
            EditorUtils.DrawProperty("Type", operationType.ToString());
            if (operationType != OperationType.None) {
                EditorUtils.DrawProperty("Price", operationPrice);
                if (operationType != OperationType.Drop) EditorUtils.DrawProperty("Time", operationTime);
                if (operationType != OperationType.Drop) EditorUtils.DrawEntity("Target", operationTargetEntity);
                EditorUtils.DrawProperty("Position", operationTargetPosition);
                if (operationType == OperationType.Combine)
                    Shard_EditorUtils.DrawShard("Combined Shard", ref combinedShard, ServiceContainer.Get<Shard_Calculator>());
            }
        }
#endif

        public enum OperationType {
            None,
            Insert,
            Combine,
            Drop
        }
    }
}
