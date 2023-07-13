using td.common;
using td.utils.ecs;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace td.features.shards.mb
{
    public class CombineShardCostPopup : MonoBehaviour
    {
        [SerializeField] private TMP_Text tTitle;
        [SerializeField] private TMP_Text tCostGood;
        [SerializeField] private TMP_Text tCostBad;

        private void Show(int cost, bool good, string title)
        {
            var text = $"<size=80%>{Constants.UI.CurrencySign}</size>{cost:N0}".Replace(',', '\'');
            tTitle.text = title;
            tCostGood.text = text;
            tCostBad.text = text;
            tCostGood.gameObject.SetActive(good);
            tCostBad.gameObject.SetActive(!good);
            gameObject.SetActive(true);
        }

        public void ShowCombineCost(int cost, bool good) => Show(cost, good, "Combine Shards");
        public void ShowIntegrateCost(int cost, bool good) => Show(cost, good, "Integrate Shard");

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}