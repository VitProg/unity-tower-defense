using System;
using System.Collections.Generic;
using NaughtyAttributes;
using td.features._common;
using td.features.gameStatus.bus;
using td.features.level.bus;
using td.features.shard.mb;
using td.features.state;
using td.utils.di;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace td.features.shard.shardStore
{
    public class UI_ShardStore : MonoInjectable
    {
        public Button closeButton;
        public GridLayoutGroup grid;
        public Button levelUp;
        public Button levelDown;
        public TMP_Text levelText;

        [OnValueChanged("RefreshLevel")] [MinValue(1), MaxValue(10)]
        public byte level = 1;

        [SerializeField] private GameObject buttonPrefab;

        private readonly EcsInject<IEventBus> events;
        private readonly EcsInject<IState> state;
        private readonly EcsInject<ShardCalculator> calc;
        private readonly EcsInject<Shard_Service> shardService;
        
        private readonly List<IDisposable> eventDisposers = new(3);

        private void Start()
        {
            grid ??= GetComponent<GridLayoutGroup>();
            closeButton.onClick.AddListener(OnClose);
            levelDown.onClick.AddListener(delegate { ChangeLevel(-1); });
            levelUp.onClick.AddListener(delegate { ChangeLevel(1); });
            
            eventDisposers.Add(events.Value.Unique.SubscribeTo<Event_StateChanged>(OnStateChanged));
            eventDisposers.Add(events.Value.Unique.SubscribeTo<Event_LevelFinished>(delegate { Hide(); }));
            eventDisposers.Add(events.Value.Unique.SubscribeTo<Event_YouDied>(delegate { Hide(); }));
            
            for (var index = 0; index < grid.transform.childCount; index++)
            {
                var go = grid.transform.GetChild(index).gameObject;
                var b = go.GetComponent<ShardUIButton>();
                b.onPointerClicked.RemoveAllListeners();
                b.onPointerEntered.RemoveAllListeners();
                b.onPointerExited.RemoveAllListeners();
                Destroy(go);
            }
        }

        private void OnStateChanged(ref Event_StateChanged e)
        {
            if (e.shardStore.IsEmpty) return;

            if (e.shardStore.items.HasValue)
            {
                Refresh();
            }

            if (e.shardStore.x.HasValue)
            {
                var pos = transform.position;
                pos.x = state.Value.ShardStore.X;
                transform.position = pos;
            }

            if (e.shardStore.visible.HasValue && gameObject.activeSelf != state.Value.ShardStore.Visible)
            {
                gameObject.SetActive(state.Value.ShardStore.Visible);
                if (state.Value.ShardStore.Visible)
                {
                    for (var index = 0; index < grid.transform.childCount; index++)
                    {
                        grid.transform.GetChild(index).GetComponent<ShardUIButton>().Refresh();
                    }
                }
            }
        }

        private void Hide()
        {
            state.Value.ShardStore.Visible = false;
            gameObject.SetActive(false);
        }

        private void Refresh()
        {
            var length = state.Value.ShardStore.Items.Count;

            var tr = transform;
            var gridWidth = (grid.cellSize.x + grid.spacing.x * 2) * (length + 1);
            ((RectTransform)(tr)).SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, gridWidth);

            var fisicCount = grid.transform.childCount;
            var index = 0;
            foreach (var item in state.Value.ShardStore.Items)
            {
                var isNew = index >= fisicCount;

                var go = isNew 
                    ? Instantiate(buttonPrefab, grid.transform)
                    : grid.transform.GetChild(index).gameObject;

                var ui = go.GetComponent<ShardUIButton>();

                if (isNew)
                {
                    ui.onPointerClicked.AddListener(delegate { OnShardPointerClicked(ui); });
                    ui.onPointerEntered.AddListener(delegate { OnShardPointerEntered(ui); });
                    ui.onPointerExited.AddListener(delegate { OnShardPointerExited(ui); });
                }

                ref var shard = ref ui.GetShard();
                ShardUtils.Clear(ref shard);
                ShardUtils.Set(ref shard, item.shardType, (byte)calc.Value.GetQuantityForLevel(level));
                shardService.Value.PrecalcAllCosts(ref shard);

                ui.showPlus = false;
                ui.cost = shard.cost;
                ui.hasShard = tr;
                ui.SetShard(ref shard);
                ui.Refresh();

                index++;
            }

            for (; index < fisicCount; index++)
            {
                var go = grid.transform.GetChild(index).gameObject;
                var b = go.GetComponent<ShardUIButton>();
                b.onPointerClicked.RemoveAllListeners();
                b.onPointerEntered.RemoveAllListeners();
                b.onPointerExited.RemoveAllListeners();
                Destroy(go);
            }
        }

        private void OnShardPointerEntered(ShardUIButton shardUIButton)
        {
            ref var shard = ref shardUIButton.GetShard();
            state.Value.ShardCollection.HoveredItem = shardUIButton;

            state.Value.InfoPanel.Clear();
            state.Value.InfoPanel.Shard = shard;
            state.Value.InfoPanel.Title = "Buying shard";
            state.Value.InfoPanel.Cost = shard.cost;
        }

        private void OnShardPointerExited(ShardUIButton shardButton)
        {
            if (state.Value.ShardCollection.HoveredItem == shardButton)
            {
                state.Value.ShardCollection.HoveredItem = null;
                if (state.Value.InfoPanel.Shard.HasValue && CommonUtils.IdsIsEquals(state.Value.InfoPanel.Shard.Value._id_, shardButton.GetShard()._id_))
                {
                    state.Value.InfoPanel.Clear();
                }
            }
        }

        private void OnShardPointerClicked(ShardUIButton shardButton)
        {
            if (
                !shardButton.hasShard ||
                shardButton.cost <= 0 ||
                state.Value.Energy < shardButton.cost ||
                state.Value.ShardCollection.Items.Count + 1 > state.Value.ShardCollection.MaxItems
            ) return;

            var newShard = shardButton.GetShard().MakeCopy();

            state.Value.Energy -= shardButton.cost;

            // state.Value.ShardStore.Remove(state.Value.ShardStore[index]);
            state.Value.ShardCollection.AddItem(ref newShard);

            // state.Value.RefreshShardStore();
            state.Value.ShardCollection.UpdateItems();
            ;
            state.Value.ShardStore.Visible = false;
        }

        private void RefreshLevel()
        {
            levelText.text = level.ToString();
        }

        private void ChangeLevel(int l)
        {
            var newLevel = Mathf.Clamp(level + l, 1, 10);
            if (level == newLevel) return;

            level = (byte)newLevel;
            RefreshLevel();
            Refresh();
        }

        private void OnClose()
        {
            state.Value.ShardStore.Visible = false;
        }

        private void OnDestroy()
        {
            foreach (var disposer in eventDisposers)
            {
                disposer?.Dispose();
            }
            eventDisposers.Clear();
            
            for (var index = 0; index < grid.transform.childCount; index++)
            {
                var go = grid.transform.GetChild(index).gameObject;
                var b = go.GetComponent<ShardUIButton>();
                b.onPointerClicked.RemoveAllListeners();
                b.onPointerEntered.RemoveAllListeners();
                b.onPointerExited.RemoveAllListeners();
                Destroy(go);
            }

            closeButton.onClick.RemoveAllListeners();
            levelUp.onClick.RemoveAllListeners();
            levelDown.onClick.RemoveAllListeners();
            state.Value.ShardStore.ClearItems();
        }
    }
}