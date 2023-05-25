using System.Text.RegularExpressions;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Leopotam.EcsLite.Unity.Ugui;
using TMPro;
using UnityEngine;

namespace td.features.ui
{
    public class UpdateUISystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<UpdateUIOuterCommand>> entities = Constants.Worlds.Outer;
        
        [EcsUguiNamed(Constants.UI.Components.LivesLabel)] private TMP_Text livesLabelText;
        [EcsUguiNamed(Constants.UI.Components.MoneyLabel)] private TMP_Text moneyLabelText;
        // [EcsUguiNamed(Constants.UI.Components.WaveCountdown)] private GameObject waveCountdownLabel;
        // [EcsUguiNamed(Constants.UI.Components.WaveCountdownLabel)] private TMP_Text waveCountdownLabelText;
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

                if (livesLabelText != null && data.lives != null)
                {
                    livesLabelText.text = oneNumberRegex.Replace(livesLabelText.text, ((int)data.lives).ToString());
                }

                //
                if (moneyLabelText != null && data.money != null)
                {
                    moneyLabelText.text = oneNumberRegex.Replace(moneyLabelText.text, data.money.ToString());
                }
                
                //
                if (waveLabelText != null && data is { waveNumber: not null, waveCount: not null })
                {
                    if (data.waveNumber != 0 && data.waveCount != 0)
                    {
                        waveLabel.SetActive(true);
                        waveLabelText.text = waveRegex.Replace(waveLabelText.text, $@"{data.waveNumber}/{data.waveCount}");
                    }
                    else
                    {
                        waveLabel.SetActive(false);
                    }
                }

                // if (waveCountdownLabelText != null && data.NextWaveCountdown != null)
                // {
                //     if (data.NextWaveCountdown > 0)
                //     {
                //         waveCountdownLabel.SetActive(true);
                //         waveCountdownLabelText.text = oneNumberRegex.Replace(waveCountdownLabelText.text, data.NextWaveCountdown.ToString());
                //     }
                //     else
                //     {
                //         waveCountdownLabel.SetActive(false);
                //     }
                // }
                if (newWaveTimer != null && data.nextWaveCountdown != null)
                {
                    if (data.nextWaveCountdown > 0)
                    {
                        newWaveTimerContainer.SetActive(true);
                        var text = $"{data.nextWaveCountdown:0.00}";
                        var l = text.Split('.');
                        newWaveTimer.text = $"{l[0]}<size=75%>:{l[1]}</size>";
                    }
                    else
                    {
                        newWaveTimerContainer.SetActive(false);
                    }
                }

                if (enemiesLabelText != null && data.enemiesCount != null)
                {
                    enemiesLabelText.text =
                        oneNumberRegex.Replace(enemiesLabelText.text, data.enemiesCount.ToString());
                }
            }
        }
    }
}