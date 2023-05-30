using System.Text;
using td.utils.ecs;
using TMPro;
using UnityEngine;

namespace td.features.shards.mb
{
    public class ShardInfoPanel : MonoBehaviour
    {
        public TMP_Text textField;

        private ShardCalculator shardCalculator;

        public void ShowInfo(ref Shard shard)
        {
            shardCalculator ??= DI.GetCustom<ShardCalculator>();

            var sb = new StringBuilder();

            var quantity = ShardUtils.GetQuantity(ref shard);

            sb.Append($"{quantity}: {shard.red}-{shard.green}-{shard.blue}-{shard.aquamarine}-{shard.yellow}-{shard.orange}-{shard.pink}-{shard.violet}\n");
            sb.Append($"\n");

            sb.Append($"Level: {shardCalculator!.GetShardLevel(ref shard)}\n");
            sb.Append($"\n");
            
            var speed = shardCalculator!.GetProjectileSpeed(ref shard);
            sb.Append($"Projectile Speed: {speed:0.00}\n");

            var fireRate = shardCalculator.GetFireRate(ref shard);
            sb.Append($"Fire Rate: {fireRate:0.00}\n");

            var radius = shardCalculator.GetTowerRadius(ref shard);
            sb.Append($"Radius: {radius:0.00}\n");
           
            sb.Append($"\n");

            if (shardCalculator.HasBaseDamage(ref shard))
            {
                shardCalculator.CalculateBaseDamageParams(ref shard, out var damage);
                sb.Append($"Damage: {damage:0.00}\n");
                sb.Append($"\n");
            }

            // red - разрывной. удар по области
            if (shardCalculator.HasExplosive(ref shard))
            {
                shardCalculator.CalculateExplosiveParams(ref shard, out var damage, out var diameter, out var damageFading);
                sb.Append($"Explosive\n");
                sb.Append($" - damage: {damage:0.00}\n");
                sb.Append($" - diameter: {diameter:0.00}\n");
                sb.Append($" - fading: {damageFading:0.00}\n");
                sb.Append($"\n");
            }

            // green - отравляет мобов на время
            if (shardCalculator.HasPoison(ref shard))
            {
                shardCalculator.CalculatePoisonParams(ref shard, out var damageInterval, out var interval, out var duration);
                sb.Append($"Poison\n");
                sb.Append($" - damage interval: {damageInterval:0.00}\n");
                sb.Append($" - interval: {interval:0.00}\n");
                sb.Append($" - duration: {duration:0.00}\n");
                sb.Append($"\n");
            }

            // blue - замедляет мобов на время
            if (shardCalculator.HasSlowing(ref shard))
            {
                shardCalculator.CalculateSlowingParams(ref shard, out var speedMultipler, out var duration);
                sb.Append($"Slowing\n");
                sb.Append($" - speed multipler: {speedMultipler:0.00}\n");
                sb.Append($" - duration: {duration:0.00}\n");
                sb.Append($"\n");
            }

            // aquamarine - молния. цепная реакция от моба к мобу
            if (shardCalculator.HasLightning(ref shard))
            {
                shardCalculator.CalculateLightningParams(ref shard, out var damage, out var damageReduction, out var chainReaction);
                sb.Append($"Lightning\n");
                sb.Append($" - damage: {damage:0.00}\n");
                sb.Append($" - damage reduction: {damageReduction:0.00}\n");
                sb.Append($" - chain reaction: {chainReaction:0.00}\n");
                sb.Append($"\n");
            }

            // violet - шок, кантузия… останавливает цель на короткое время. срабатывает с % вероятности
            if (shardCalculator.HasShocking(ref shard))
            {
                shardCalculator.CalculateShockingParams(ref shard, out var duration, out var probability);
                sb.Append($"Shocking\n");
                sb.Append($"  - duration: {duration:0.00}\n");
                sb.Append($"  - probability: {probability:0.00}\n");
                sb.Append($"\n");
            }

            textField.text = sb.ToString();
        }
    }
}