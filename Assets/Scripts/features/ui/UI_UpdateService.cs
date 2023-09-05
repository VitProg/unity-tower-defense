using System;
using System.Text.RegularExpressions;
using NaughtyAttributes;
using td.features.eventBus;
using td.features.state;
using td.features.state.bus;
using td.features.wave;
using td.utils;
using td.utils.di;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace td.features.ui
{
    public class UI_UpdateService : MonoBehaviour
    {
        // [Required] [SerializeField] private TMP_Text livesLabelText;
        // [Required] [SerializeField] private TMP_Text moneyLabelText;
        [Required] [SerializeField] private UI_SliderBar barLives;
        [Required] [SerializeField] private UI_SliderBar barEnergy;
        [Required] [SerializeField] private TMP_Text tWave;
        [Required] [SerializeField] private TMP_Text tEnemies;

        // [Required] [SerializeField] private GameObject newWaveTimerContainer;
        // [Required] [SerializeField] private TMP_Text newWaveTimer;

        private readonly Regex oneNumberRegex = new(@"[\d#.-]+");
        private readonly Regex waveRegex = new(@"(\d+|#+)/(\d+|#+)");

        private State _state;
        private State State =>  _state ??= ServiceContainer.Get<State>();
        private Wave_State _waveState;
        private Wave_State WaveState =>  _waveState ??= ServiceContainer.Get<Wave_State>();
        private EventBus _events;
        private EventBus Events => _events ??= ServiceContainer.Get<EventBus>();

        private void Start()
        {
            Events.unique.ListenTo<Event_StateChanged>(OnStateChanged);
            Events.unique.ListenTo<Event_Wave_StateChanged>(OnWaveStateChanged);
        }
        
        private void OnDestroy()
        {
            Events.unique.RemoveListener<Event_StateChanged>(OnStateChanged);
            Events.unique.RemoveListener<Event_Wave_StateChanged>(OnWaveStateChanged);
        }
        
        // ----------------------------------------------------------

        public void OnStateChanged(ref Event_StateChanged ev)
        {
            if (barLives != null && (ev.lives || ev.maxLives)){
                barLives.value = (uint)State.GetLives();
                barLives.maxValue = (uint)State.GetMaxLives();
                barLives.Refresh();
            }

            if (barEnergy != null && (ev.energy || ev.maxEnergy)){
                barEnergy.value = State.GetEnergy();
                barEnergy.maxValue = State.GetMaxEnergy();
                barEnergy.Refresh();
            }
        }

        private void OnWaveStateChanged(ref Event_Wave_StateChanged ev)
        {
            //
            if (tWave != null && (ev.waveNumber || ev.waveCount))
            {
                if (WaveState.GetWaveNumber() >= 0 && WaveState.GetWaveCount() > 0)
                {
                    tWave.gameObject.SetActive(true);
                    tWave.text = $"Wave: {Math.Max(WaveState.GetWaveNumber(), 1)}/{WaveState.GetWaveCount()}";
                }
                else
                {
                    tWave.gameObject.SetActive(false);
                }
            }

            /*if (newWaveTimer != null && ev.nextWaveCountdown)
            {
                if (!WaveState.GetWaiting() && !WaveState.IsWaveActive() && !WaveState.AreAllWavesComplete() && WaveState.GetWaveNumber() >= 0 && WaveState.GetNextWaveCountdown() > 0)
                {
                    newWaveTimerContainer.SetActive(true);
                    var text = $"{WaveState.GetNextWaveCountdown():0.00}";
                    var l = text.Contains('.')
                        ? text.Split('.')
                        : text.Split(',');
                    newWaveTimer.text = $"{l[0]}<size=75%>:{l[1]}</size>";
                }
                else
                {
                    newWaveTimerContainer.SetActive(false);
                }
            }*/

            if (tEnemies != null && ev.enemiesCount)
            {
                tEnemies.text = $"Enemies: {IntegerFormat(WaveState.GetEnemiesCount())}";
            }
        }

        private static string IntegerFormat(float number) => number.ToString("N0").Replace(',', '\'').Replace('.', '\'');
        private static string IntegerFormat(int number) => number.ToString("N0").Replace(',', '\'').Replace('.', '\'');
    }
}