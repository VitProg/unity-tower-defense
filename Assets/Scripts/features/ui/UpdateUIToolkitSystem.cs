using System.Text.RegularExpressions;
using JetBrains.Annotations;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine.UIElements;

namespace td.features.ui
{
    public class UpdateUIToolkitSystem : IEcsRunSystem, IEcsInitSystem
    {
        private readonly EcsFilterInject<Inc<UpdateUIOuterCommand>> entities = Constants.Worlds.Outer;

        [CanBeNull] private Label livesLebel;
        [CanBeNull] private Label moneyLebel;
        [CanBeNull] private Label waveLabel;
        [CanBeNull] private Label waveCountdownLabel;
        [CanBeNull] private Label enemiesCounterLabel;
        
        private Button bayTowerButton;

        private readonly Regex oneNumberRegex = new(@"[\d#.-]+");
        private readonly Regex waveRegex = new(@"(\d+|#+)/(\d+|#+)");

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in entities.Value)
            {
                var data = entities.Pools.Inc1.Get(entity);

                if (livesLebel != null && data.Lives != null)
                {
                    livesLebel.text = oneNumberRegex.Replace(livesLebel.text, ((int)data.Lives).ToString());
                }

                //
                if (moneyLebel != null && data.Money != null)
                {
                    moneyLebel.text = oneNumberRegex.Replace(moneyLebel.text, data.Money.ToString());
                }

                //
                if (waveLabel != null && data is { WaveNumber: not null, WaveCount: not null })
                {
                    if (data.WaveNumber != 0 && data.WaveCount != 0)
                    {
                        waveLabel.visible = true;
                        waveLabel.text = waveRegex.Replace(waveLabel.text, $@"{data.WaveNumber}/{data.WaveCount}");
                    }
                    else
                    {
                        waveLabel.visible = false;
                    }
                }

                //
                if (waveCountdownLabel != null && data.NextWaveCountdown != null)
                {
                    if (data.NextWaveCountdown > 0)
                    {
                        waveCountdownLabel.visible = true;
                        waveCountdownLabel.text =
                            oneNumberRegex.Replace(waveCountdownLabel.text, data.NextWaveCountdown.ToString());
                    }
                    else
                    {
                        waveCountdownLabel.visible = false;
                    }
                }

                //
                if (enemiesCounterLabel != null && data.EnemiesCount != null)
                {
                    enemiesCounterLabel.text =
                        oneNumberRegex.Replace(enemiesCounterLabel.text, data.EnemiesCount.ToString());
                }
            }
        }

        public void Init(IEcsSystems systems)
        {
            var root = UnityEngine.Object.FindObjectOfType<UIDocument>().rootVisualElement;

            livesLebel = root.Query<Label>("lives").First();
            moneyLebel = root.Query<Label>("money").First();
            waveLabel = root.Query<Label>("wave").First();
            waveCountdownLabel = root.Query<Label>("waveCountdown").First();
            enemiesCounterLabel = root.Query<Label>("enemies").First();
        }
    }

}