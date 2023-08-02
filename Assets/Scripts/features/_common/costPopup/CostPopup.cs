using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Leopotam.EcsLite;
using td.features.gameStatus.bus;
using td.features.level.bus;
using td.features.state;
using td.utils.di;
using TMPro;
using UnityEngine;

namespace td.features._common.costPopup
{
    public class CostPopup : MonoInjectable
    {
        [SerializeField] private TMP_Text tTitle;
        [SerializeField] private TMP_Text tCostGood;
        [SerializeField] private TMP_Text tCostBad;

        private readonly EcsInject<IState> state;
        private readonly EcsInject<IEventBus> events;

        private Task currentTask;
        
        private readonly List<IDisposable> eventDisposers = new(3);
        
        private void Start()
        {
            eventDisposers.Add(events.Value.Unique.SubscribeTo<Event_StateChanged>(OnStateChanged));
            eventDisposers.Add(events.Value.Unique.SubscribeTo<Event_LevelFinished>(delegate { Hide(); }));
            eventDisposers.Add(events.Value.Unique.SubscribeTo<Event_YouDied>(delegate { Hide(); }));
        }

        private void OnDestroy()
        {
            foreach (var disposer in eventDisposers)
            {
                disposer?.Dispose();
            }

            eventDisposers.Clear();
        }

        private void OnStateChanged(ref Event_StateChanged e)
        {
            if (e.costPopup.IsEmpty) return;
            
            var s = state.Value.CostPopup;

            if (!s.Visible) Hide();
            else Show(s.Cost, s.IsFine, s.Title);
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
            state.Value.CostPopup.Visible = false;
            gameObject.SetActive(false);
        }
    }
}