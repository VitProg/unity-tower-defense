// using System.Text.RegularExpressions;
// using JetBrains.Annotations;
// using Leopotam.EcsLite;
// using Leopotam.EcsLite.Di;
// using UnityEngine.UIElements;
//
// namespace td.features.ui
// {
//     public class UpdateUIToolkitSystem : IEcsRunSystem, IEcsInitSystem
//     {
//         private readonly EcsFilterInject<Inc<UpdateUIOuterCommand>> entities = Constants.Worlds.Outer;
//
//         [CanBeNull] private Label livesLebel;
//         [CanBeNull] private Label moneyLebel;
//         [CanBeNull] private Label waveLabel;
//         [CanBeNull] private Label waveCountdownLabel;
//         [CanBeNull] private Label enemiesCounterLabel;
//         
//         private Button bayTowerButton;
//
//         private readonly Regex oneNumberRegex = new(@"[\d#.-]+");
//         private readonly Regex waveRegex = new(@"(\d+|#+)/(\d+|#+)");
//
//         public void Run(IEcsSystems systems)
//         {
//             foreach (var entity in entities.Value)
//             {
//                 var data = entities.Pools.Inc1.Get(entity);
//
//                 if (livesLebel != null && data.lives != null)
//                 {
//                     livesLebel.text = oneNumberRegex.Replace(livesLebel.text, ((int)data.lives).ToString());
//                 }
//
//                 //
//                 if (moneyLebel != null && data.money != null)
//                 {
//                     moneyLebel.text = oneNumberRegex.Replace(moneyLebel.text, data.money.ToString());
//                 }
//
//                 //
//                 if (waveLabel != null && data is { waveNumber: not null, waveCount: not null })
//                 {
//                     if (data.waveNumber != 0 && data.waveCount != 0)
//                     {
//                         waveLabel.visible = true;
//                         waveLabel.text = waveRegex.Replace(waveLabel.text, $@"{data.waveNumber}/{data.waveCount}");
//                     }
//                     else
//                     {
//                         waveLabel.visible = false;
//                     }
//                 }
//
//                 //
//                 if (waveCountdownLabel != null && data.nextWaveCountdown != null)
//                 {
//                     if (data.nextWaveCountdown > 0)
//                     {
//                         waveCountdownLabel.visible = true;
//                         waveCountdownLabel.text =
//                             oneNumberRegex.Replace(waveCountdownLabel.text, data.nextWaveCountdown.ToString());
//                     }
//                     else
//                     {
//                         waveCountdownLabel.visible = false;
//                     }
//                 }
//
//                 //
//                 if (enemiesCounterLabel != null && data.enemiesCount != null)
//                 {
//                     enemiesCounterLabel.text =
//                         oneNumberRegex.Replace(enemiesCounterLabel.text, data.enemiesCount.ToString());
//                 }
//             }
//         }
//
//         public void Init(IEcsSystems systems)
//         {
//             var root = UnityEngine.Object.FindObjectOfType<UIDocument>().rootVisualElement;
//
//             livesLebel = root.Query<Label>("lives").First();
//             moneyLebel = root.Query<Label>("money").First();
//             waveLabel = root.Query<Label>("wave").First();
//             waveCountdownLabel = root.Query<Label>("waveCountdown").First();
//             enemiesCounterLabel = root.Query<Label>("enemies").First();
//         }
//     }
//
// }