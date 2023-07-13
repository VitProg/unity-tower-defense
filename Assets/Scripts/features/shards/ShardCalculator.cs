using System;
using td.features.shards.config;
using td.utils.ecs;
using UnityEngine;

namespace td.features.shards
{
    public class ShardCalculator
    {
        [Inject] private ShardsConfig config;
        
        public float GetProjectileSpeed(ref Shard shard)
        {
            var speedBase = config.speedBase;
            // var speedReduce = config.speedReduce;
            var impactOfRed = config.speedImpactOfRed;

            var quantity = ShardUtils.GetQuantity(ref shard);
            // var max = ShardUtils.GetMax(ref shard);

            // var amplifier = GetShardAmplifier(max);

            // var speed = speedBase + ((Mathf.Sqrt(amplifier) / Mathf.Log(quantity + 1f)) / speedBase) * speedMul
            // var speed =
            //     speedBase *
            //     (
            //         Mathf.Sqrt(amplifier) /
            //         Mathf.Log(quantity * speedReduce + (speedBase / 10f)) /
            //         Mathf.Log(amplifier + (speedBase / 10f) + speedReduce)
            //     );
            // var speed = speedBase * (Mathf.Sqrt(amplifier / (Mathf.Sqrt(0.333333f))) / 10f + 0.87f);
            
            //todo
            var levelModifier = Mathf.Pow(config.GetLevelCoefficient(quantity) / 5.35f, 2f);
            var speed = levelModifier * Mathf.Log10(Mathf.Pow(quantity, 2f)) + speedBase;

            if (shard.red > 0)
            {
                var redAmplifier = GetShardAmplifier(shard.red, quantity);
                speed /= Mathf.Sqrt(Mathf.Sqrt(redAmplifier) / impactOfRed) + 1f;
            }

            return speed;
        }

        public float GetShardAmplifier(int quantity)
        {
            return GetShardAmplifier(quantity, quantity);
        }

        public float GetShardAmplifier(int quantity, int allQuantity)
        {
            var o = (float)quantity;
            var a = (float)allQuantity;
            // @see https://docs.google.com/spreadsheets/d/1Vze5h6492TZL5gN8KHKPSS2ckDovCIrYgW4ylsVhmCk/edit#gid=0
            // return o / a * (Mathf.Log(a) * o + 1) * (Mathf.Pow(2, Mathf.Sqrt(o)) - 1);
            return o / a * (Mathf.Log(a) * o + 1) * (Mathf.Pow(2, Mathf.Sqrt(o))) / 2 / (Mathf.Sqrt(a / o));
        }

        public float GetShardDropAmplifier(int quantity, int allQuantity) =>
            GetShardAmplifier(quantity, allQuantity) * 6f;

        public float GetShardTrapAmplifier(int quantity, int allQuantity) =>
            GetShardAmplifier(quantity, allQuantity) / 6f;

        // @see https://docs.google.com/spreadsheets/d/1Vze5h6492TZL5gN8KHKPSS2ckDovCIrYgW4ylsVhmCk/edit#gid=676981276
        public float GetNearbyBuildingsAmplifier(int quantity) =>
            0.75f / Mathf.Exp(quantity / 1.15f - 1f) + 1f;

        public bool HasBaseDamage(ref Shard shard) => true;
        public bool HasSlowing(ref Shard shard) => shard.blue > 0;
        public bool HasExplosive(ref Shard shard) => shard.red > 0;
        public bool HasPoison(ref Shard shard) => shard.green > 0;
        public bool HasLightning(ref Shard shard) => shard.aquamarine > 0;
        public bool HasShocking(ref Shard shard) => shard.violet > 0;

        public void CalculateSlowingParams(ref Shard shard, out float power, out float duration)
        {
            var powerDivider = config.slowPowerDivider;
            var powerMin = config.slowPowerMin;
            var durationDevider = config.slowDurationDevider;
            var durationMin = config.slowDurationMin;

            var quantity = ShardUtils.GetQuantity(ref shard);

            var amplifier = GetShardAmplifier(shard.blue, quantity);

            power = Mathf.Sqrt(amplifier) / Mathf.Log(shard.blue + 1f) / powerDivider + powerMin;
            duration = durationMin + (Mathf.Sqrt(amplifier) - 1f) / durationDevider;
        }

        public void CalculateBaseDamageParams(ref Shard shard, out float damage)
        {
            var baseDamage = config.baseDamage;
            var impactOfRed = config.baseDamageImpactOfRed;
            
            var quantity = ShardUtils.GetQuantity(ref shard);
            
            // var max = ShardUtils.GetMax(ref shardPackedEntity);
            //
            // damage = GetShardAmplifier(Math.Max(max, Mathf.CeilToInt(quantity / 10f)));

            var levelModifier = 1 + Mathf.Pow(config.GetLevelCoefficient(quantity) / 2.97823f, 2f);
            damage = levelModifier * Mathf.Pow(quantity / 7.5f, 2) + baseDamage;

            //todo
            if (shard.red > 0)
            {
                var redAmplifier = GetShardDropAmplifier(shard.red, quantity);
                damage /= Mathf.Sqrt(redAmplifier * impactOfRed) + 1f;
            }

            damage *= baseDamage;
        }

        public int GetShardLevel(ref Shard shard)
        {
            var quantity = ShardUtils.GetQuantity(ref shard);
            return config.GetLevelCoefficient(quantity);
        }
        
        public int GetShardLevel(int quantity) => config.GetLevelCoefficient(quantity);

        public void CalculateExplosiveParams(
            ref Shard shard,
            out float damage,
            out float diameter,
            out float damageFading
        ) {
            var damageReducer = config.explosiveDamageReducer;
            var radiusAdd = config.explosiveRadiusAdd;
            var fadingReducer = config.explosiveFadingReducer;
            
            var quantity = ShardUtils.GetQuantity(ref shard);
            var amplifier = GetShardAmplifier(shard.red, quantity);

            damage = amplifier / damageReducer;
            diameter = Mathf.Log(amplifier) + radiusAdd;
            damageFading = 0.9f;//diameter / Mathf.Sqrt(shard.red * fadingReducer);
        }

        public void CalculatePoisonParams(ref Shard shard, out float damage, out float interval,
            out float duration)
        {
            var damageBase = config.poisonDamageBase;
            var damageReducer = config.poisonDamageReducer;
            var minInterval = config.poisonMinInterval;
            var minDuration = config.poisonMinDuration;
            
            var amplifier = GetShardAmplifier(shard.green, ShardUtils.GetQuantity(ref shard));

            damage = amplifier / Mathf.Sqrt(shard.green * 2f) / damageReducer;
            interval = Mathf.Sqrt((Mathf.Log(amplifier))) + minInterval;
            duration = Mathf.Sqrt(Mathf.Sqrt(amplifier)) + minDuration;

            duration *= damageBase;
        }

        public void CalculateLightningParams(
            ref Shard shard,
            out float duration,
            out float damage,
            out float damageReduction,
            out float damageInterval,
            out int chainReaction,
            out float chainReactionRadius)
        {
            // todo
            duration = 1.5f;
            damage = 0.5f;
            damageReduction = 1.5f;
            damageInterval = 0.25f;
            chainReaction = 3;
            chainReactionRadius = 2f;
            // todo
        }

        public void CalculateShockingParams(ref Shard shard, out float duration, out float probability)
        {
            duration = 3f;
            probability = 0.25f;
            //todo
        }

        // pink - увеличивает скорострельность
        public float GetFireRate(ref Shard shard)
        {
            var baseFireRate = config.fireRateBase;
            var pinkReduser = config.fireRatePinkReduser;
            
            var max = ShardUtils.GetMax(ref shard);
            var quantity = ShardUtils.GetQuantity(ref shard);
            var amplifier = GetShardAmplifier(max);

            // var fireRate = baseFireRate + (Mathf.Log(amplifier) * Mathf.Sqrt(max));
            var levelModifier = Mathf.Pow(config.GetLevelCoefficient(quantity) / 3.62f, 2f);
            var fireRate = levelModifier * Mathf.Log10(Mathf.Pow(quantity, 2f)) + baseFireRate;

            if (shard.pink > 0)
            {
                var pinkAmplifier = GetShardAmplifier(shard.pink, quantity);
                fireRate += Mathf.Sqrt(pinkAmplifier * pinkReduser) / Mathf.Sqrt(quantity);
            }

            if (shard.aquamarine > 0)
            {
                fireRate /= 1 + Mathf.Sqrt(shard.aquamarine);
            }

            return fireRate;
        }

        // yellow - увеличивает радиус
        public float GetTowerRadius(ref Shard shard)
        {
            var baseRadius = config.radiusBase;
            var impactOfYellow = config.radiusImpactOfYellow;

            var quantity = ShardUtils.GetQuantity(ref shard);
            var levelCooficient = config.GetLevelCoefficient(quantity);
            var levelModifier = Mathf.Pow(levelCooficient / 10f, 2f);

            var radius = levelModifier * Mathf.Log10(Mathf.Pow(quantity + 1, 2f)) + baseRadius;

            if (shard.yellow > 0)
            {
                var yellowAmplifier = GetShardAmplifier(shard.yellow, quantity);

                radius += Mathf.Sqrt(yellowAmplifier * impactOfYellow) / Mathf.Sqrt(quantity);
            }

            radius = Mathf.Max(baseRadius, radius);

            return radius;
        }

        public int CalculateCost(ref Shard shard, int singleCost)
        {
            // ToDo добавить в рассчет цены каждого слияния
            var cost = singleCost;
            var quantity = ShardUtils.GetQuantity(ref shard);

            // todo
            var combineCost = CalculateCombineCost(ref shard, ref shard, singleCost);

            return (int)(cost * Mathf.Pow(quantity, 2) + combineCost);
        }

        public int CalculateCombineCost(ref Shard targetShard, ref Shard sourceShard, int baseCombineCost)
        {
            // todo
            var quantityTarget = ShardUtils.GetQuantity(ref targetShard) - 1;
            var quantitySource = ShardUtils.GetQuantity(ref sourceShard) - 1;

            return (int)(baseCombineCost * Mathf.Sqrt(quantityTarget * quantitySource));
        }
    }
}