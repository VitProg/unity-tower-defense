using System.Text.RegularExpressions;
using NaughtyAttributes;
using td.features.eventBus;
using td.features.state;
using td.utils.di;
using TMPro;
using UnityEngine;

namespace td.features.ui
{
    public class UI_UpdateService : MonoBehaviour
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

        private State State =>  ServiceContainer.Get<State>();
        private EventBus Events =>  ServiceContainer.Get<EventBus>();

        private void Start()
        {
            Events.unique.ListenTo<Event_StateChanged>(OnStateChanged);
        }

        private void OnDestroy()
        {
            Events.unique.RemoveListener<Event_StateChanged>(OnStateChanged);
        }

        public void OnStateChanged(ref Event_StateChanged ev)
        {
            if (livesLabelText != null && ev.lives)
            {
                livesLabelText.text = oneNumberRegex.Replace(livesLabelText.text, IntegerFormat(State.GetLives()));
            }

            //
            if (moneyLabelText != null && ev.energy)
            {
                moneyLabelText.text = Constants.UI.CurrencySign + IntegerFormat(State.GetEnergy());
            }

            //
            if (waveLabelText != null && (ev.waveNumber || ev.waveCount))
            {
                if (State.GetWaveNumber() != 0 && State.GetWaveCount() != 0)
                {
                    waveLabel.SetActive(true);
                    waveLabelText.text =
                        waveRegex.Replace(waveLabelText.text, $@"{State.GetWaveNumber()}/{State.GetWaveCount()}");
                }
                else
                {
                    waveLabel.SetActive(false);
                }
            }

            if (newWaveTimer != null && ev.nextWaveCountdown)
            {
                if (State.GetNextWaveCountdown() > 0)
                {
                    newWaveTimerContainer.SetActive(true);
                    var text = $"{State.GetNextWaveCountdown():0.00}";
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

            if (enemiesLabelText != null && ev.enemiesCount)
            {
                enemiesLabelText.text =
                    oneNumberRegex.Replace(enemiesLabelText.text, IntegerFormat(State.GetEnemiesCount()));
            }
        }
        
        private static string IntegerFormat(float number) => number.ToString("N0").Replace(',', '\'').Replace('.', '\'');
        private static string IntegerFormat(int number) => number.ToString("N0").Replace(',', '\'').Replace('.', '\'');
    }
}