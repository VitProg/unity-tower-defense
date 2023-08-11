using System.Threading.Tasks;
using Leopotam.EcsProto.QoL;
using td.features._common;
using td.features.eventBus;
using td.features.gameStatus.bus;
using td.features.level.bus;
using td.features.state;
using td.utils.di;
using TMPro;
using UnityEngine;

namespace td.features.costPopup
{
    public class CostPopup : MonoInjectable
    {
        [SerializeField] private TMP_Text tTitle;
        [SerializeField] private TMP_Text tCostGood;
        [SerializeField] private TMP_Text tCostBad;

        [DI] private State state;
        [DI] private EventBus events;

        private Task currentTask;
        private void Start()
        {
            events.unique.ListenTo<Event_StateChanged>(OnStateChanged);
            events.unique.ListenTo<Event_LevelFinished>(OnLevelFinished);
            events.unique.ListenTo<Event_YouDied>(OnYouDied);
        }
        
        private void OnDestroy()
        {
            events.unique.RemoveListener<Event_StateChanged>(OnStateChanged);
            events.unique.RemoveListener<Event_LevelFinished>(OnLevelFinished);
            events.unique.RemoveListener<Event_YouDied>(OnYouDied);
        }

        private void OnStateChanged(ref Event_StateChanged e)
        {
            if (e.costPopup.IsEmpty) return;
            
            var s = state.CostPopup;

            if (!s.Visible) Hide();
            else Show(s.Cost, s.IsFine, s.Title);
        }
        
        private void OnYouDied(ref Event_YouDied obj)
        {
            Hide();
        }

        private void OnLevelFinished(ref Event_LevelFinished obj)
        {
            Hide();
        }

        private void Show(uint cost, bool isFine, string title /*, uint time = 3000*/)
        {
            currentTask?.Dispose();

            var text = CommonUtils.CostFormat(cost);
            tTitle.text = title;
            tCostGood.text = text;
            tCostBad.text = text;
            tCostGood.gameObject.SetActive(isFine);
            tCostBad.gameObject.SetActive(!isFine);
            gameObject.SetActive(true);

            // await Task.Yield();
            // currentTask = Task.Delay((int)time);
            // await currentTask;

            // Hide();
        }

        // public void ShowCombineCost(uint cost, bool good) => Show(cost, good, "Combine Shards");
        // public void ShowInsertCost(uint cost, bool good) => Show(cost, good, "Integrate Shard");

        public void Hide()
        {
            state.CostPopup.Visible = false;
            gameObject.SetActive(false);
        }
    }
}