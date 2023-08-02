using System;
using System.Text.RegularExpressions;
using Leopotam.EcsLite;
using NaughtyAttributes;
using td.features.state;
using td.utils.di;
using TMPro;
using UnityEngine;

namespace td.features.ui
{
    public class UI_UpdateService : MonoInjectable
    {
        [Required] [SerializeField] private TMP_Text livesLabelText;
        [Required] [SerializeField] private TMP_Text moneyLabelText;
        [Required] [SerializeField] private GameObject waveLabel;
        [Required] [SerializeField] private TMP_Text waveLabelText;
        [Required] [SerializeField] private TMP_Text enemiesLabelText;

        [Required] [SerializeField] private GameObject newWaveTimerContainer;
        [Required] [SerializeField] private TMP_Text newWaveTimer;

        private readonly Regex oneNumberRegex = new(@"[\d#.-]+");
        private readonly Regex waveRegex = new(@"(\d+|#+)/(\d+|#+)");

        private readonly EcsInject<IState> state;
        private readonly EcsInject<IEventBus> events;
        
        private IDisposable eventDispose;

        private void Start()
        {
            eventDispose = events.Value.Unique.SubscribeTo<Event_StateChanged>(OnStateChanged);
        }
        
        public void OnStateChanged(ref Event_StateChanged @event)
        {
            if (livesLabelText != null && @event.lives.HasValue)
            {
                livesLabelText.text = oneNumberRegex.Replace(livesLabelText.text, IntegerFormat(state.Value.Lives));
            }

            //
            if (moneyLabelText != null && @event.energy.HasValue)
            {
                moneyLabelText.text = Constants.UI.CurrencySign + IntegerFormat(state.Value.Energy);
            }

            //
            if (waveLabelText != null && (@event.waveNumber.HasValue || @event.waveCount.HasValue))
            {
                if (state.Value.WaveNumber != 0 && state.Value.WaveCount != 0)
                {
                    waveLabel.SetActive(true);
                    waveLabelText.text =
                        waveRegex.Replace(waveLabelText.text, $@"{state.Value.WaveNumber}/{state.Value.WaveCount}");
                }
                else
                {
                    waveLabel.SetActive(false);
                }
            }

            if (newWaveTimer != null && @event.nextWaveCountdown.HasValue)
            {
                if (state.Value.NextWaveCountdown > 0)
                {
                    newWaveTimerContainer.SetActive(true);
                    var text = $"{state.Value.NextWaveCountdown:0.00}";
                    var l = text.Contains('.')
                        ? text.Split('.')
                        : text.Split(',');
                    newWaveTimer.text = $"{l[0]}<size=75%>:{l[1]}</size>";
                }
                else
                {
                    newWaveTimerContainer.SetActive(false);
                }
            }

            if (enemiesLabelText != null && @event.enemiesCount.HasValue)
            {
                enemiesLabelText.text =
                    oneNumberRegex.Replace(enemiesLabelText.text, IntegerFormat(state.Value.EnemiesCount));
            }
        }
        
        private static string IntegerFormat(float number) => number.ToString("N0").Replace(',', '\'').Replace('.', '\'');
        private static string IntegerFormat(int number) => number.ToString("N0").Replace(',', '\'').Replace('.', '\'');
    }
}