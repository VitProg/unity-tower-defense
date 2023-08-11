using System;
using System.Collections.Generic;
using NaughtyAttributes;
using td.features._common;
using td.features.gameStatus.bus;
using td.features.level;
using td.features.level.bus;
using td.features.shard.components;
using td.features.shard.mb;
using td.features.state;
using td.features.tower;
using td.utils;
using td.utils.di;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace td.features.shard.shardCollection
{
    public class UI_ShardCollection : MonoInjectable
    {
        public GridLayoutGroup grid;

        [OnValueChanged("Refresh"), MinValue(2), MaxValue(12)]
        public int length = 6;

        [SerializeField] private GameObject buttonPrefab;

        private readonly EcsInject<IState> state;
        private readonly EcsInject<IEventBus> events;
        private readonly EcsInject<SharedData> shared;
        private readonly EcsInject<ShardCalculator> calc;
        private readonly EcsInject<Shard_Service> shardService;
        private readonly EcsInject<Tower_Service> towerService;
        private readonly EcsInject<MB_Shard_Service> mbShardService;
        private readonly EcsInject<LevelMap_Service> levelMapService;

        private ShardConrol dndShard;

        [FormerlySerializedAs("isDrugging")] [NaughtyAttributes.ReadOnly] [SerializeField]
        private bool isDragging = false;

        [NaughtyAttributes.ReadOnly] [SerializeField] private CanDropStatus canDropStatus = CanDropStatus.False;
        private uint dropCost;
        private ShardUIButton targetSource;

        private readonly List<IDisposable> eventDisposers = new(3);
        private EcsPackedEntity? targetTower;
        private EcsPackedEntity? targetShardInTower;
        private Vector2? targetDropPosition;

        private void Start()
        {
            grid ??= GetComponent<GridLayoutGroup>();
            
            eventDisposers.Add(events.Value.Unique.SubscribeTo<Event_StateChanged>(OnStateChanged));
            eventDisposers.Add(events.Value.Unique.SubscribeTo<Command_StopGameSimulation>(delegate { StopDragging(); }));
            eventDisposers.Add(events.Value.Unique.SubscribeTo<Event_LevelFinished>(delegate { StopDragging(); }));
            eventDisposers.Add(events.Value.Unique.SubscribeTo<Event_YouDied>(delegate { StopDragging(); }));

            for (var index = 0; index < transform.childCount; index++)
            {
                Destroy(transform.GetChild(index).gameObject);
            }

            dndShard = shared.Value.draggableShard;
            // }
        }

        private void OnDestroy()
        {
            foreach (var disposer in eventDisposers)
            {
                disposer?.Dispose();
            }

            eventDisposers.Clear();
        }

        private void OnNewLevel(ref Command_LoadLevel item)
        {
            StopDragging();
        }

        private void OnStateChanged(ref Event_StateChanged e)
        {
            if (e.shardCollection.IsEmpty) return;

            if (e.shardCollection.items.HasValue || e.shardCollection.maxItems.HasValue)
            {
                Refresh();
            }
        }

        private void Refresh()
        {
            var tr = transform;

            if (length != state.Value.ShardCollection.MaxItems)
            {
                length = state.Value.ShardCollection.MaxItems;
                if (length < 2) length = 2;
                if (length > 12) length = 12;

                var gridWidth = (grid.cellSize.x + grid.spacing.x * 2) * (length + 1);
                ((RectTransform)(tr)).SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, gridWidth);

                var fisicCount = tr.childCount;

                if (fisicCount < length)
                    for (var index = fisicCount; index < length; index++)
                    {
                        var go = Instantiate(buttonPrefab, transform);
                        var b = go.GetComponent<ShardUIButton>();
                        b.onPointerClicked.AddListener(delegate(Vector2 point) { OnShardPointerClicked(b, point); });
                        b.onPointerEntered.AddListener(delegate(Vector2 point) { OnShardPointerEntered(b, point); });
                        b.onPointerExited.AddListener(delegate(Vector2 point) { OnShardPointerExited(b, point); });
                        b.onDragStart.AddListener(delegate(Vector2 point) { OnShardDragStart(b, point); });
                        b.onDragMove.AddListener(delegate(Vector2 point) { OnShardDragMove(b, point); });
                        b.onDragFinish.AddListener(delegate(Vector2 point) { OnShardDragFinish(b, point); });
                    }

                if (fisicCount > length)
                    for (var index = length; index < fisicCount; index++)
                    {
                        var go = transform.GetChild(length).gameObject;
                        var b = go.GetComponent<ShardUIButton>();
                        b.onPointerEntered.RemoveAllListeners();
                        b.onPointerExited.RemoveAllListeners();
                        b.onDragStart.RemoveAllListeners();
                        b.onDragMove.RemoveAllListeners();
                        b.onDragFinish.RemoveAllListeners();
                        DestroyImmediate(go);
                    }
            }

            var isPlusShowed = false;
            // buttons.Clear();
            for (var index = 0; index < tr.childCount; index++)
            {
                var b = tr.GetChild(index).gameObject.GetComponent<ShardUIButton>();
                b.SetHidden(false);

                if (state.Value != null && index < state.Value.ShardCollection.Items.Count)
                {
                    b.canDrag = true;
                    b.showPlus = false;
                    b.cost = 0;
                    var shard = state.Value.ShardCollection.Items[index];
                    b.SetShard(ref shard);
                }
                else
                {
                    b.canDrag = false;
                    // b.hasShard = false;
                    b.showPlus = !isPlusShowed;
                    isPlusShowed = true;
                    b.ClearShard();
                }

                b.Refresh();
            }
        }

        private void OnShardDragStart(ShardUIButton shardButton, Vector2 point)
        {
            isDragging = true;
            mbShardService.Value.InitializeDndShard(shardButton, point);
            targetSource = shardButton;
            shardButton.SetHidden(true);
        }

        private void OnShardDragMove(ShardUIButton shardButton, Vector2 point)
        {
            var dndTransform = dndShard.transform;
            dndTransform.position = CameraUtils.ToWorldPoint(shared.Value.canvasCamera, point);
            dndTransform.FixAnchoeredPosition();

            // state.Value.CostPopup.Clear();
            canDropStatus = CanDropStatus.False;

            ref var shard = ref shardButton.GetShard();

            if (shard.costInsert == 0) shardService.Value.PrecalcAllCosts(ref shard);
            
            targetTower = null;
            targetShardInTower = null;
            targetDropPosition = null;
 
            var hoveredInCollection = state.Value.ShardCollection.HoveredItem;
            if (hoveredInCollection != null)
            {
                ref var hoveredInCollectionShard = ref hoveredInCollection.GetShard();

                // Note! If we drag shard targets its own cell, we allow it to return there
                if (
                    CommonUtils.IdsIsEquals(hoveredInCollectionShard._id_, shard._id_) ||
                    hoveredInCollection == shardButton ||
                    (targetSource == shardButton && shardButton.IsHovered)
                )
                {
                    canDropStatus = CanDropStatus.Rollback;
                    
                    state.Value.CostPopup.Visible = false;

                    state.Value.InfoPanel.Shard = hoveredInCollectionShard;
                    state.Value.InfoPanel.Visible = true;
                    state.Value.InfoPanel.Title = null;
                    state.Value.InfoPanel.CostTitle = null;
                    state.Value.InfoPanel.Cost = 0;
                    
                    // Debug.Log($"canDrop: SELF {canDropStatus}, {dropCost}");
                    // Debug.Log(" 1:" + CommonUtils.IdsIsEquals(hoveredInCollectionShard._id_, shard._id_));
                    // Debug.Log(" 2:" + (hoveredInCollection == shardButton));
                    // Debug.Log(" 3:" + (targetSource == shardButton && shardButton.IsHovered));

                    return;
                }
                //

                ref var targetShard = ref hoveredInCollection.GetShard();

                var (check, combineCost) = shardService.Value.CheckCanCombineShards(ref shard, ref targetShard);

                // Debug.Log($"CheckCanCombineShards: {check}, {combineCost}");

                canDropStatus = check == CanCombineShardType.True
                    ? CanDropStatus.CombineWithShardInCollection
                    : CanDropStatus.False;

                state.Value.CostPopup.Cost = combineCost;
                state.Value.CostPopup.Visible = true;
                state.Value.CostPopup.Title = "Combine Shards"; // todo i18
                state.Value.CostPopup.IsFine = canDropStatus != CanDropStatus.False;

                // state.Value.InfoPanel.Clear();
                state.Value.InfoPanel.Shard = Shard.CombineTwoShardsToNew(ref targetShard, ref shard);
                state.Value.InfoPanel.Visible = true;
                state.Value.InfoPanel.Title = "Combined shard";
                state.Value.InfoPanel.CostTitle = "Combine cost";
                state.Value.InfoPanel.Cost = combineCost;
                
                dropCost = combineCost;
            }
            else
            {
                var worldPosition = shared.Value.mainCamera.ScreenToWorldPoint(point);
                worldPosition.z = 0f;
                var coords = HexGridUtils.PositionToCell(worldPosition);
                
                var (checkCanDrop, operationCost, towerPackedEntity, shardPackedEntity) = levelMapService.Value.CheckCanDrop(ref coords, ref shard);

                // Debug.Log($"CheckCanDropAndGetCostByScreen: {checkCanDrop}, {operationCost}");

                state.Value.InfoPanel.Shard = shard;
                state.Value.InfoPanel.Visible = true;
                state.Value.InfoPanel.Title = null;
                state.Value.InfoPanel.CostTitle = null;
                state.Value.InfoPanel.Cost = 0;

                state.Value.CostPopup.Cost = operationCost;
                state.Value.CostPopup.Visible = checkCanDrop != CanDropShardOnMapType.False;
                dropCost = operationCost;

                // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
                switch (checkCanDrop)
                {
                    // case CanDropShardOnMapType.Combine:
                    //     canDropStatus = CanDropStatus.CombineInTower;
                    //     state.Value.CostPopup.Title = "Combine Shards"; // todo i18
                    //     break;
                    // case CanDropShardOnMapType.FalseCombineCost:
                    //     state.Value.CostPopup.Title = "Combine Shards"; // todo i18
                    //     break;

                    case CanDropShardOnMapType.CombineInTower:
                        canDropStatus = CanDropStatus.CombineWithShardInTower;
                        state.Value.CostPopup.Title = "Combine Shards In Tower"; // todo i18
                        targetTower = towerPackedEntity;
                        targetShardInTower = shardPackedEntity;
                        break;
                    case CanDropShardOnMapType.FalseCombineInTower:
                        state.Value.CostPopup.Title = "Combine Shards In Tower"; // todo i18
                        break;

                    case CanDropShardOnMapType.InsertInTower:
                        canDropStatus = CanDropStatus.InsertInTower;
                        state.Value.CostPopup.Title = "Integrate Shard To Tower"; //todo i18
                        targetTower = towerPackedEntity;
                        break;
                    case CanDropShardOnMapType.FalseInsertInTower:
                        state.Value.CostPopup.Title = "Integrate Shard To Tower"; //todo i18
                        break;

                    case CanDropShardOnMapType.DropToFloor:
                        canDropStatus = CanDropStatus.DropToFloor;
                        state.Value.CostPopup.Title = "Explode Shard"; //todo i18
                        break;
                    case CanDropShardOnMapType.FalseDropToFloor:
                        state.Value.CostPopup.Title = "Explode Shard"; //todo i18
                        targetDropPosition = worldPosition;
                        break;
                }
            }

            var deny = !shardButton.IsHovered && canDropStatus == CanDropStatus.False;
            state.Value.CostPopup.IsFine = !deny;
            dndShard.shardMB.deny.SetVisible(deny);

            // Debug.Log($"canDrop: {canDropStatus}, {dropCost}");
        }

        private void StopDragging()
        {
            if (!isDragging || !targetSource) return;

            isDragging = false;

            mbShardService.Value.RevertDndShard(targetSource);
            // targetSource.SetHidden(false);
            targetSource = null;
            
            state.Value.CostPopup.Clear();
        }

        private void OnShardDragFinish(ShardUIButton shardButton, Vector2 point)
        {
            // Just to be sure, let's update the status of the drop opportunity
            OnShardDragMove(shardButton, point);

            StopDragging();

            shardButton.SetHidden(false);

            // Debug.Log($"canDrop: {canDropStatus}, {dropCost}");

            ref var shard = ref shardButton.GetShard();

            var success = false;

            // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
            switch (canDropStatus)
            {
                case CanDropStatus.CombineWithShardInCollection:
                {
                    var target = state.Value.ShardCollection.HoveredItem;
                    if (target != null)
                    {
                        ref var targetShard = ref target.GetShard();
                        targetShard.CombineWith(ref shard);
                        shardService.Value.PrecalcAllCosts(ref targetShard);
                        state.Value.ShardCollection.UpdateItem(ref targetShard);
                        //todo: run combine fx
                        success = true;
                    }
                    
                    break;
                }
                case CanDropStatus.CombineWithShardInTower:
                {
                    // todo
                    if (targetTower.HasValue && targetShardInTower.HasValue)
                    {
                        ref var targetShard = ref shardService.Value.GetShard(targetShardInTower.Value, out var shardEntity);
                        //todo update view
                        targetShard.CombineWith(ref shard);
                        shardService.Value.PrecalcAllCosts(ref targetShard);
                        state.Value.ShardCollection.RemoveItem(shard._id_);
                        var shardMB = shardService.Value.GetShardMB(shardEntity);
                        shardMB.shardData = targetShard;
                        shardMB.Refresh();
                        //todo: run combine fx
                        success = true;
                    }
                    break;
                }
                case CanDropStatus.InsertInTower:
                {
                    // todo
                    if (targetTower.HasValue)
                    {
                        shardService.Value.PrecalcAllCosts(ref shard);
                        shardService.Value.InsertShardInTower(ref shard, targetTower.Value);
                        state.Value.ShardCollection.RemoveItem(shard._id_);
                        //todo: run combine fx
                        success = true;
                    }
                    break;
                }
                case CanDropStatus.DropToFloor:
                {
                    // todo
                    if (targetDropPosition.HasValue)
                    {
                        shardService.Value.DropToMap(ref shard, targetDropPosition.Value);
                        //todo remove from colection and update it
                        state.Value.ShardCollection.RemoveItem(shard._id_);
                    }
                    break;
                }
                default:
                    targetSource.SetHidden(false);
                    break;
            }

            if (success)
            {
                state.Value.ShardCollection.RemoveItem(shard._id_);
                state.Value.Energy -= dropCost;
            }

            targetDropPosition = null;
            targetTower = null;
            targetShardInTower = null;

            canDropStatus = CanDropStatus.False;
            dropCost = 0;
        }

        private void OnShardPointerClicked(ShardUIButton shardButton, Vector2 point)
        {
            if (shardButton.showPlus)
            {
                state.Value.ShardStore.Visible = !state.Value.ShardStore.Visible;
                state.Value.ShardStore.X = shardButton.transform.position.x;
            }
            else
            {
                state.Value.ShardStore.Visible = false;
            }
        }

        private void OnShardPointerEntered(ShardUIButton shardButton, Vector2 point)
        {
            if (!shardButton.hasShard || shardButton.hidden || shardButton.cost > 0) return;
            state.Value.ShardCollection.HoveredItem = shardButton;
            // state.Value.InfoPanel.Clear();
            state.Value.InfoPanel.Shard = shardButton.GetShard();
            state.Value.InfoPanel.Visible = true;
            state.Value.InfoPanel.Cost = 0;
            state.Value.InfoPanel.CostTitle = null;
            state.Value.InfoPanel.Before = null;
            state.Value.InfoPanel.After = null;
        }

        private void OnShardPointerExited(ShardUIButton shardButton, Vector2 point)
        {
            if (state.Value.ShardCollection.HoveredItem == shardButton)
            {
                state.Value.ShardCollection.HoveredItem = null;
                if (state.Value.InfoPanel.Shard.HasValue && CommonUtils.IdsIsEquals(state.Value.InfoPanel.Shard.Value._id_, shardButton.GetShard()._id_))
                {       
                    state.Value.InfoPanel.Clear();
                    state.Value.InfoPanel.Visible = false;
                }
            }
        }
    }

    public enum CanDropStatus
    {
        False = 0,
        Rollback = 1,
        CombineWithShardInCollection = 2,
        CombineWithShardInTower = 3,
        InsertInTower = 4,
        DropToFloor = 5,
    }
}