using System.Text;
using td.features._common;
using td.features.eventBus;
using td.features.gameStatus.bus;
using td.features.level.bus;
using td.features.shard;
using td.features.state;
using td.utils.di;
using TMPro;
using UnityEngine;

namespace td.features.infoPanel
{
    public class UI_InfoPanel : MonoBehaviour
    {
        public TMP_Text textField;

        private Shard_Calculator Calc =>  ServiceContainer.Get<Shard_Calculator>();
        private EventBus Events =>  ServiceContainer.Get<EventBus>();
        private State State =>  ServiceContainer.Get<State>();

        private void Start()
        {
            Events.unique.ListenTo<Event_InfoPanel_StateChanged>(OnStateChanged);
            Events.unique.ListenTo<Event_LevelFinished>(OnLevelFinished);
            Events.unique.ListenTo<Event_YouDied>(OnYouDied);
        }

        private void OnYouDied(ref Event_YouDied obj)
        {
            Hide();
        }

        private void OnLevelFinished(ref Event_LevelFinished obj)
        {
            Hide();
        }

        private void OnDestroy()
        {
            Events.unique.RemoveListener<Event_InfoPanel_StateChanged>(OnStateChanged);
            Events.unique.RemoveListener<Event_LevelFinished>(OnLevelFinished);
            Events.unique.RemoveListener<Event_YouDied>(OnYouDied);
        }
        
        private void OnStateChanged(ref Event_InfoPanel_StateChanged ev)
        {
            if (ev.IsEmpty()) return;

            var state = State.Ex<InfoPanel_StateExtension>();

            if (state.GetVisible()) Show(ref state);
            else Hide();
        }

        private void Hide()
        {
            // Debug.Log("InfoPanel Hide");
            gameObject.SetActive(false);
        }

        private void Show(ref InfoPanel_StateExtension state)
        {
            var sb = new StringBuilder();

            Header(ref sb, ref state);

            if (state.HasShard())
            {
                ShardInfo(ref sb, ref state);
            }
            
            if (state.HasEnemy())
            {
                EnemyInfo(ref sb, ref state);
            }
            
            Footer(ref sb, ref state);
            
            textField.text = sb.ToString();
                
            gameObject.SetActive(true);
        }

        private void Header(ref StringBuilder sb, ref InfoPanel_StateExtension state)
        {
            var title = state.GetTitle();
            if (title?.Length > 0) sb.AppendLine($"<size=120%><b>{title}</b></size>").AppendLine();

            var price = state.GetPrice();
            var priceTitle = state.GetPriceTitle();
            if (price > 0) sb.AppendLine($"{priceTitle ?? "Price: "}: ${CommonUtils.PriceFormat(price)}");
            
            var time = state.GetTime();
            var timeTitle = state.GetTimeTitle();
            if (time > 0) sb.AppendLine($"{timeTitle ?? "Time: "}: ${CommonUtils.TMPTimeFormat(price)}");

            if (price > 0 || time > 0) sb.AppendLine();

            var before = state.GetBefore();
            if (before?.Length > 0) sb.AppendLine(before);
        }


        private void Footer(ref StringBuilder sb, ref InfoPanel_StateExtension state)
        {
            var after = state.GetAfter();
            if (after?.Length > 0) sb.AppendLine().AppendLine(after);
        }

        private void ShardInfo(ref StringBuilder sb, ref InfoPanel_StateExtension state)
        {
            ref var shard = ref state.GetShard();

            var quantity = ShardUtils.GetQuantity(ref shard);

            sb.AppendLine($"{quantity}: {shard}");
            sb.AppendLine();

            sb.AppendLine($"Level: {Calc.GetShardLevel(ref shard)}");
            sb.AppendLine();
            
            var speed = Calc.GetProjectileSpeed(ref shard);
            sb.AppendLine($"Projectile Speed: {speed:0.00}");

            var fireRate = Calc.GetFireRate(ref shard);
            sb.AppendLine($"Fire Rate: {fireRate:0.00}");

            var radius = Calc.GetTowerRadius(ref shard);
            sb.AppendLine($"Radius: {radius:0.00}");

            if (Calc.HasBaseDamage(ref shard))
            {
                Calc.CalculateBaseDamageParams(ref shard, out var damage, out var type);
                sb.AppendLine($"Damage: {damage:0.00}");
                sb.AppendLine($"Damage Type: {type}");
            }
            
            Calc.CalculateSpread(ref shard, out var spread, out var distanceFactor);
            sb.AppendLine($"Spread: {spread:0.00} / {distanceFactor:0.00}");

            // red - разрывной. удар по области
            if (Calc.HasExplosive(ref shard))
            {
                Calc.CalculateExplosiveParams(ref shard, out var damage, out var diameter, out var damageFading);
                sb.AppendLine($"Explosive");
                sb.AppendLine($" - damage: {damage:0.00}");
                sb.AppendLine($" - diameter: {diameter:0.00}");
                sb.AppendLine($" - fading: {damageFading:0.00}");
                sb.AppendLine($"");
            }

            // green - отравляет мобов на время
            if (Calc.HasPoison(ref shard))
            {
                Calc.CalculatePoisonParams(ref shard, out var damage, out var duration);
                sb.AppendLine($"Poison");
                sb.AppendLine($" - damage: {damage:0.00}");
                sb.AppendLine($" - duration: {duration:0.00}");
                sb.AppendLine($"");
            }

            // blue - замедляет мобов на время
            if (Calc.HasSlowing(ref shard))
            {
                Calc.CalculateSlowingParams(ref shard, out var speedMultipler, out var duration);
                sb.AppendLine($"Slowing");
                sb.AppendLine($" - speed multipler: {speedMultipler:0.00}");
                sb.AppendLine($" - duration: {duration:0.00}");
                sb.AppendLine($"");
            }

            // aquamarine - молния. цепная реакция от моба к мобу
            if (Calc.HasLightning(ref shard))
            {
                Calc.CalculateLightningParams(ref shard, out var interval, out var damage, out var damageReduction, out var damageInterval, out var chainReaction, out var chainReactionRadius);
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
            if (Calc.HasShocking(ref shard))
            {
                Calc.CalculateShockingParams(ref shard, out var duration, out var probability);
                sb.AppendLine($"Shocking");
                sb.AppendLine($"  - duration: {duration:0.00}");
                sb.AppendLine($"  - probability: {probability:0.00}");
                sb.AppendLine($"");
            }
        }

        private void EnemyInfo(ref StringBuilder sb, ref InfoPanel_StateExtension state)
        {
            ref var enemy = ref state.GetEnemy();

            //todo

            sb.AppendLine("ToDo: Enemy Info");
        }
    }
}