using System;
using System.Collections.Generic;
using System.Text;
using Leopotam.EcsLite;
using td.features._common;
using td.features.gameStatus.bus;
using td.features.level.bus;
using td.features.shard;
using td.features.state;
using td.utils.di;
using TMPro;
using UnityEngine;

namespace td.features.infoPanel
{
    public class UI_InfoPanel : MonoInjectable
    {
        public TMP_Text textField;

        private readonly EcsInject<ShardCalculator> calc;
        private readonly EcsInject<IEventBus> events;
        private readonly EcsInject<IState> state;
        
        private readonly List<IDisposable> eventDisposers = new(3);

        private void Start()
        {
            eventDisposers.Add(events.Value.Unique.SubscribeTo<Event_StateChanged>(OnStateChanged));
            eventDisposers.Add(events.Value.Unique.SubscribeTo<Event_LevelFinished>(delegate { Hide(); }));
            eventDisposers.Add(events.Value.Unique.SubscribeTo<Event_YouDied>(delegate { Hide(); }));
        }

        private void OnDestroy()
        {
            foreach (var disposer in eventDisposers)
            {
                disposer?.Dispose();
            }

            eventDisposers.Clear();
        }
        
        private void OnStateChanged(ref Event_StateChanged item)
        {
            if (item.infoPanel.IsEmpty) return;

            if (!state.Value.InfoPanel.Visible)
            {
                // Debug.Log("infoPanel Visible false");
                Hide();
                return;
            }
            
            if (item.infoPanel.shard.HasValue)
            {
                ShowShardInfo();
                return;
            }

            if (item.infoPanel.enemy.HasValue)
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
            var s = state.Value.InfoPanel;
            
            var sb = new StringBuilder();
            
            if (s.Title?.Length > 0) sb.AppendLine($"<size=120%><b>{s.Title}</b></size>\n");
            if (s.Cost > 0) sb.AppendLine($"{s.CostTitle ?? "Cost: "}: ${CommonUtils.CostFormat(s.Cost)}\n");
            if (s.Before?.Length > 0) sb.AppendLine($"{s.Before}\n");

            return sb;
        }

        private void ShowShardInfo()
        {
            var s = state.Value.InfoPanel;
            
            if (!s.Shard.HasValue) return;
            
            var shard = s.Shard!.Value;

            // Debug.Log($"InfoPanel ShowShardInfo {shard}");
            gameObject.SetActive(true);

            var sb = GetHead();

            var quantity = ShardUtils.GetQuantity(ref shard);

            sb.AppendLine($"{quantity}: {shard}");
            sb.AppendLine($"");

            sb.AppendLine($"Level: {calc.Value.GetShardLevel(ref shard)}");
            sb.AppendLine($"");
            
            var speed = calc.Value.GetProjectileSpeed(ref shard);
            sb.AppendLine($"Projectile Speed: {speed:0.00}");

            var fireRate = calc.Value.GetFireRate(ref shard);
            sb.AppendLine($"Fire Rate: {fireRate:0.00}");

            var radius = calc.Value.GetTowerRadius(ref shard);
            sb.AppendLine($"Radius: {radius:0.00}");

            if (calc.Value.HasBaseDamage(ref shard))
            {
                calc.Value.CalculateBaseDamageParams(ref shard, out var damage);
                sb.AppendLine($"Damage: {damage:0.00}");
            }
            
            calc.Value.CalculateSpread(ref shard, out var spread, out var distanceFactor);
            sb.AppendLine($"Spread: {spread:0.00} / {distanceFactor:0.00}");

            // red - разрывной. удар по области
            if (calc.Value.HasExplosive(ref shard))
            {
                calc.Value.CalculateExplosiveParams(ref shard, out var damage, out var diameter, out var damageFading);
                sb.AppendLine($"Explosive");
                sb.AppendLine($" - damage: {damage:0.00}");
                sb.AppendLine($" - diameter: {diameter:0.00}");
                sb.AppendLine($" - fading: {damageFading:0.00}");
                sb.AppendLine($"");
            }

            // green - отравляет мобов на время
            if (calc.Value.HasPoison(ref shard))
            {
                calc.Value.CalculatePoisonParams(ref shard, out var damageInterval, out var interval, out var duration);
                sb.AppendLine($"Poison");
                sb.AppendLine($" - damage interval: {damageInterval:0.00}");
                sb.AppendLine($" - interval: {interval:0.00}");
                sb.AppendLine($" - duration: {duration:0.00}");
                sb.AppendLine($"");
            }

            // blue - замедляет мобов на время
            if (calc.Value.HasSlowing(ref shard))
            {
                calc.Value.CalculateSlowingParams(ref shard, out var speedMultipler, out var duration);
                sb.AppendLine($"Slowing");
                sb.AppendLine($" - speed multipler: {speedMultipler:0.00}");
                sb.AppendLine($" - duration: {duration:0.00}");
                sb.AppendLine($"");
            }

            // aquamarine - молния. цепная реакция от моба к мобу
            if (calc.Value.HasLightning(ref shard))
            {
                calc.Value.CalculateLightningParams(ref shard, out var interval, out var damage, out var damageReduction, out var damageInterval, out var chainReaction, out var chainReactionRadius);
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
            if (calc.Value.HasShocking(ref shard))
            {
                calc.Value.CalculateShockingParams(ref shard, out var duration, out var probability);
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
            var s = state.Value.InfoPanel;
            var sb = new StringBuilder();
            if (s.After?.Length > 0) sb.AppendLine($"\n{s.After}");
            return sb;
        }
    }
}