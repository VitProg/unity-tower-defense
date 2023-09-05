using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using NaughtyAttributes;
using td.features.eventBus;
using td.features.level.bus;
using td.features.shard.bus;
using td.features.shard.components;
using td.features.shard.mb;
using td.features.shard.shardCollection;
using td.features.state;
using td.features.state.bus;
using td.utils;
using td.utils.di;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace td.features.shard.shardStore
{
    public class UI_ShardStore : MonoBehaviour
    {
        [Required] public Button closeButton;
        [Required] public GridLayoutGroup grid;
        [Required] public Button levelUp;
        [Required] public Button levelDown;
        [Required] public TMP_Text levelText;

        [SerializeField] private GameObject buttonPrefab;
        
        private ShardButtonsByType shardButtons;

        #region DI
        private EventBus _events;
        private EventBus Events => _events ??= ServiceContainer.Get<EventBus>();
        
        private State _state;
        private State State =>  _state ??= ServiceContainer.Get<State>();
        
        private ShardStore_State _stateEx;
        private ShardStore_State StoreState =>  _stateEx ??= State.Ex<ShardStore_State>();
        
        private Shard_Calculator _calc;
        private Shard_Calculator Calc => _calc ??= ServiceContainer.Get<Shard_Calculator>();
        
        private Shard_Service _shardService;
        private Shard_Service ShardService => _shardService ??= ServiceContainer.Get<Shard_Service>();
        #endregion
        
        private void Start()
        {
            grid ??= GetComponent<GridLayoutGroup>();
            closeButton.onClick.AddListener(OnClose);
            levelDown.onClick.AddListener(StoreState.ReduceLevel);
            levelUp.onClick.AddListener(StoreState.IncreaseLevel);
            
            Events.unique.ListenTo<Event_StateChanged>(OnStateChanged);
            Events.unique.ListenTo<Event_ShardStore_StateChanged>(OnStoreStateChanged);
            Events.unique.ListenTo<Event_LevelFinished>(OnLevelFinished);
            Events.unique.RemoveListener<Command_LoadLevel>(OnLevelLoad);

            for (var index = 0; index < grid.transform.childCount; index++)
            {
                var go = grid.transform.GetChild(index).gameObject;
                var b = go.GetComponent<UI_Shard_Button>();
                b.OnPointerClicked.RemoveAllListeners();
                b.OnPointerEntered.RemoveAllListeners();
                b.OnPointerExited.RemoveAllListeners();
                Destroy(go);
            }
        }
        
        private void OnDestroy()
        {
            Events.unique.RemoveListener<Event_StateChanged>(OnStateChanged);
            Events.unique.RemoveListener<Event_ShardStore_StateChanged>(OnStoreStateChanged);
            Events.unique.RemoveListener<Event_LevelFinished>(OnLevelFinished);
            
            for (var index = 0; index < grid.transform.childCount; index++)
            {
                var go = grid.transform.GetChild(index).gameObject;
                var b = go.GetComponent<UI_Shard_Button>();
                b.OnPointerClicked.RemoveAllListeners();
                b.OnPointerEntered.RemoveAllListeners();
                b.OnPointerExited.RemoveAllListeners();
                Destroy(go);
            }

            closeButton.onClick.RemoveAllListeners();
            levelDown.onClick.RemoveListener(StoreState.IncreaseLevel);
            levelUp.onClick.RemoveListener(StoreState.ReduceLevel);
            levelUp.onClick.RemoveAllListeners();
            levelDown.onClick.RemoveAllListeners();
            State.Ex<ShardCollection_State>().Clear();
        }
        
        // ----------------------------------------------------------------

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        private void OnStateChanged(ref Event_StateChanged ev)
        {
            if (ev.lives && State.IsDead()) Hide();
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        private void OnLevelLoad(ref Command_LoadLevel item) => Hide();

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        private void OnLevelFinished(ref Event_LevelFinished obj) => Hide();

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        private void Hide()
        {
            StoreState.SetVisible(false);
            gameObject.SetActive(false);
        }

        private void OnStoreStateChanged(ref Event_ShardStore_StateChanged e)
        {
            if (e.IsEmpty()) return;

            if (e.items || e.level)
            {
                Refresh();
            }

            if (e.level)
            {
                RefreshLevel();
            }

            var pos = transform.position;
            if (!FloatUtils.IsEquals(StoreState.GetX(), pos.x))
            {
                pos.x = StoreState.GetX();
                transform.position = pos;
            }

            if (e.visible && gameObject.activeSelf != StoreState.GetVisible())
            {
                gameObject.SetActive(StoreState.GetVisible());
                if (StoreState.GetVisible())
                {
                    for (var index = 0; index < grid.transform.childCount; index++)
                    {
                        grid.transform.GetChild(index).GetComponent<UI_Shard_Button>().Refresh();
                    }
                }
            }
        }

        private void Refresh()
        {
            var s = StoreState;
            
            var count = s.GetCount();

            var tr = transform;
            var gridWidth = (grid.cellSize.x + grid.spacing.x * 2) * (count + 1);
            ((RectTransform)(tr)).SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, gridWidth);
            
            shardButtons.Clear();

            var fisicCount = grid.transform.childCount;
            
            for (var idx = 0; idx < count; idx++)
            {
                ref var item = ref StoreState.GetItem(idx);
                
                var isNew = idx >= fisicCount;

                var go = isNew 
                    ? Instantiate(buttonPrefab, grid.transform)
                    : grid.transform.GetChild(idx).gameObject;

                var ui = go.GetComponent<UI_Shard_Button>();

                shardButtons.Set(item.shardType, ui);

                if (isNew)
                {
                    var storeItem = item;
                    ui.OnPointerClicked.AddListener(delegate { OnShardPointerClicked(storeItem.shardType); });
                    ui.OnPointerEntered.AddListener(delegate { OnShardPointerEntered(storeItem.shardType); });
                    ui.OnPointerExited.AddListener(delegate { OnShardPointerExited(storeItem.shardType); });
                }

                var level = StoreState.GetLevel();

                if (item.shard.level != level)
                {
                    ShardUtils.Set(ref item.shard, item.shardType, (byte)Calc.GetQuantityForLevel(level));
                    ShardService.PrecalcAllData(ref item.shard);
                    StoreState.ItemUpdated(item.shardType);
                }
                
                ui.showPlus = false;
                ui.price = item.shard.price;
                ui.hasShard = tr;
                ui.SetShard(ref item.shard);
                ui.Refresh();
            }

            for (var idx = count; idx < fisicCount; idx++)
            {
                var go = grid.transform.GetChild(idx).gameObject;
                var b = go.GetComponent<UI_Shard_Button>();
                b.OnPointerClicked.RemoveAllListeners();
                b.OnPointerEntered.RemoveAllListeners();
                b.OnPointerExited.RemoveAllListeners();
                Destroy(go);
            }
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        private void OnShardPointerEntered(ShardTypes shardType)
        {
            if (!StoreState.HasItem(shardType)) return;
            StoreState.SetHovered(shardType);
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        private void OnShardPointerExited(ShardTypes _) => StoreState.SetHoveredIndex(-1);

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        private void OnShardPointerClicked(ShardTypes shardType)
        {
            Debug.Log("OnShardPointerClicked: " + shardType);
            
            var shardButton = shardButtons.Get(shardType);
            if (!shardButton || !shardButton.hasShard) return;

            ref var cmd = ref Events.global.Add<Command_BuyShard>();
            cmd.shard = shardButton.GetShard();
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        private void RefreshLevel()
        {
            levelText.text = StoreState.GetLevel().ToString();
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        private void OnClose() => StoreState.SetVisible(false);
    }

    internal struct ShardButtonsByType {
        [CanBeNull] private UI_Shard_Button red;
        [CanBeNull] private UI_Shard_Button green;
        [CanBeNull] private UI_Shard_Button blue;
        [CanBeNull] private UI_Shard_Button yellow;
        [CanBeNull] private UI_Shard_Button orange;
        [CanBeNull] private UI_Shard_Button pink;
        [CanBeNull] private UI_Shard_Button violet;
        [CanBeNull] private UI_Shard_Button aquamarine;

        public void Clear()
        {
            red = null;
            green = null;
            blue = null;
            yellow = null;
            orange = null;
            pink = null;
            violet = null;
            aquamarine = null;
        }

        public void Set(ShardTypes shardType, UI_Shard_Button ui)
        {
            switch (shardType)
            {
                case ShardTypes.Red:
                    red = ui;
                    break;
                case ShardTypes.Green:
                    green = ui;
                    break;
                case ShardTypes.Blue:
                    blue = ui;
                    break;
                case ShardTypes.Yellow:
                    yellow = ui;
                    break;
                case ShardTypes.Orange:
                    orange = ui;
                    break;
                case ShardTypes.Pink:
                    pink = ui;
                    break;
                case ShardTypes.Violet:
                    violet = ui;
                    break;
                case ShardTypes.Aquamarine:
                    aquamarine = ui;
                    break;
                default:
                    break;
            }
        }       
        
        public UI_Shard_Button Get(ShardTypes shardType) =>
            shardType switch
            {
                ShardTypes.Red => red,
                ShardTypes.Green => green,
                ShardTypes.Blue => blue,
                ShardTypes.Yellow => yellow,
                ShardTypes.Orange => orange,
                ShardTypes.Pink => pink,
                ShardTypes.Violet => violet,
                ShardTypes.Aquamarine => aquamarine,
                _ => null
            };
    }
}