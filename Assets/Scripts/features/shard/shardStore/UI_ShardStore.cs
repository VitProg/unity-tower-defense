using Leopotam.Types;
using NaughtyAttributes;
using td.features._common;
using td.features.eventBus;
using td.features.gameStatus.bus;
using td.features.infoPanel;
using td.features.level.bus;
using td.features.shard.mb;
using td.features.shard.shardCollection;
using td.features.state;
using td.utils;
using td.utils.di;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace td.features.shard.shardStore
{
    public class UI_ShardStore : MonoBehaviour
    {
        public Button closeButton;
        public GridLayoutGroup grid;
        public Button levelUp;
        public Button levelDown;
        public TMP_Text levelText;

        [OnValueChanged("RefreshLevel")] [MinValue(1), MaxValue(10)]
        public byte level = 1;

        [SerializeField] private GameObject buttonPrefab;

        private EventBus Events =>  ServiceContainer.Get<EventBus>();
        private State State =>  ServiceContainer.Get<State>();
        private Shard_Calculator Calc =>  ServiceContainer.Get<Shard_Calculator>();
        private Shard_Service ShardService =>  ServiceContainer.Get<Shard_Service>();
        
        private void Start()
        {
            grid ??= GetComponent<GridLayoutGroup>();
            closeButton.onClick.AddListener(OnClose);
            levelDown.onClick.AddListener(delegate { ChangeLevel(-1); });
            levelUp.onClick.AddListener(delegate { ChangeLevel(1); });
            
            Events.unique.ListenTo<Event_ShardStore_StateChanged>(OnStateChanged);
            Events.unique.ListenTo<Event_LevelFinished>(OnLevelFinished);
            Events.unique.ListenTo<Event_YouDied>(OnYouDied);
            
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

        private void OnYouDied(ref Event_YouDied obj)
        {
            Hide();
        }

        private void OnLevelFinished(ref Event_LevelFinished obj)
        {
            Hide();
        }

        private void OnStateChanged(ref Event_ShardStore_StateChanged e)
        {
            if (e.IsEmpty()) return;

            if (e.items)
            {
                Refresh();
            }

            var s = State.Ex<ShardStore_StateEx>();

            var pos = transform.position;
            if (!FloatUtils.IsEquals(s.GetX(), pos.x))
            {
                pos.x = s.GetX();
                transform.position = pos;
            }

            if (e.visible && gameObject.activeSelf != s.GetVisible())
            {
                gameObject.SetActive(s.GetVisible());
                if (s.GetVisible())
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
            State.Ex<ShardStore_StateEx>().SetVisible(false);
            gameObject.SetActive(false);
        }

        private void Refresh()
        {
            var s = State.Ex<ShardStore_StateEx>();
            
            var length = s.GetItems().Count;

            var tr = transform;
            var gridWidth = (grid.cellSize.x + grid.spacing.x * 2) * (length + 1);
            ((RectTransform)(tr)).SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, gridWidth);

            var fisicCount = grid.transform.childCount;
            var index = 0;
            foreach (var item in s.GetItems())
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
                ShardUtils.Set(ref shard, item.shardType, (byte)Calc.GetQuantityForLevel(level));
                ShardService.PrecalcAllCosts(ref shard);

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
            var infoPanel = State.Ex<InfoPanel_StateExtension>();
            var coll = State.Ex<ShardCollection_StateExtension>();
            
            ref var shard = ref shardUIButton.GetShard();
            coll.SetHoveredItem(shardUIButton);

            infoPanel.Clear();
            infoPanel.SetShard(ref shard);
            infoPanel.SetTitle("Buying shard");
            infoPanel.SetPrice(shard.cost);
            infoPanel.SetVisible(true);
        }

        private void OnShardPointerExited(ShardUIButton shardButton)
        {
            var si = State.Ex<InfoPanel_StateExtension>();
            var sc = State.Ex<ShardCollection_StateExtension>();

            if (sc.GetHoveredItem() == shardButton)
            {
                sc.SetHoveredItem(null);
                if (
                    si.HasShard() && 
                    CommonUtils.IdsIsEquals(si.GetShard()._id_, shardButton.GetShard()._id_)
                )
                {
                    si.Clear();
                }
            }
        }

        private void OnShardPointerClicked(ShardUIButton shardButton)
        {
            var si = State.Ex<InfoPanel_StateExtension>();
            var sc = State.Ex<ShardCollection_StateExtension>();
            var s = State.Ex<ShardStore_StateEx>();

            if (
                !shardButton.hasShard ||
                shardButton.cost <= 0 ||
                State.GetEnergy() < shardButton.cost ||
                sc.GetItems().Count + 1 > sc.GetMaxItems()
            ) return;

            var newShard = shardButton.GetShard().MakeCopy();

            State.SetEnergy(State.GetEnergy() - shardButton.cost);

            // state.ShardStore.Remove(state.ShardStore[index]);
            sc.AddItem(ref newShard);

            // state.RefreshShardStore();
            sc.UpdateItems();
            
            s.SetVisible(false);
        }

        private void RefreshLevel()
        {
            levelText.text = level.ToString();
        }

        private void ChangeLevel(int l)
        {
            var newLevel = MathFast.Clamp(level + l, 1, 10);
            if (level == newLevel) return;

            level = (byte)newLevel;
            RefreshLevel();
            Refresh();
        }

        private void OnClose()
        {
            State.Ex<ShardStore_StateEx>().SetVisible(false);
        }

        private void OnDestroy()
        {
            Events.unique.RemoveListener<Event_ShardStore_StateChanged>(OnStateChanged);
            Events.unique.RemoveListener<Event_LevelFinished>(OnLevelFinished);
            Events.unique.RemoveListener<Event_YouDied>(OnYouDied);
            
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
            State.Ex<ShardCollection_StateExtension>().Clear();
        }
    }
}