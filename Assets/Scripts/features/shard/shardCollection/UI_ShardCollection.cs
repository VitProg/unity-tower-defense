using System.Collections.Generic;
using System.Runtime.CompilerServices;
using NaughtyAttributes;
using td.features._common;
using td.features.camera;
using td.features.eventBus;
using td.features.inputEvents.bus;
using td.features.level;
using td.features.level.bus;
using td.features.level.cells;
using td.features.shard.bus;
using td.features.shard.mb;
using td.features.shard.shardStore;
using td.features.state;
using td.features.state.bus;
using td.utils;
using td.utils.di;
using UnityEngine;

namespace td.features.shard.shardCollection
{
    public class UI_ShardCollection : MonoBehaviour
    {
        [OnValueChanged("Refresh"), MinValue(2), MaxValue(12)]
        public int length = 6;

        [SerializeField] private GameObject buttonPrefab;

        #region DI
        private State _state;
        private State State => _state ??= ServiceContainer.Get<State>();

        private ShardCollection_State _collState;
        private ShardCollection_State CollState => _collState ??= State.Ex<ShardCollection_State>();

        private ShardStore_State _stateStore;
        private ShardStore_State StateStore => _stateStore ??= State.Ex<ShardStore_State>();

        private Level_State _levelState;
        private Level_State LevelStat => _levelState ??= State.Ex<Level_State>();

        private EventBus _events;
        private EventBus Events => _events ??= ServiceContainer.Get<EventBus>();

        private Camera_Service _cameraService;
        private Camera_Service CameraService => _cameraService ??= ServiceContainer.Get<Camera_Service>();

        private Shard_Service _shardService;
        private Shard_Service ShardService => _shardService ??= ServiceContainer.Get<Shard_Service>();

        private Shard_MB_Service _mbShardService;
        private Shard_MB_Service MBShardService => _mbShardService ??= ServiceContainer.Get<Shard_MB_Service>();

        private Shard_Calculator _calc;
        private Shard_Calculator Calc => _calc ??= ServiceContainer.Get<Shard_Calculator>();

        #endregion

        private UI_Shard dndShard;
        private RectTransform dndShardRectTransform;

        private RectTransform rectTransform;

        private void Start()
        {
            rectTransform = (RectTransform)transform;
            
            Events.unique.ListenTo<Event_StateChanged>(OnStateChanged);
            Events.unique.ListenTo<Event_ShardCollection_StateChanged>(OnCollectionStateChanged);
            Events.unique.ListenTo<Event_LevelFinished>(OnLevelFinished);
            Events.unique.ListenTo<Command_LoadLevel>(OnLevelLoad);
            Events.global.ListenTo<Event_PointerUp>(OnOuterClick);

            for (var index = 0; index < transform.childCount; index++)
            {
                Destroy(transform.GetChild(index).gameObject);
            }

            length = 0;

            dndShard = MBShardService.GetDraggableShard();
            dndShardRectTransform = (RectTransform)dndShard.transform;
            // }
        }

        private void OnDestroy()
        {
            Events.unique.RemoveListener<Event_StateChanged>(OnStateChanged);
            Events.unique.RemoveListener<Event_ShardCollection_StateChanged>(OnCollectionStateChanged);
            Events.unique.RemoveListener<Event_LevelFinished>(OnLevelFinished);
            Events.unique.RemoveListener<Command_LoadLevel>(OnLevelLoad);
            Events.global.RemoveListener<Event_PointerUp>(OnOuterClick);
        }

        // ----------------------------------------------------------------

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnStateChanged(ref Event_StateChanged ev)
        {
            if (ev.lives && State.IsDead()) StopDragging();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnLevelFinished(ref Event_LevelFinished obj) => StopDragging();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnLevelLoad(ref Command_LoadLevel item) => StopDragging();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnCollectionStateChanged(ref Event_ShardCollection_StateChanged e)
        {
            if (e.IsEmpty()) return;

            if (e.items || e.maxShards)
            {
                Refresh();
            }
        }

        private readonly List<UI_Shard_Button> shards = new();

        private void Refresh()
        {
            if (!Application.isPlaying || length != CollState.GetCount())
            {
                if (Application.isPlaying)
                {
                    length = CollState.GetMaxShards();
                }

                // var gridWidth = (grid.cellSize.x + grid.spacing.x * 2) * (length + 1);
                // rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, gridWidth);

                var fisicCount = transform.childCount;

                if (fisicCount < length)
                    for (var index = fisicCount; index < length; index++)
                    {
                        var go = Instantiate(buttonPrefab, transform);

                        if (Application.isPlaying)
                        {
                            var b = go.GetComponent<UI_Shard_Button>();
                            var idx = index;
                            b.OnPointerClicked.AddListener(delegate { OnShardPointerClicked(idx); });
                            b.OnPointerEntered.AddListener(delegate { OnShardPointerEntered(idx); });
                            b.OnPointerExited.AddListener(delegate { OnShardPointerExited(idx); });
                            b.OnDragStart.AddListener(delegate(Vector2 point) { OnShardDragStart(idx, point); });
                            b.OnDragMove.AddListener(delegate(Vector2 point) { OnShardDragMove(idx, point); });
                            b.OnDragFinish.AddListener(delegate(Vector2 point) { OnShardDragFinish(idx); });
                        }
                    }

                if (fisicCount > length)
                    for (var index = length; index < fisicCount; index++)
                    {
                        var go = transform.GetChild(length).gameObject;

                        if (Application.isPlaying)
                        {
                            var b = go.GetComponent<UI_Shard_Button>();
                            b.OnPointerClicked.RemoveAllListeners();
                            b.OnPointerEntered.RemoveAllListeners();
                            b.OnPointerExited.RemoveAllListeners();
                            b.OnDragStart.RemoveAllListeners();
                            b.OnDragMove.RemoveAllListeners();
                            b.OnDragFinish.RemoveAllListeners();
                        }

                        DestroyImmediate(go);
                    }
            }

            shards.Clear();
            var isPlusShowed = false;
            var count = CollState.GetCount();
            for (var index = 0; index < transform.childCount; index++)
            {
                var b = transform.GetChild(index).gameObject.GetComponent<UI_Shard_Button>();
                b.SetHidden(false);

                if (Application.isPlaying && State != null && index < count)
                {
                    b.canDrag = true;
                    b.showPlus = false;
                    b.price = 0;
                    ref var shard = ref CollState.GetItem(index);
                    b.SetShard(ref shard);
                    b.uiShard.collectionIndex = index;
                }
                else
                {
                    b.canDrag = false;
                    b.hasShard = false;
                    b.showPlus = !isPlusShowed;
                    isPlusShowed = true;
                    b.ClearShard();
                    b.uiShard.collectionIndex = -1;
                }

                b.Refresh();
                shards.Add(b);
            }
        }

        private void OnShardDragStart(int index, Vector2 point)
        {
            if (index < 0 || index >= shards.Count || !shards[index].hasShard) return;

            CollState.SetDraggableShardIndex(index);

            var shardButton = shards[index];
            dndShard.SetupFromAnother(in shardButton.uiShard, true);

            dndShard.gameObject.SetActive(true);
            dndShard.transform.position = CameraService.ScreenToCanves(point);
            dndShard.transform.FixAnchoredPosition();

            shardButton.SetHidden(true);

            CameraService.MutePanAndZoom();
        }

        private void OnShardDragMove(int index, Vector2 screenPoint)
        {
            if (index < 0 || index >= shards.Count || !shards[index].hasShard) return;
            var shardButton = shards[index];
            ref var draggableShard = ref shardButton.GetShard();

            if (draggableShard.priceInsert == 0) ShardService.PrecalcAllData(ref draggableShard);

            dndShardRectTransform.SetPositionAndFixAnchoredPosition(CameraService.ScreenToCanves(screenPoint));

            CollState.SetDraggableShardIndex(index);

            var operationChanged = false;

            var hoveredIndex = CollState.GetHoveredIndex();
            var hoveredInCollection = hoveredIndex >= 0 && hoveredIndex != CollState.GetDraggableShardIndex()
                ? shards[hoveredIndex]
                : null;

            var worldPosition = CameraService.ScreenToMain(screenPoint);
            var coords = HexGridUtils.PositionToCell(worldPosition);

            var deny = true;
            var hasCell = LevelStat.HasCell(coords.x, coords.y);
            var cellHasShard = false;

            if (hasCell)
            {
                cellHasShard = LevelStat.GetCell(coords.x, coords.y).HasShard();
            }

            Debug.Log("----------------------------------------------------------------");
            Debug.Log("hasCell = " + hasCell);
            Debug.Log("cellHasShard = " + cellHasShard);
            Debug.Log("hoveredInCollection = " + hoveredInCollection);

            if (hoveredInCollection != null || cellHasShard)
            {
                var inTower = cellHasShard && hoveredInCollection == null;

                Debug.Log("inTower = " + inTower);

                var targetShard = hoveredInCollection
                    ? hoveredInCollection.GetShard()
                    : ShardService.GetShard(LevelStat.GetCell(coords.x, coords.y).packedShardEntity, out _);

                Debug.Log("targetShard = " + targetShard);

                // Note! If we drag shard targets its own cell, we allow it to return there
                if (
                    CommonUtils.IdsIsEquals(targetShard._id_, draggableShard._id_) ||
                    hoveredInCollection == shardButton ||
                    (CollState.GetDraggableShardIndex() == index && shardButton.IsHovered)
                )
                {
                    Debug.Log("--- return");
                    return;
                }

                var combineCost = Calc.CalculateCombinePrice(ref draggableShard, ref targetShard);
                var combineTime = Calc.CalculateCombineTime(ref draggableShard, ref targetShard);

                Debug.Log("> CombineOperation");

                CollState.CombineOperation(
                    ref targetShard,
                    ref draggableShard,
                    combineCost,
                    combineTime,
                    inTower ? LevelStat.GetCell(coords.x, coords.y).packedBuildingEntity : null
                );

                operationChanged = true;
            }
            else if (hasCell)
            {
                var cell = LevelStat.GetCell(coords.x, coords.y);
                if (cell.type == CellTypes.CanBuild && cell.HasBuilding() && !cell.HasShard())
                {
                    if (cell.buildingId == Constants.Buildings.ShardTower)
                    {
                        CollState.InsertOperation(
                            draggableShard.priceInsert,
                            draggableShard.timeInsert,
                            cell.packedBuildingEntity
                        );
                        operationChanged = true;
                    }
                    else if (cell.buildingId == Constants.Buildings.ShardTrap)
                    {
                        CollState.InsertOperation(
                            (uint)(draggableShard.priceInsert * 1.25f),
                            (uint)(draggableShard.timeInsert * 1.25f /* todo move to constants */),
                            cell.packedBuildingEntity
                        );
                        operationChanged = true;
                    }
                }
                else
                {
                    CollState.DropOperation(
                        draggableShard.priceDrop,
                        worldPosition.x,
                        worldPosition.y
                    );
                    operationChanged = true;
                }
            }

            if (!operationChanged) {
                CollState.UnsetOperationTargetEntity();
                CollState.UnsetOperationTargetPosition();
                CollState.UnsetOperation();
            }

            deny = !(CollState.IsAnyOperation() && State.IsEnoughEnergy(CollState.GetOperationPrice()));

            dndShard.SetDeny(deny);
        }

        private void StopDragging()
        {
            CollState.UnsetOperation();

            if (!CollState.IsDragging()) return;

            var shardButton = shards[CollState.GetDraggableShardIndex()];
            
            shardButton.uiShard.SetupFromAnother(dndShard);
            dndShard.shard._id_ = 0;
            dndShard.gameObject.SetActive(false);

            CameraService.ResumePanAndZoom();

            CollState.StopDragging();
        }

        private void OnShardDragFinish(int index)
        {
            if (index < 0 || index >= shards.Count || !shards[index].hasShard ||
                index != CollState.GetDraggableShardIndex())
            {
                // todo: some error ((
                StopDragging();
                return;
            }

            var shardButton = shards[index];

            // ref var shard = ref shardButton.GetShard();

            if (CollState.IsCombineOperation())
            {
                if (CollState.HasOperationTargetEntity())
                {
                    // in building
                    ref var cmd = ref Events.global.Add<Command_CombineShards_InBuilding>();
                    cmd.sourceIndex = index;
                    cmd.targetBuilding = CollState.GetOperationTargetEntity();
                    cmd.cost = CollState.GetOperationPrice();
                    cmd.time = CollState.GetOperationTime();
                    Debug.Log($">>> Send Command_CombineShards_InBuilding sourceIndex = {index}");
                }
                else
                {
                    // in collection
                    ref var cmd = ref Events.global.Add<Command_CombineShards_InCollection>();
                    cmd.sourceIndex = index;
                    cmd.targetIndex = CollState.GetHoveredIndex();
                    cmd.cost = CollState.GetOperationPrice();
                    cmd.time = CollState.GetOperationTime();
                    Debug.Log($">>> Send Command_CombineShards_InCollection sourceIndex = {index}");
                }
            }
            else if (CollState.IsInsertOperation())
            {
                ref var cmd = ref Events.global.Add<Command_InsertShard_InBuilding>();
                cmd.sourceIndex = index;
                cmd.targetBuilding = CollState.GetOperationTargetEntity();
                cmd.cost = CollState.GetOperationPrice();
                cmd.time = CollState.GetOperationTime();
                Debug.Log($">>> Send Command_InsertShard_InBuilding sourceIndex = {index}");
            }
            else if (CollState.IsDropOperation())
            {
                ref var cmd = ref Events.global.Add<Command_DropShard_OnMap>();
                cmd.sourceIndex = index;
                cmd.position = CollState.GetOperationTargetPosition();
                cmd.cost = CollState.GetOperationPrice();
                Debug.Log($">>> Send Command_DropShard_OnMap sourceIndex = {index}");
            }
            else
            {
                //Rollback
            }

            // StateEx.UnsetOperation();
            // StateEx.StopDragging();
            StopDragging();

            shardButton.SetHidden(false);
        }
        
        private void OnOuterClick(ref Event_PointerUp _)
        {
            StateStore.SetVisible(false);
        }

        private void OnShardPointerClicked(int index)
        {
            if (index < 0 || index >= shards.Count) return;
            var shardButton = shards[index];

            if (shardButton.showPlus)
            {
                StateStore.ToggleVisible();
                StateStore.SetX(shardButton.transform.position.x);
            }
            else
            {
                StateStore.SetVisible(false);
                if (shardButton.hasShard)
                {
                    CollState.SetHoveredIndex(
                        CollState.GetHoveredIndex() == index ? -1 : index
                    );
                }
            }
        }

        private void OnShardPointerEntered(int index)
        {
            Debug.Log("OnShardPointerEntered(" + index + ')');
            if (index < 0 || index >= shards.Count || !shards[index].hasShard) return;
            CollState.SetHoveredIndex(index);
        }

        private void OnShardPointerExited(int index)
        {
            if (index < 0 || index >= shards.Count || !shards[index].hasShard) return;
            CollState.SetHoveredIndex(-1);
        }
    }
}