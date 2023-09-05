using System.Runtime.CompilerServices;
using System.Text;
using td.features._common;
using td.features.eventBus;
using td.features.level.bus;
using td.features.shard;
using td.features.state;
using td.features.state.bus;
using td.utils.di;
using TMPro;
using UnityEngine;

namespace td.features.infoPanel {
    public class UI_InfoPanel : MonoBehaviour {
        public TMP_Text textField;

        private Shard_Calculator _calc;
        private Shard_Calculator Calc => _calc ??= ServiceContainer.Get<Shard_Calculator>();
        private EventBus _events;
        private EventBus Events => _events ??= ServiceContainer.Get<EventBus>();
        private State _state;
        private State State => _state ??= ServiceContainer.Get<State>();
        private InfoPanel_State _infoPanelState;
        private InfoPanel_State InfoPanelState => _infoPanelState ??= ServiceContainer.Get<InfoPanel_State>();

        private void Start() {
            Events.unique.ListenTo<Event_StateChanged>(OnStateChanged);
            Events.unique.ListenTo<Event_InfoPanel_StateChanged>(OnInfoPanelStateChanged);
            Events.unique.ListenTo<Event_LevelFinished>(OnLevelFinished);
        }

        private void OnDestroy() {
            Events.unique.RemoveListener<Event_StateChanged>(OnStateChanged);
            Events.unique.RemoveListener<Event_InfoPanel_StateChanged>(OnInfoPanelStateChanged);
            Events.unique.RemoveListener<Event_LevelFinished>(OnLevelFinished);
        }

        // ----------------------------------------------------------------

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnStateChanged(ref Event_StateChanged ev) {
            if (ev.lives && State.IsDead()) Hide();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnLevelFinished(ref Event_LevelFinished obj) => Hide();

        private void OnInfoPanelStateChanged(ref Event_InfoPanel_StateChanged ev) {
            if (ev.IsEmpty()) return;
            
            if (InfoPanelState.GetVisible()) Show();
            else Hide();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Hide() {
            if (gameObject.activeSelf)
                gameObject.SetActive(false);
        }

        private void Show() {
            var sb = new StringBuilder();

            Header(sb);

            if (InfoPanelState.HasShard()) {
                ShardInfo(sb);
            }

            if (InfoPanelState.HasEnemy()) {
                EnemyInfo(sb);
            }

            Footer(sb);

            textField.text = "";
            textField.text = sb.ToString();
            Debug.Log("InfoPanel UPDATE TEXT!!!");
            
            gameObject.SetActive(true);
        }

        private void Header(StringBuilder sb) {
            var s = InfoPanelState;
            
            var title = s.GetTitle();
            if (title?.Length > 0) sb.AppendLine($"<size=120%><b>{title}</b></size>").AppendLine();

            var price = s.GetPrice();
            var priceTitle = s.GetPriceTitle();
            if (price > 0) sb.AppendLine($"{priceTitle ?? "Price: "}: ${CommonUtils.PriceFormat(price)}");

            var time = s.GetTime();
            var timeTitle = s.GetTimeTitle();
            if (time > 0) sb.AppendLine($"{timeTitle ?? "Time: "}: ${CommonUtils.TMPTimeFormat(price)}");

            if (price > 0 || time > 0) sb.AppendLine();

            var before = s.GetBefore();
            if (before?.Length > 0) sb.AppendLine(before);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Footer(StringBuilder sb) {
            var after = InfoPanelState.GetAfter();
            if (after?.Length > 0) sb.AppendLine().AppendLine(after);
        }

        private void ShardInfo(StringBuilder sb) {
            ref var shard = ref InfoPanelState.GetShard();
            
            var quantity = ShardUtils.GetQuantity(ref shard);

            sb.AppendLine($"{quantity}: {shard}");
            sb.AppendLine();

            sb.AppendLine($"Level: {shard.level}");
            sb.AppendLine();

            sb.AppendLine($"Projectile Speed: {shard.projectileSpeed:0.00}");

            sb.AppendLine($"Fire Rate: {shard.fireRate:0.00}");

            sb.AppendLine($"Radius: {shard.radius:0.00}");

            if (Calc.HasBaseDamage(ref shard)) {
                Calc.CalculateBaseDamageParams(ref shard, out var damage, out var type);
                sb.AppendLine($"Damage: {damage:0.00}");
                sb.AppendLine($"Damage Type: {type}");
            }

            Calc.CalculateSpread(ref shard, out var spread, out var distanceFactor);
            sb.AppendLine($"Spread: {spread:0.00} / {distanceFactor:0.00}");

            // red - разрывной. удар по области
            if (Calc.HasExplosive(ref shard)) {
                Calc.CalculateExplosiveParams(ref shard, out var damage, out var diameter, out var damageFading);
                sb.AppendLine($"Explosive");
                sb.AppendLine($" - damage: {damage:0.00}");
                sb.AppendLine($" - diameter: {diameter:0.00}");
                sb.AppendLine($" - fading: {damageFading:0.00}");
                sb.AppendLine($"");
            }

            // green - отравляет мобов на время
            if (Calc.HasPoison(ref shard)) {
                Calc.CalculatePoisonParams(ref shard, out var damage, out var duration);
                sb.AppendLine($"Poison");
                sb.AppendLine($" - damage: {damage:0.00}");
                sb.AppendLine($" - duration: {duration:0.00}");
                sb.AppendLine($"");
            }

            // blue - замедляет мобов на время
            if (Calc.HasSlowing(ref shard)) {
                Calc.CalculateSlowingParams(ref shard, out var speedMultipler, out var duration);
                sb.AppendLine($"Slowing");
                sb.AppendLine($" - speed multipler: {speedMultipler:0.00}");
                sb.AppendLine($" - duration: {duration:0.00}");
                sb.AppendLine($"");
            }

            // aquamarine - молния. цепная реакция от моба к мобу
            if (Calc.HasLightning(ref shard)) {
                Calc.CalculateLightningParams(
                    ref shard,
                    out var interval,
                    out var damage,
                    out var damageReduction,
                    out var damageInterval,
                    out var chainReaction,
                    out var chainReactionRadius
                );
                sb.AppendLine($"Lightning");
                sb.AppendLine($" - duration: {interval:0.00}s");
                sb.AppendLine($" - damage: {damage:0.00}");
                sb.AppendLine($" - damage reduction: {damageReduction:0.00}");
                sb.AppendLine($" - damage interval: {damageInterval:0.00}s");
                sb.AppendLine($" - chain reaction: {chainReaction:0}");
                sb.AppendLine($" - chain reaction radius: {chainReactionRadius:0.00}");
                sb.AppendLine($"");
            }

            // violet - шок, кантузия… останавливает цель на короткое время. срабатывает с % вероятности
            if (Calc.HasShocking(ref shard)) {
                Calc.CalculateShockingParams(ref shard, out var duration, out var probability);
                sb.AppendLine($"Shocking");
                sb.AppendLine($"  - duration: {duration:0.00}");
                sb.AppendLine($"  - probability: {probability:0.00}");
                sb.AppendLine($"");
            }
        }

        private void EnemyInfo(StringBuilder sb) {
            ref var enemy = ref InfoPanelState.GetEnemy();

            //todo

            sb.AppendLine("ToDo: Enemy Info");
        }
    }
}
