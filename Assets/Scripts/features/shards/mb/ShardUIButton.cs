using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace td.features.shards.mb
{
    public class ShardUIButton : MonoBehaviour
    {
        public ShardMonoBehaviour shardUI;
        public Image plus;
        public TMP_Text costText;

        [OnValueChanged("Refresh")]
        public bool hasShard = false;

        [OnValueChanged("Refresh")]
        public bool showPlus = false;

        [OnValueChanged("Refresh")]
        public int cost = 0;

        public bool druggable = false;

        private void Start()
        {
            shardUI ??= transform.GetComponentInChildren<ShardMonoBehaviour>();
            costText ??= transform.GetComponentInChildren<TMP_Text>();
            plus ??= transform.GetComponentInChildren<Image>();

            Refresh();
        }

        [Button("Refresh UI")]
        public void Refresh()
        {
            shardUI.gameObject.SetActive(hasShard);
            plus.gameObject.SetActive(!hasShard && showPlus);

            if (hasShard)
            {
                shardUI.UpdateFromEntity();
                shardUI.Refresh();
            }

            if (cost > 0 && hasShard)
            {
                costText.text = $"<size=80%>{Constants.UI.CurrencySign}</size>{cost:N0}".Replace(',', '\'');
                costText.gameObject.SetActive(true);
            }
            else
            {
                costText.gameObject.SetActive(false);
            }
        }
    }
}