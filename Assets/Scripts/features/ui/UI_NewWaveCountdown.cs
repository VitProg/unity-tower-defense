using System;
using System.Runtime.CompilerServices;
using NaughtyAttributes;
using td.features.eventBus;
using td.features.state;
using td.features.wave;
using td.utils.di;
using TMPro;
using UnityEngine;

namespace td.features.ui
{
    public class UI_NewWaveCountdown : MonoBehaviour
    {
        [Required][SerializeField] private TMP_Text tTimeS;
        [Required][SerializeField] private TMP_Text tTimeMS;

        private string[] timeSCache;
        private string[] timeMSCache;

        #region DI
        private EventBus _events;
        private EventBus Events => _events ??= ServiceContainer.Get<EventBus>();
        
        private Wave_State _waveState;
        private Wave_State WaveState => _waveState ??= ServiceContainer.Get<Wave_State>();
        #endregion

        private void Awake() {
            timeSCache = new string[100];
            for (var i = 0; i <= 99; i++) {
                timeSCache[i] = i.ToString();
            }
            timeMSCache = new string[100];
            for (var i = 0; i <= 99; i++) {
                timeMSCache[i] = ":" + (i < 10 ? "0" + i : i.ToString());
            }
            
            Events.unique.ListenTo<Event_Wave_StateChanged>(OnWaveStateChanged);
        }

        private void OnDestroy()
        {
            Events.unique.RemoveListener<Event_Wave_StateChanged>(OnWaveStateChanged);
        }
        
        //

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnWaveStateChanged(ref Event_Wave_StateChanged ev)
        {
            if (!ev.nextWaveCountdown) return;
            Refresh();
        }

        [Button]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Refresh()
        {
            if (WaveState.IsWaiting() || WaveState.IsWaveActive()) {
                gameObject.SetActive(false);
                return;
            }
            
            gameObject.SetActive(true);
            
            var t = (int)(WaveState.GetNextWaveCountdown() * 100);
            var s = t / 100;
            var ms = Math.Abs(t % 100);

#if DEBUG
            if (s >= timeSCache.Length) {
                Debug.LogWarning($"Index {s} out of range timeSCache {timeSCache.Length}");
            }   
            if (ms >= timeMSCache.Length) {
                Debug.LogWarning($"Index {ms} out of range timeMSCache {timeMSCache.Length}");
            }   
#endif
            
            tTimeS.text = timeSCache[s];
            tTimeMS.text = timeMSCache[ms];
        }
    }
}