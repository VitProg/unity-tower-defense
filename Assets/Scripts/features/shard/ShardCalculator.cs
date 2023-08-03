using System;
using System.Runtime.CompilerServices;
using Leopotam.EcsLite;
using td.features._common;
using td.features.level;
using td.features.shard.components;
using UnityEngine;
using UnityEngine.U2D;

namespace td.features.shard
{
    public class ShardCalculator
    {
        private readonly EcsInject<ShardsConfig> config;
        private readonly EcsInject<LevelMap> levelMap;
        
        public float GetProjectileSpeed(ref Shard shard)
        {
            return 5;
            var speedBase = config.Value.speedBase;
            // var speedReduce = config.speedReduce;
            var impactOfRed = config.Value.speedImpactOfRed;

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
            var levelModifier = Mathf.Pow(config.Value.GetLevelCoefficient(quantity) / 5.35f, 2f);
            var speed = levelModifier * Mathf.Log10(Mathf.Pow(quantity, 2f)) + speedBase;

            if (shard.red > 0)
            {
                var redAmplifier = GetShardAmplifier(shard.red, quantity);
                speed /= Mathf.Sqrt(Mathf.Sqrt(redAmplifier) / impactOfRed) + 1f;
            }

            return speed;
        }

        public float GetShardAmplifier(uint quantity)
        {
            return GetShardAmplifier(quantity, quantity);
        }

        public float GetShardAmplifier(uint quantity, uint allQuantity)
        {
            var o = (float)quantity;
            var a = (float)allQuantity;
            // @see https://docs.google.com/spreadsheets/d/1Vze5h6492TZL5gN8KHKPSS2ckDovCIrYgW4ylsVhmCk/edit#gid=0
            // return o / a * (Mathf.Log(a) * o + 1) * (Mathf.Pow(2, Mathf.Sqrt(o)) - 1);
            return o / a * (Mathf.Log(a) * o + 1) * (Mathf.Pow(2, Mathf.Sqrt(o))) / 2 / (Mathf.Sqrt(a / o));
        }

        public float GetShardDropAmplifier(uint quantity, uint allQuantity) =>
            GetShardAmplifier(quantity, allQuantity) * 6f;

        public float GetShardTrapAmplifier(uint quantity, uint allQuantity) =>
            GetShardAmplifier(quantity, allQuantity) / 6f;

        // @see https://docs.google.com/spreadsheets/d/1Vze5h6492TZL5gN8KHKPSS2ckDovCIrYgW4ylsVhmCk/edit#gid=676981276
        public float GetNearbyBuildingsAmplifier(uint quantity) =>
            0.75f / Mathf.Exp(quantity / 1.15f - 1f) + 1f;

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public bool HasBaseDamage(ref Shard shard) => true;
        
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public bool HasSlowing(ref Shard shard) => shard.blue > 0;
        
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public bool HasExplosive(ref Shard shard) => shard.red > 0;
        
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public bool HasPoison(ref Shard shard) => shard.green > 0;
        
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public bool HasLightning(ref Shard shard) => shard.aquamarine > 0;
        
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public bool HasShocking(ref Shard shard) => shard.violet > 0;

        public void CalculateSlowingParams(ref Shard shard, out float power, out float duration)
        {
            power = 1.5f;
            duration = 3f;
            return;
            //todo
            //
            // var powerDivider = config.Value.slowPowerDivider;
            // var powerMin = config.Value.slowPowerMin;
            // var durationDevider = config.Value.slowDurationDevider;
            // var durationMin = config.Value.slowDurationMin;
            //
            // var quantity = ShardUtils.GetQuantity(ref shard);
            //
            // var amplifier = GetShardAmplifier(shard.blue, quantity);
            //
            // power = Mathf.Sqrt(amplifier) / Mathf.Log(shard.blue + 1f) / powerDivider + powerMin;
            // duration = durationMin + (Mathf.Sqrt(amplifier) - 1f) / durationDevider;
        }

        public void CalculateBaseDamageParams(ref Shard shard, out float damage, out DamageType type)
        {
            var baseDamage = config.Value.baseDamage;
            var impactOfRed = config.Value.baseDamageImpactOfRed;
            
            var quantity = ShardUtils.GetQuantity(ref shard);
            
            // var max = ShardUtils.GetMax(ref shardPackedEntity);
            //
            // damage = GetShardAmplifier(Math.Max(max, Mathf.CeilToInt(quantity / 10f)));

            var levelModifier = 1 + Mathf.Pow(config.Value.GetLevelCoefficient(quantity) / 2.97823f, 2f);
            damage = levelModifier * Mathf.Pow(quantity / 7.5f, 2) + baseDamage;

            //todo
            if (shard.red > 0)
            {
                var redAmplifier = GetShardDropAmplifier(shard.red, quantity);
                damage /= Mathf.Sqrt(redAmplifier * impactOfRed) + 1f;
            }

            var pRed = shard.red / (float)quantity;
            var pGreen = shard.green / (float)quantity;
            var pBlue = shard.blue / (float)quantity;
            var pYellow = shard.yellow / (float)quantity;
            var pOrange = shard.orange / (float)quantity;
            var pPink = shard.pink / (float)quantity;
            var pViolet = shard.violet / (float)quantity;
            var pAquamarine = shard.aquamarine / (float)quantity;

            if (pGreen > 0.5f) type = DamageType.Poison;
            else if (pBlue > 0.5f) type = DamageType.Cold;
            else if (pRed > 0.5f) type = DamageType.Fire;
            else if (pViolet > 0.5f) type = DamageType.Electro; //?
            else if (pAquamarine > 0.5f) type = DamageType.Electro; //?
            else type = DamageType.Casual;
            
            damage *= baseDamage;
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public int GetShardLevel(ref Shard shard)
        {
            var quantity = ShardUtils.GetQuantity(ref shard);
            return config.Value.GetLevelCoefficient(quantity);
        }
        
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public int GetShardLevel(uint quantity) => config.Value.GetLevelCoefficient(quantity);
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        private float GetShardLogSqrLevel(uint quantity) => LogSqrLevel[GetShardLevel(quantity)];

        private static readonly float[] LogSqrLevel = new float[15] {
            0f,
            0.6020599913f,
            0.9542425094f,
            1.204119983f,
            1.397940009f,
            1.556302501f,
            1.69019608f,
            1.806179974f,
            1.908485019f,
            2f,
            2.08278537f,
            2.158362492f,
            2.227886705f,
            2.292256071f,
            2.352182518f,
        };
        
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public int GetQuantityForLevel(uint quantity) => config.Value.triangularPyramids[Math.Clamp(quantity - 1, 0, config.Value.triangularPyramids.Length - 1)];

        public void CalculateExplosiveParams(
            ref Shard shard,
            out float damage,
            out float diameter,
            out float damageFading
        ) {
            var damageReducer = config.Value.explosiveDamageReducer;
            var radiusAdd = config.Value.explosiveRadiusAdd;
            var fadingReducer = config.Value.explosiveFadingReducer;
            
            var quantity = ShardUtils.GetQuantity(ref shard);
            var amplifier = GetShardAmplifier(shard.red, quantity);

            damage = amplifier / damageReducer;
            diameter = Mathf.Log(Mathf.Sqrt(amplifier / 10f)) + radiusAdd;
            damageFading = 0.9f;//diameter / Mathf.Sqrt(shard.red * fadingReducer);
        }

        public void CalculatePoisonParams(
            ref Shard shard,
            out float damage,
            out float duration)
        {
            var damageBase = config.Value.poisonDamageBase;
            var damageReducer = config.Value.poisonDamageReducer;
            // var minInterval = config.Value.poisonMinInterval;
            var minDuration = config.Value.poisonMinDuration;
            
            var amplifier = GetShardAmplifier(shard.green, ShardUtils.GetQuantity(ref shard));

            damage = amplifier / Mathf.Sqrt(shard.green * 2f) / damageReducer;
            // interval = Mathf.Sqrt((Mathf.Log(amplifier))) + minInterval;
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
            var baseFireRate = config.Value.fireRateBase;
            var pinkReduser = config.Value.fireRatePinkReduser;
            
            var max = ShardUtils.GetMax(ref shard);
            var quantity = ShardUtils.GetQuantity(ref shard);
            var amplifier = GetShardAmplifier(max);

            // var fireRate = baseFireRate + (Mathf.Log(amplifier) * Mathf.Sqrt(max));
            var levelModifier = Mathf.Pow(config.Value.GetLevelCoefficient(quantity) / 3.62f, 2f);
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
            var baseRadius = config.Value.radiusBase;
            var impactOfYellow = config.Value.radiusImpactOfYellow;

            var quantity = ShardUtils.GetQuantity(ref shard);
            var levelCooficient = config.Value.GetLevelCoefficient(quantity);
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

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public uint GetBaseCostByType(ShardTypes type)
        {
            var costs = levelMap.Value.LevelConfig?.shardsCost;
            return type switch
            {
                ShardTypes.Red => costs?.red ?? 10,
                ShardTypes.Green => costs?.green ?? 10,
                ShardTypes.Blue => costs?.blue ?? 10,
                ShardTypes.Yellow => costs?.yellow ?? 10,
                ShardTypes.Orange => costs?.orange ?? 10,
                ShardTypes.Pink => costs?.pink ?? 10,
                ShardTypes.Violet => costs?.violet ?? 10,
                ShardTypes.Aquamarine => costs?.aquamarine ?? 10,
                _ => 99999
            };
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public uint CalculateCostByType(ShardTypes type, uint typeQuantity)
        {
            var baseCost = GetBaseCostByType(type);
            var level = GetShardLevel(typeQuantity);
            return Math.Max(1, (uint)(typeQuantity * baseCost * level * level));
        }

        public uint CalculateCost(ref Shard shard)
        {
            uint cost = 0;
            cost += CalculateCostByType(ShardTypes.Red, shard.red);
            cost += CalculateCostByType(ShardTypes.Green, shard.green);
            cost += CalculateCostByType(ShardTypes.Blue, shard.blue);
            cost += CalculateCostByType(ShardTypes.Yellow, shard.yellow);
            cost += CalculateCostByType(ShardTypes.Orange, shard.orange);
            cost += CalculateCostByType(ShardTypes.Pink, shard.pink);
            cost += CalculateCostByType(ShardTypes.Violet, shard.violet);
            cost += CalculateCostByType(ShardTypes.Aquamarine, shard.aquamarine);
            return Math.Max(1, cost);
        }
        
        // todo cache into shard
        public uint CalculateInsertCost(ref Shard shard)
        {
            //todo
            uint cost = 0;
            cost += (uint)(CalculateCostByType(ShardTypes.Red, shard.red) / 6 * Mathf.CeilToInt(GetShardLogSqrLevel(shard.red) + 1));
            cost += (uint)(CalculateCostByType(ShardTypes.Green, shard.green) / 6 * Mathf.CeilToInt(GetShardLogSqrLevel(shard.green) + 1));
            cost += (uint)(CalculateCostByType(ShardTypes.Blue, shard.blue) / 6 * Mathf.CeilToInt(GetShardLogSqrLevel(shard.blue) + 1));
            cost += (uint)(CalculateCostByType(ShardTypes.Yellow, shard.yellow) / 6 * Mathf.CeilToInt(GetShardLogSqrLevel(shard.yellow) + 1));
            cost += (uint)(CalculateCostByType(ShardTypes.Orange, shard.orange) / 6 * Mathf.CeilToInt(GetShardLogSqrLevel(shard.orange) + 1));
            cost += (uint)(CalculateCostByType(ShardTypes.Pink, shard.pink) / 6 * Mathf.CeilToInt(GetShardLogSqrLevel(shard.pink) + 1));
            cost += (uint)(CalculateCostByType(ShardTypes.Violet, shard.violet) / 6 * Mathf.CeilToInt(GetShardLogSqrLevel(shard.violet) + 1));
            cost += (uint)(CalculateCostByType(ShardTypes.Aquamarine, shard.aquamarine) / 6 * Mathf.CeilToInt(GetShardLogSqrLevel(shard.aquamarine) + 1));
            return Math.Max(1, cost);
        }
        
        public uint CalculateCombineCost(ref Shard shard)
        {
            uint cost = 0;
            cost += (uint)(CalculateCostByType(ShardTypes.Red, shard.red) / 4 * Mathf.CeilToInt(GetShardLogSqrLevel(shard.red) + 1));
            cost += (uint)(CalculateCostByType(ShardTypes.Green, shard.green) / 4 * Mathf.CeilToInt(GetShardLogSqrLevel(shard.green) + 1));
            cost += (uint)(CalculateCostByType(ShardTypes.Blue, shard.blue) / 4 * Mathf.CeilToInt(GetShardLogSqrLevel(shard.blue) + 1));
            cost += (uint)(CalculateCostByType(ShardTypes.Yellow, shard.yellow) / 4 * Mathf.CeilToInt(GetShardLogSqrLevel(shard.yellow) + 1));
            cost += (uint)(CalculateCostByType(ShardTypes.Orange, shard.orange) / 4 * Mathf.CeilToInt(GetShardLogSqrLevel(shard.orange) + 1));
            cost += (uint)(CalculateCostByType(ShardTypes.Pink, shard.pink) / 4 * Mathf.CeilToInt(GetShardLogSqrLevel(shard.pink) + 1));
            cost += (uint)(CalculateCostByType(ShardTypes.Violet, shard.violet) / 4 * Mathf.CeilToInt(GetShardLogSqrLevel(shard.violet) + 1));
            cost += (uint)(CalculateCostByType(ShardTypes.Aquamarine, shard.aquamarine) / 4 * Mathf.CeilToInt(GetShardLogSqrLevel(shard.aquamarine) + 1));

            return Math.Max(2, cost);
        }
        
        public uint CalculateRemoveCost(ref Shard shard) => Math.Max(1, CalculateInsertCost(ref shard) / 2);

        public uint CalculateDropCost(ref Shard shard) => Math.Max(6, CalculateInsertCost(ref shard) + CalculateCombineCost(ref shard, ref shard) / 2);

        // todo cache part of cost into shard and use this
        public uint CalculateCombineCost(ref Shard targetShard, ref Shard sourceShard)
        {
            uint cost = 0;
            if (targetShard.costCombine > 0)
                cost += targetShard.costCombine;
            else
                cost += CalculateCombineCost(ref targetShard);
            
            if (sourceShard.costCombine > 0)
                cost += sourceShard.costCombine;
            else
                cost += CalculateCombineCost(ref sourceShard);

            return Math.Max(2, cost);
        }

        public uint CalculateCombinerIntoTowerCost(ref Shard targetShardInTower, ref Shard sourceShard)
        {
            var combineCost = CalculateCombineCost(ref targetShardInTower, ref sourceShard);
            var removeCost = targetShardInTower.costRemove > 0 ? targetShardInTower.costRemove : CalculateRemoveCost(ref targetShardInTower);

            var combinedShard = targetShardInTower.MakeCopy();
            combinedShard.CombineWith(ref sourceShard);
            var combinedShardInsertCost = CalculateInsertCost(ref combinedShard);
            
            // var insertCost = targetShardInTower.costInsert > 0 ? targetShardInTower.costInsert : CalculateInsertCost(ref targetShardInTower);
            //var coef = (GetShardLevel(ref targetShardInTower) + GetShardLevel(ref sourceShard)) / 10; //TODO

            return Math.Max(3, combineCost + removeCost + combinedShardInsertCost);
        }

        public void CalculateSpread(ref Shard shard, out float maxSpread, out float distanceFactor)
        {
            // todo
            var firerate = GetFireRate(ref shard);
            var speed = GetProjectileSpeed(ref shard);
            
            maxSpread = 0.05f * firerate;
            distanceFactor = 0.01f * firerate * firerate / speed;
        }
    }
}