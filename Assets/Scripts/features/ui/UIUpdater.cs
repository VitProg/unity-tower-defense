using System;
using System.Text.RegularExpressions;
using NaughtyAttributes;
using td.features.eventBus;
using td.features.state;
using td.utils.ecs;
using TMPro;
using UnityEngine;

namespace td.features.ui
{
    public class UIUpdater : MonoBehaviour, IEventReceiver<StateChangedEvent>
    {
        public UniqueId Id { get; } = new();

        [Required] [SerializeField] private TMP_Text livesLabelText;
        [Required] [SerializeField] private TMP_Text moneyLabelText;
        [Required] [SerializeField] private GameObject waveLabel;
        [Required] [SerializeField] private TMP_Text waveLabelText;
        [Required] [SerializeField] private TMP_Text enemiesLabelText;

        [Required] [SerializeField] private GameObject newWaveTimerContainer;
        [Required] [SerializeField] private TMP_Text newWaveTimer;

        private readonly Regex oneNumberRegex = new(@"[\d#.-]+");
        private readonly Regex waveRegex = new(@"(\d+|#+)/(\d+|#+)");

        [Inject] private State state;

        public async void Start()
        {
            await DI.Resolve(this);
            DI.Get<EventBus>()!.Subscribe(this);
            Refresh();
        }

        private void Refresh()
        {
            OnEvent(StateChangedEvent.FromState(state));
        }

        public void OnEvent(StateChangedEvent @event)
        {
            if (livesLabelText != null && @event.lives.HasValue)
            {
                livesLabelText.text = oneNumberRegex.Replace(livesLabelText.text, IntegerFormat(state.Lives));
            }

            //
            if (moneyLabelText != null && @event.money.HasValue)
            {
                moneyLabelText.text = Constants.UI.CurrencySign + IntegerFormat(state.Money);
            }

            //
            if (waveLabelText != null && (@event.waveNumber.HasValue || @event.waveCount.HasValue))
            {
                if (state.WaveNumber != 0 && state.WaveCount != 0)
                {
                    waveLabel.SetActive(true);
                    waveLabelText.text =
                        waveRegex.Replace(waveLabelText.text, $@"{state.WaveNumber}/{state.WaveCount}");
                }
                else
                {
                    waveLabel.SetActive(false);
                }
            }

            if (newWaveTimer != null && @event.nextWaveCountdown.HasValue)
            {
                if (state.NextWaveCountdown > 0)
                {
                    newWaveTimerContainer.SetActive(true);
                    var text = $"{state.NextWaveCountdown:0.00}";
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
                    oneNumberRegex.Replace(enemiesLabelText.text, IntegerFormat(state.EnemiesCount));
            }
        }
        
        private static string IntegerFormat(float number) => number.ToString("N0").Replace(',', '\'').Replace('.', '\'');
        private static string IntegerFormat(int number) => number.ToString("N0").Replace(',', '\'').Replace('.', '\'');
    }
}