using System.Text.RegularExpressions;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Leopotam.EcsLite.Unity.Ugui;
using td.features.state;
using td.utils.ecs;
using TMPro;
using UnityEngine;

namespace td.features.ui
{
    public class UIUpdateSystem : IEcsRunSystem
    {
        [Inject] private State state;
        
        private readonly EcsFilterInject<Inc<StateChangedEvent>> entities = Constants.Worlds.Outer;
        
        [EcsUguiNamed(Constants.UI.Components.LivesLabel)] private TMP_Text livesLabelText;
        [EcsUguiNamed(Constants.UI.Components.MoneyLabel)] private TMP_Text moneyLabelText;
        [EcsUguiNamed(Constants.UI.Components.WaveLabel)] private GameObject waveLabel;
        [EcsUguiNamed(Constants.UI.Components.WaveLabel)] private TMP_Text waveLabelText;
        [EcsUguiNamed(Constants.UI.Components.EnemiesLabel)] private TMP_Text enemiesLabelText;
        
        [EcsUguiNamed("NewWaveTimer")] private GameObject newWaveTimerContainer;
        [EcsUguiNamed("NewWaveTimer_Timer")] private TMP_Text newWaveTimer;
        
        private readonly Regex oneNumberRegex = new(@"[\d#.-]+");
        private readonly Regex waveRegex = new(@"(\d+|#+)/(\d+|#+)");
        
        public void Run(IEcsSystems systems)
        {
            foreach (var entity in entities.Value)
            {
                var data = entities.Pools.Inc1.Get(entity);

                if (livesLabelText != null && data.lives == true)
                {
                    livesLabelText.text = oneNumberRegex.Replace(livesLabelText.text, IntegerFormat(state.Lives));
                }

                //
                if (moneyLabelText != null && data.money == true)
                {
                    moneyLabelText.text = Constants.UI.CurrencySign + IntegerFormat(state.Money);
                }
                
                //
                if (waveLabelText != null && (data.waveNumber.HasValue || data.waveCount == true))
                {
                    if (state.WaveNumber != 0 && state.WaveCount != 0)
                    {
                        waveLabel.SetActive(true);
                        waveLabelText.text = waveRegex.Replace(waveLabelText.text, $@"{state.WaveNumber}/{state.WaveCount}");
                    }
                    else
                    {
                        waveLabel.SetActive(false);
                    }
                }
                
                if (newWaveTimer != null && data.nextWaveCountdown == true)
                {
                    if (state.NextWaveCountdown > 0)
                    {
                        newWaveTimerContainer.SetActive(true);
                        var text = $"{state.NextWaveCountdown:0.00}";
                        var l = text.Split('.');
                        newWaveTimer.text = $"{l[0]}<size=75%>:{l[1]}</size>";
                    }
                    else
                    {
                        newWaveTimerContainer.SetActive(false);
                    }
                }

                if (enemiesLabelText != null && data.enemiesCount == true)
                {
                    enemiesLabelText.text = oneNumberRegex.Replace(enemiesLabelText.text, IntegerFormat(state.EnemiesCount));
                }
            }
        }

        private string IntegerFormat(float number) => number.ToString("N0").Replace(',', '\'');
        private string IntegerFormat(int number) => number.ToString("N0").Replace(',', '\'');
    }
}