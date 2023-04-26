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
        [EcsUguiNamed(Constants.UI.Components.WaveCountdown)] private GameObject waveCountdownLabel;
        [EcsUguiNamed(Constants.UI.Components.WaveCountdownLabel)] private TMP_Text waveCountdownLabelText;
        [EcsUguiNamed(Constants.UI.Components.WaveLabel)] private GameObject waveLabel;
        [EcsUguiNamed(Constants.UI.Components.WaveLabel)] private TMP_Text waveLabelText;
        [EcsUguiNamed(Constants.UI.Components.EnemiesLabel)] private TMP_Text enemiesLabelText;
        
        private readonly Regex oneNumberRegex = new(@"[\d#.-]+");
        private readonly Regex waveRegex = new(@"(\d+|#+)/(\d+|#+)");

        
        public void Run(IEcsSystems systems)
        {
            foreach (var entity in entities.Value)
            {
                var data = entities.Pools.Inc1.Get(entity);

                if (livesLabelText != null && data.Lives != null)
                {
                    livesLabelText.text = oneNumberRegex.Replace(livesLabelText.text, ((int)data.Lives).ToString());
                }

                //
                if (moneyLabelText != null && data.Money != null)
                {
                    moneyLabelText.text = oneNumberRegex.Replace(moneyLabelText.text, data.Money.ToString());
                }
                
                //
                if (waveLabelText != null && data is { WaveNumber: not null, WaveCount: not null })
                {
                    if (data.WaveNumber != 0 && data.WaveCount != 0)
                    {
                        waveLabel.SetActive(true);
                        waveLabelText.text = waveRegex.Replace(waveLabelText.text, $@"{data.WaveNumber}/{data.WaveCount}");
                    }
                    else
                    {
                        waveLabel.SetActive(false);
                    }
                }

                if (waveCountdownLabelText != null && data.NextWaveCountdown != null)
                {
                    if (data.NextWaveCountdown > 0)
                    {
                        waveCountdownLabel.SetActive(true);
                        waveCountdownLabelText.text = oneNumberRegex.Replace(waveCountdownLabelText.text, data.NextWaveCountdown.ToString());
                    }
                    else
                    {
                        waveCountdownLabel.SetActive(false);
                    }
                }

                if (enemiesLabelText != null && data.EnemiesCount != null)
                {
                    enemiesLabelText.text =
                        oneNumberRegex.Replace(enemiesLabelText.text, data.EnemiesCount.ToString());
                }
            }
        }
    }
}