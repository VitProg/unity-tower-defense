using System.Runtime.CompilerServices;
using td.features._common;
using td.features.eventBus;
using td.features.level.bus;
using td.features.state;
using td.features.state.bus;
using td.utils.di;
using TMPro;
using UnityEngine;

namespace td.features.priceTimePopup
{
    public class UI_PriceTimePopup : MonoBehaviour
    {
        [SerializeField] private TMP_Text tTitle;
        [SerializeField] private TMP_Text tCostGood;
        [SerializeField] private TMP_Text tCostBad;
        [SerializeField] private TMP_Text tTime;

        private State _state;
        private State State =>  _state ??= ServiceContainer.Get<State>();
        private PriceTimePopup_State _popupState;
        private PriceTimePopup_State PopupState =>  _popupState ??= ServiceContainer.Get<PriceTimePopup_State>();
        private EventBus _events;
        private EventBus Events => _events ??= ServiceContainer.Get<EventBus>();

        private void Start()
        {
            Events.unique.ListenTo<Event_StateChanged>(OnStateChanged);
            Events.unique.ListenTo<Event_PriceTimePopup_StateChanged>(OnPriceTimePopupStateChanged);
            Events.unique.ListenTo<Event_LevelFinished>(OnLevelFinished);
        }
        
        private void OnDestroy()
        {
            Events.unique.RemoveListener<Event_StateChanged>(OnStateChanged);
            Events.unique.RemoveListener<Event_PriceTimePopup_StateChanged>(OnPriceTimePopupStateChanged);
            Events.unique.RemoveListener<Event_LevelFinished>(OnLevelFinished);
        }
        
        // ----------------------------------------------------------------

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        private void OnStateChanged(ref Event_StateChanged ev)
        {
            if (ev.lives && State.IsDead()) PopupState.SetVisible(false);
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        private void OnLevelFinished(ref Event_LevelFinished obj) => PopupState.SetVisible(false);

        private void OnPriceTimePopupStateChanged(ref Event_PriceTimePopup_StateChanged e)
        {
            if (e.IsEmpty()) return;

            if (!PopupState.GetVisible())
            {
                gameObject.SetActive(false);
                return;
            }

            tTitle.text = PopupState.GetTitle();
            
            var time = PopupState.GetTime();
            if (time > 0) {
                tTime.text = CommonUtils.TMPTimeFormat(time);
                tTime.gameObject.SetActive(true);
            } else tTime.gameObject.SetActive(false);
            
            var price = CommonUtils.PriceFormat(PopupState.GetPrice());
            tCostGood.text = price;
            tCostBad.text = price;
            tCostGood.gameObject.SetActive(PopupState.GetIsFine());
            tCostBad.gameObject.SetActive(!PopupState.GetIsFine());
            
            gameObject.SetActive(true);
        }
    }
}