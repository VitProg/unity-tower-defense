using System;
using td.features.eventBus;
using td.features.level.mb;
using td.features.wave.bus;
using td.utils;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;
using ServiceContainer = td.utils.di.ServiceContainer;

namespace td.features.wave {
    public class UI_WaveStart : MonoBehaviour, IPointerClickHandler {

        private EventBus _events;
        private EventBus Events => _events ??= ServiceContainer.Get<EventBus>();
        
        private Wave_State _waveState;
        private Wave_State WaveState => _waveState ??= ServiceContainer.Get<Wave_State>();

        public void Start() {
            Events.unique.ListenTo<Event_Wave_StateChanged>(OnWaveStateChanged);
        }

        private void OnDestroy() {
            Events.unique.RemoveListener<Event_Wave_StateChanged>(OnWaveStateChanged);
        }

        private void OnWaveStateChanged(ref Event_Wave_StateChanged ev) {
            if (WaveState.IsWaiting() || WaveState.IsNextWaveCountdown()) {
                Show();
            } else {
                Hide();
            }
        }

        private void Show() {
            gameObject.SetActive(true);
        }

        private void Hide() {
            gameObject.SetActive(false);
        }

        public void OnPointerClick(PointerEventData eventData) {
            Debug.Log("Command_StartNextWave");
            Events.unique.GetOrAdd<Command_StartNextWave>();
        }
    }
}
