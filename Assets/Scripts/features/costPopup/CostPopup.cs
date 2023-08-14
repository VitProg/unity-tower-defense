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
    public class CostPopup : MonoBehaviour
    {
        [SerializeField] private TMP_Text tTitle;
        [SerializeField] private TMP_Text tCostGood;
        [SerializeField] private TMP_Text tCostBad;

        private State State =>  ServiceContainer.Get<State>();
        private EventBus Events =>  ServiceContainer.Get<EventBus>();

        private Task currentTask;
        private void Start()
        {
            Events.unique.ListenTo<Event_CostPopup_StateChanged>(OnStateChanged);
            Events.unique.ListenTo<Event_LevelFinished>(OnLevelFinished);
            Events.unique.ListenTo<Event_YouDied>(OnYouDied);
        }
        
        private void OnDestroy()
        {
            Events.unique.RemoveListener<Event_CostPopup_StateChanged>(OnStateChanged);
            Events.unique.RemoveListener<Event_LevelFinished>(OnLevelFinished);
            Events.unique.RemoveListener<Event_YouDied>(OnYouDied);
        }

        private void OnStateChanged(ref Event_CostPopup_StateChanged e)
        {
            if (e.IsEmpty()) return;
            
            var s = State.Ex<CostPopup_StateExtension>();

            if (!s.GetVisible()) Hide();
            else Show(s.GetCost(), s.GetIsFine(), s.GetTitle());
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
            State.Ex<CostPopup_StateExtension>().SetVisible(false);
            // state.CostPopup.Visible = false;
            gameObject.SetActive(false);
        }
    }
}