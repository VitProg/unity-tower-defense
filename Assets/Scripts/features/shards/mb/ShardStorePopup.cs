using NaughtyAttributes;
using td.features.shards.commands;
using td.features.shards.events;
using td.utils.ecs;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace td.features.shards.mb
{
    public class ShardStorePopup : MonoBehaviour
    {
        public Button closeButton;
        public GridLayoutGroup grid;
        public Button levelUp;
        public Button levelDown;
        public TMP_Text levelText;

        [OnValueChanged("RefreshLevel")]
        [MinValue(1), MaxValue(10)]
        public byte level = 1;

        private void Start()
        {
            grid ??= GetComponent<GridLayoutGroup>();
            closeButton.onClick.AddListener(OnClose);
            levelDown.onClick.AddListener(delegate { ChangeLevel(-1); });
            levelUp.onClick.AddListener(delegate { ChangeLevel(1); });
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
            DI.Systems.OuterSingle<UIShardStoreLevelChangedOuterEvent>().level = level;
        }

        private void OnClose()
        {
            DI.Systems.OuterSingle<UIHideShardStoreOuterCommand>();
            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            closeButton.onClick.RemoveAllListeners();
            levelUp.onClick.RemoveAllListeners();
            levelDown.onClick.RemoveAllListeners();
        }
    }
}