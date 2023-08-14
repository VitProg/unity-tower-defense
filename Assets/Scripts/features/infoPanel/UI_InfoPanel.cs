using System.Text;
using Leopotam.EcsProto.QoL;
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

            var s = State.Ex<InfoPanel_StateExtension>();
            
            if (!s.GetVisible())
            {
                // Debug.Log("infoPanel Visible false");
                Hide();
                return;
            }
            
            if (ev.shard)
            {
                ShowShardInfo();
                return;
            }

            if (ev.enemy)
            {
                ShowEnemyInfo();
                return;
            }

            // Hide(); 
        }

        private void ShowEnemyInfo()
        {
            // TODO
        }

        private void Hide()
        {
            // Debug.Log("InfoPanel Hide");
            gameObject.SetActive(false);
        }

        private StringBuilder GetHead()
        {
            var s = State.Ex<InfoPanel_StateExtension>();
            
            var sb = new StringBuilder();
            
            if (s.GetTitle()?.Length > 0) sb.AppendLine($"<size=120%><b>{s.GetTitle()}</b></size>\n");
            if (s.GetCost() > 0) sb.AppendLine($"{s.GetCostTitle() ?? "Cost: "}: ${CommonUtils.CostFormat(s.GetCost())}\n");
            if (s.GetBefore()?.Length > 0) sb.AppendLine($"{s.GetBefore()}\n");

            return sb;
        }

        private void ShowShardInfo()
        {
            var s = State.Ex<InfoPanel_StateExtension>();
            
            if (!s.HasShard()) return;
            
            ref var shard = ref s.GetShard();

            // Debug.Log($"InfoPanel ShowShardInfo {shard}");
            gameObject.SetActive(true);

            var sb = GetHead();

            var quantity = ShardUtils.GetQuantity(ref shard);

            sb.AppendLine($"{quantity}: {shard}");
            sb.AppendLine($"");

            sb.AppendLine($"Level: {Calc.GetShardLevel(ref shard)}");
            sb.AppendLine($"");
            
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

            sb.Append(GetFooter());

            textField.text = sb.ToString();
        }

        private StringBuilder GetFooter()
        {
            var s = State.Ex<InfoPanel_StateExtension>();
            var sb = new StringBuilder();
            if (s.GetAfter()?.Length > 0) sb.AppendLine($"\n{s.GetAfter()}");
            return sb;
        }
    }
}