using Leopotam.EcsProto.QoL;
using NaughtyAttributes;
using td.features._common;
using td.features.camera;
using td.features.costPopup;
using td.features.eventBus;
using td.features.gameStatus.bus;
using td.features.infoPanel;
using td.features.level;
using td.features.level.bus;
using td.features.shard.components;
using td.features.shard.mb;
using td.features.shard.shardStore;
using td.features.state;
using td.features.tower;
using td.utils;
using td.utils.di;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace td.features.shard.shardCollection
{
    public class UI_ShardCollection : MonoBehaviour
    {
        public GridLayoutGroup grid;

        [OnValueChanged("Refresh"), MinValue(2), MaxValue(12)]
        public int length = 6;

        [SerializeField] private GameObject buttonPrefab;

        private State State =>  ServiceContainer.Get<State>();
        private EventBus Events =>  ServiceContainer.Get<EventBus>();
        private Camera_Service CameraService =>  ServiceContainer.Get<Camera_Service>();
        private Shard_Calculator Calc =>  ServiceContainer.Get<Shard_Calculator>();
        private Shard_Service ShardService =>  ServiceContainer.Get<Shard_Service>();
        private Tower_Service TowerService =>  ServiceContainer.Get<Tower_Service>();
        private Shard_MB_Service MBShardService =>  ServiceContainer.Get<Shard_MB_Service>();
        private Level_Map_Service LevelMapService =>  ServiceContainer.Get<Level_Map_Service>();

        private ShardConrol dndShard;

        [FormerlySerializedAs("isDrugging")] [ReadOnly] [SerializeField]
        private bool isDragging = false;

        [ReadOnly] [SerializeField] private CanDropStatus canDropStatus = CanDropStatus.False;
        private uint dropCost;
        private ShardUIButton targetSource;
        
        private ProtoPackedEntityWithWorld? targetTower;
        private ProtoPackedEntityWithWorld? targetShardInTower;
        private Vector2? targetDropPosition;

        private void Start()
        {
            grid ??= GetComponent<GridLayoutGroup>();
            
            Events.unique.ListenTo<Event_ShardCollection_StateChanged>(OnStateChanged);
            Events.unique.ListenTo<Command_StopGameSimulation>(OnStopGameSimulation);
            Events.unique.ListenTo<Event_LevelFinished>(OnLevelFinished);
            Events.unique.ListenTo<Event_YouDied>(OnYouDied);

            for (var index = 0; index < transform.childCount; index++)
            {
                Destroy(transform.GetChild(index).gameObject);
            }

            dndShard = MBShardService.GetDraggableShard();
            // }
        }

        private void OnYouDied(ref Event_YouDied obj) => StopDragging();
        private void OnLevelFinished(ref Event_LevelFinished obj) => StopDragging();
        private void OnStopGameSimulation(ref Command_StopGameSimulation obj) => StopDragging();

        private void OnDestroy()
        {
            Events.unique.RemoveListener<Event_ShardCollection_StateChanged>(OnStateChanged);
            Events.unique.RemoveListener<Command_StopGameSimulation>(OnStopGameSimulation);
            Events.unique.RemoveListener<Event_LevelFinished>(OnLevelFinished);
            Events.unique.RemoveListener<Event_YouDied>(OnYouDied);
        }

        private void OnNewLevel(ref Command_LoadLevel item)
        {
            StopDragging();
        }

        private void OnStateChanged(ref Event_ShardCollection_StateChanged e)
        {
            if (e.IsEmpty()) return;

            if (e.items || e.maxItems)
            {
                Refresh();
            }
        }

        private void Refresh()
        {
            var tr = transform;

            var coll = State.Ex<ShardCollection_StateExtension>();
            
            if (length != coll.GetMaxItems())
            {
                length = coll.GetMaxItems();
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

                if (State != null && index < coll.GetItems().Count)
                {
                    b.canDrag = true;
                    b.showPlus = false;
                    b.cost = 0;
                    var shard = coll.GetItems()[index];
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
            MBShardService.InitializeDndShard(shardButton, point);
            targetSource = shardButton;
            shardButton.SetHidden(true);
        }

        private void OnShardDragMove(ShardUIButton shardButton, Vector2 point)
        {
            var coll = State.Ex<ShardCollection_StateExtension>();
            var costPopup = State.Ex<CostPopup_StateExtension>();
            var infoPanel = State.Ex<InfoPanel_StateExtension>();
            
            var dndTransform = dndShard.transform;
            dndTransform.position = CameraUtils.ToWorldPoint(CameraService.GetCanvasCamera(), point);
            dndTransform.FixAnchoeredPosition();

            // sc.Clear();
            canDropStatus = CanDropStatus.False;

            ref var shard = ref shardButton.GetShard();

            if (shard.costInsert == 0) ShardService.PrecalcAllCosts(ref shard);
            
            targetTower = null;
            targetShardInTower = null;
            targetDropPosition = null;
 
            var hoveredInCollection = coll.GetHoveredItem();
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
                    
                    infoPanel.SetShard(ref hoveredInCollectionShard);
                    infoPanel.SetVisible(true);
                    infoPanel.SetTitle(null);
                    infoPanel.SetCostTitle(null);
                    infoPanel.SetCost(0);
                    
                    // Debug.Log($"canDrop: SELF {canDropStatus}, {dropCost}");
                    // Debug.Log(" 1:" + CommonUtils.IdsIsEquals(hoveredInCollectionShard._id_, shard._id_));
                    // Debug.Log(" 2:" + (hoveredInCollection == shardButton));
                    // Debug.Log(" 3:" + (targetSource == shardButton && shardButton.IsHovered));

                    return;
                }
                //

                ref var targetShard = ref hoveredInCollection.GetShard();

                var (check, combineCost) = ShardService.CheckCanCombineShards(ref shard, ref targetShard);

                // Debug.Log($"CheckCanCombineShards: {check}, {combineCost}");

                canDropStatus = check == CanCombineShardType.True
                    ? CanDropStatus.CombineWithShardInCollection
                    : CanDropStatus.False;

                costPopup.SetCost(combineCost);
                costPopup.SetVisible(true);
                costPopup.SetTitle("Combine Shards"); // todo i18
                costPopup.SetIsFine(canDropStatus != CanDropStatus.False);

                var combinedShard = Shard.CombineTwoShardsToNew(ref targetShard, ref shard);
                infoPanel.SetShard(ref combinedShard);
                infoPanel.SetVisible(true);
                infoPanel.SetTitle("Combined shard");
                infoPanel.SetCostTitle("Combine cost");
                infoPanel.SetCost(combineCost);
                
                dropCost = combineCost;
            }
            else
            {
                var worldPosition = CameraService.GetMainCamera().ScreenToWorldPoint(point);
                worldPosition.z = 0f;
                var coords = HexGridUtils.PositionToCell(worldPosition);
                
                var (checkCanDrop, operationCost, towerPackedEntity, shardPackedEntity) = LevelMapService.CheckCanDrop(ref coords, ref shard);

                infoPanel.SetShard(ref shard);
                infoPanel.SetVisible(true);
                infoPanel.SetTitle(null);
                infoPanel.SetCostTitle(null);
                infoPanel.SetCost(0);

                costPopup.SetCost(operationCost);
                costPopup.SetVisible(checkCanDrop != CanDropShardOnMapType.False);
                dropCost = operationCost;

                // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
                switch (checkCanDrop)
                {
                    // case CanDropShardOnMapType.Combine:
                    //     canDropStatus = CanDropStatus.CombineInTower;
                    //     sc.Title = "Combine Shards"; // todo i18
                    //     break;
                    // case CanDropShardOnMapType.FalseCombineCost:
                    //     sc.Title = "Combine Shards"; // todo i18
                    //     break;

                    case CanDropShardOnMapType.CombineInTower:
                        canDropStatus = CanDropStatus.CombineWithShardInTower;
                        costPopup.SetTitle("Combine Shards In Tower"); // todo i18
                        targetTower = towerPackedEntity;
                        targetShardInTower = shardPackedEntity;
                        break;
                    case CanDropShardOnMapType.FalseCombineInTower:
                        costPopup.SetTitle("Combine Shards In Tower"); // todo i18
                        break;

                    case CanDropShardOnMapType.InsertInTower:
                        canDropStatus = CanDropStatus.InsertInTower;
                        costPopup.SetTitle("Integrate Shard To Tower"); //todo i18
                        targetTower = towerPackedEntity;
                        break;
                    case CanDropShardOnMapType.FalseInsertInTower:
                        costPopup.SetTitle("Integrate Shard To Tower"); //todo i18
                        break;

                    case CanDropShardOnMapType.DropToFloor:
                        canDropStatus = CanDropStatus.DropToFloor;
                        costPopup.SetTitle("Explode Shard"); //todo i18
                        break;
                    case CanDropShardOnMapType.FalseDropToFloor:
                        costPopup.SetTitle("Explode Shard"); //todo i18
                        targetDropPosition = worldPosition;
                        break;
                }
            }

            var deny = !shardButton.IsHovered && canDropStatus == CanDropStatus.False;
            costPopup.SetIsFine(!deny);
            dndShard.shardMB.deny.SetVisible(deny);

            // Debug.Log($"canDrop: {canDropStatus}, {dropCost}");
        }

        private void StopDragging()
        {
            if (!isDragging || !targetSource) return;

            isDragging = false;

            MBShardService.RevertDndShard(targetSource);
            // targetSource.SetHidden(false);
            targetSource = null;
            
            var costPopup = State.Ex<CostPopup_StateExtension>();
            costPopup.Clear();
        }

        private void OnShardDragFinish(ShardUIButton shardButton, Vector2 point)
        {
            var coll = State.Ex<ShardCollection_StateExtension>();
            
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
                    var target = coll.GetHoveredItem();
                    if (target != null)
                    {
                        ref var targetShard = ref target.GetShard();
                        targetShard.CombineWith(ref shard);
                        ShardService.PrecalcAllCosts(ref targetShard);
                        coll.UpdateItem(ref targetShard);
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
                        ref var targetShard = ref ShardService.GetShard(targetShardInTower.Value, out var shardEntity);
                        //todo update view
                        targetShard.CombineWith(ref shard);
                        ShardService.PrecalcAllCosts(ref targetShard);
                        coll.RemoveItem(shard._id_);
                        var shardMB = ShardService.GetShardMB(shardEntity);
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
                        ShardService.PrecalcAllCosts(ref shard);
                        ShardService.InsertShardInTower(ref shard, targetTower.Value);
                        coll.RemoveItem(shard._id_);
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
                        ShardService.DropToMap(ref shard, targetDropPosition.Value);
                        //todo remove from colection and update it
                        coll.RemoveItem(shard._id_);
                    }
                    break;
                }
                default:
                    targetSource.SetHidden(false);
                    break;
            }

            if (success)
            {
                coll.RemoveItem(shard._id_);
                State.SetEnergy(State.GetEnergy() - dropCost);
            }

            targetDropPosition = null;
            targetTower = null;
            targetShardInTower = null;

            canDropStatus = CanDropStatus.False;
            dropCost = 0;
        }

        private void OnShardPointerClicked(ShardUIButton shardButton, Vector2 point)
        {
            var store = State.Ex<ShardStore_StateEx>();
            
            if (shardButton.showPlus)
            {
                store.SetVisible(!store.GetVisible());
                store.SetX(shardButton.transform.position.x);
            }
            else
            {
                store.SetVisible(false);
            }
        }

        private void OnShardPointerEntered(ShardUIButton shardButton, Vector2 point)
        {
            var coll = State.Ex<ShardCollection_StateExtension>();
            var infoPanel = State.Ex<InfoPanel_StateExtension>();
            
            if (!shardButton.hasShard || shardButton.hidden || shardButton.cost > 0) return;
            coll.SetHoveredItem(shardButton);
            
            infoPanel.SetShard(ref shardButton.GetShard());
            infoPanel.SetVisible(true);
            infoPanel.SetCost(0);
            infoPanel.SetCostTitle(null);
            infoPanel.SetBefore(null);
            infoPanel.SetAfter(null);
        }

        private void OnShardPointerExited(ShardUIButton shardButton, Vector2 point)
        {
            var coll = State.Ex<ShardCollection_StateExtension>();
            var infoPanel = State.Ex<InfoPanel_StateExtension>();
            
            if (coll.GetHoveredItem() == shardButton)
            {
                coll.SetHoveredItem(null);
                if (infoPanel.HasShard() && CommonUtils.IdsIsEquals(infoPanel.GetShard()._id_, shardButton.GetShard()._id_))
                {       
                    infoPanel.Clear();
                    infoPanel.SetVisible(false);
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