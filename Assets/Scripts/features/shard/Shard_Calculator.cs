using System;
using System.Runtime.CompilerServices;
using Leopotam.EcsProto.QoL;
using Leopotam.Types;
using td.features._common;
using td.features.level;
using td.features.shard.components;
using td.features.shard.data;

namespace td.features.shard
{
    public class Shard_Calculator
    {
        [DI] private Shards_Config_SO configSO;
        [DI] private Level_State levelState;
        
        public float GetProjectileSpeed(ref Shard shard)
        {
            return 5;
            var speedBase = configSO.speedBase;
            // var speedReduce = config.speedReduce;
            var impactOfRed = configSO.speedImpactOfRed;

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
            var lm = configSO.GetShardLevelByQuantity(quantity) / 5.35f;
            var levelModifier = lm * lm;
            var speed = (float)(levelModifier * MathF.Log10(quantity * quantity) + speedBase);

            if (shard.red > 0)
            {
                var redAmplifier = GetShardAmplifier(shard.red, quantity);
                speed /= (MathF.Sqrt(MathF.Sqrt(redAmplifier) / impactOfRed)) + 1f;
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
            return o / a * (MathF.Log(a) * o + 1.0f) * (MathF.Pow(2.0f, MathF.Sqrt(o))) / 2.0f / MathF.Sqrt(a / o);
        }

        public float GetShardDropAmplifier(uint quantity, uint allQuantity) =>
            GetShardAmplifier(quantity, allQuantity) * 6f;

        public float GetShardTrapAmplifier(uint quantity, uint allQuantity) =>
            GetShardAmplifier(quantity, allQuantity) / 6f;

        // @see https://docs.google.com/spreadsheets/d/1Vze5h6492TZL5gN8KHKPSS2ckDovCIrYgW4ylsVhmCk/edit#gid=676981276
        public float GetNearbyBuildingsAmplifier(uint quantity) =>
            0.75f / MathF.Exp(quantity / 1.15f - 1f) + 1f;

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
            // var powerDivider = config.slowPowerDivider;
            // var powerMin = config.slowPowerMin;
            // var durationDevider = config.slowDurationDevider;
            // var durationMin = config.slowDurationMin;
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
            var baseDamage = configSO.baseDamage;
            var impactOfRed = configSO.baseDamageImpactOfRed;
            
            var quantity = ShardUtils.GetQuantity(ref shard);
            
            // var max = ShardUtils.GetMax(ref shardPackedEntity);
            //
            // damage = GetShardAmplifier(MathF.Max(max, Mathf.CeilToInt(quantity / 10f)));

            var lm = configSO.GetShardLevelByQuantity(quantity) / 2.97823f;
            var levelModifier = 1 + lm * lm;
            var q = quantity / 7.5f;
            damage = levelModifier * (q * q) + baseDamage;

            //todo
            if (shard.red > 0)
            {
                var redAmplifier = GetShardDropAmplifier(shard.red, quantity);
                damage /= MathF.Sqrt(redAmplifier * impactOfRed) + 1f;
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
        public byte GetShardLevel(ref Shard shard)
        {
            var quantity = ShardUtils.GetQuantity(ref shard);
            return configSO.GetShardLevelByQuantity(quantity);
        }
        
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public int GetShardLevel(uint quantity) => configSO.GetShardLevelByQuantity(quantity);
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
        public int GetQuantityForLevel(uint quantity) => configSO.triangularPyramids[Math.Clamp(quantity - 1, 0, configSO.triangularPyramids.Length - 1)];

        public void CalculateExplosiveParams(
            ref Shard shard,
            out float damage,
            out float diameter,
            out float damageFading
        ) {
            var damageReducer = configSO.explosiveDamageReducer;
            var radiusAdd = configSO.explosiveRadiusAdd;
            var fadingReducer = configSO.explosiveFadingReducer;
            
            var quantity = ShardUtils.GetQuantity(ref shard);
            var amplifier = GetShardAmplifier(shard.red, quantity);

            damage = amplifier / damageReducer;
            diameter = MathF.Log(MathF.Sqrt(amplifier / 10f)) + radiusAdd;
            damageFading = 0.9f;//diameter / Mathf.Sqrt(shard.red * fadingReducer);
        }

        public void CalculatePoisonParams(
            ref Shard shard,
            out float damage,
            out float duration)
        {
            var damageBase = configSO.poisonDamageBase;
            var damageReducer = configSO.poisonDamageReducer;
            // var minInterval = config.poisonMinInterval;
            var minDuration = configSO.poisonMinDuration;
            
            var amplifier = GetShardAmplifier(shard.green, ShardUtils.GetQuantity(ref shard));

            damage = amplifier / MathF.Sqrt(shard.green * 2f) / damageReducer;
            // interval = Mathf.Sqrt((Mathf.Log(amplifier))) + minInterval;
            duration = MathF.Sqrt(MathF.Sqrt(amplifier)) + minDuration;

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
            var baseFireRate = configSO.fireRateBase;
            var pinkReduser = configSO.fireRatePinkReduser;
            
            var max = ShardUtils.GetMax(ref shard);
            var quantity = ShardUtils.GetQuantity(ref shard);
            var amplifier = GetShardAmplifier(max);

            // var fireRate = baseFireRate + (Mathf.Log(amplifier) * Mathf.Sqrt(max));
            var lm = configSO.GetShardLevelByQuantity(quantity) / 3.62f;
            var levelModifier = lm * lm;
            var fireRate = levelModifier * MathF.Log10(quantity * quantity) + baseFireRate;

            if (shard.pink > 0)
            {
                var pinkAmplifier = GetShardAmplifier(shard.pink, quantity);
                fireRate += MathF.Sqrt(pinkAmplifier * pinkReduser) / MathF.Sqrt(quantity);
            }

            if (shard.aquamarine > 0)
            {
                fireRate /= MathF.Sqrt(shard.aquamarine) + 1f;
            }

            return fireRate;
        }

        // yellow - увеличивает радиус
        public float GetRadius(ref Shard shard)
        {
            var baseRadius = configSO.radiusBase;
            var impactOfYellow = configSO.radiusImpactOfYellow;

            var quantity = ShardUtils.GetQuantity(ref shard);
            var levelCooficient = configSO.GetShardLevelByQuantity(quantity);
            var lm = levelCooficient / 10f;
            var levelModifier = lm * lm;

            var radius = levelModifier * MathF.Log10((quantity + 1) * (quantity + 1)) + baseRadius;

            if (shard.yellow > 0)
            {
                var yellowAmplifier = GetShardAmplifier(shard.yellow, quantity);

                radius += MathF.Sqrt(yellowAmplifier * impactOfYellow) / MathF.Sqrt(quantity);
            }

            radius = MathFast.Max(baseRadius, radius);

            return radius;
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public uint GetBasePriceByType(ShardTypes type)
        {
            ref var cfg = ref levelState.GetLevelConfig();
            return type switch
            {
                ShardTypes.Red => cfg.shardsPrice.red,
                ShardTypes.Green => cfg.shardsPrice.green,
                ShardTypes.Blue => cfg.shardsPrice.blue,
                ShardTypes.Yellow => cfg.shardsPrice.yellow,
                ShardTypes.Orange => cfg.shardsPrice.orange,
                ShardTypes.Pink => cfg.shardsPrice.pink,
                ShardTypes.Violet => cfg.shardsPrice.violet,
                ShardTypes.Aquamarine => cfg.shardsPrice.aquamarine,
                _ => 99999
            };
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public uint CalculatePriceByType(ShardTypes type, uint typeQuantity)
        {
            var basePrice = GetBasePriceByType(type);
            var level = GetShardLevel(typeQuantity);
            return Math.Max(1, (uint)(typeQuantity * basePrice * level * level));
        }        
        
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public uint CalculateInsertTime(ref Shard shard)
        {
            var quantity = ShardUtils.GetQuantity(ref shard);
            var level = GetShardLevel(quantity);
            return Math.Max(0, (uint)((quantity / 8f + 1f) * level * level)); //todo
        }

        public uint CalculatePrice(ref Shard shard)
        {
            uint price = 0;
            price += CalculatePriceByType(ShardTypes.Red, shard.red);
            price += CalculatePriceByType(ShardTypes.Green, shard.green);
            price += CalculatePriceByType(ShardTypes.Blue, shard.blue);
            price += CalculatePriceByType(ShardTypes.Yellow, shard.yellow);
            price += CalculatePriceByType(ShardTypes.Orange, shard.orange);
            price += CalculatePriceByType(ShardTypes.Pink, shard.pink);
            price += CalculatePriceByType(ShardTypes.Violet, shard.violet);
            price += CalculatePriceByType(ShardTypes.Aquamarine, shard.aquamarine);
            return Math.Max(1, price);
        }
        
        public uint CalculateInsertPrice(ref Shard shard)
        {
            uint price = 0;
            price += (uint)(CalculatePriceByType(ShardTypes.Red, shard.red) / 6 * (int)MathF.Ceiling(GetShardLogSqrLevel(shard.red) + 1));
            price += (uint)(CalculatePriceByType(ShardTypes.Green, shard.green) / 6 * (int)MathF.Ceiling(GetShardLogSqrLevel(shard.green) + 1));
            price += (uint)(CalculatePriceByType(ShardTypes.Blue, shard.blue) / 6 * (int)MathF.Ceiling(GetShardLogSqrLevel(shard.blue) + 1));
            price += (uint)(CalculatePriceByType(ShardTypes.Yellow, shard.yellow) / 6 * (int)MathF.Ceiling(GetShardLogSqrLevel(shard.yellow) + 1));
            price += (uint)(CalculatePriceByType(ShardTypes.Orange, shard.orange) / 6 * (int)MathF.Ceiling(GetShardLogSqrLevel(shard.orange) + 1));
            price += (uint)(CalculatePriceByType(ShardTypes.Pink, shard.pink) / 6 * (int)MathF.Ceiling(GetShardLogSqrLevel(shard.pink) + 1));
            price += (uint)(CalculatePriceByType(ShardTypes.Violet, shard.violet) / 6 * (int)MathF.Ceiling(GetShardLogSqrLevel(shard.violet) + 1));
            price += (uint)(CalculatePriceByType(ShardTypes.Aquamarine, shard.aquamarine) / 6 * (int)MathF.Ceiling(GetShardLogSqrLevel(shard.aquamarine) + 1));
            return Math.Max(1, price);
        }
        
        public uint CalculateCombinePrice(ref Shard shard)
        {
            uint price = 0;
            price += (uint)(CalculatePriceByType(ShardTypes.Red, shard.red) / 4 * (int)MathF.Ceiling(GetShardLogSqrLevel(shard.red) + 1));
            price += (uint)(CalculatePriceByType(ShardTypes.Green, shard.green) / 4 * (int)MathF.Ceiling(GetShardLogSqrLevel(shard.green) + 1));
            price += (uint)(CalculatePriceByType(ShardTypes.Blue, shard.blue) / 4 * (int)MathF.Ceiling(GetShardLogSqrLevel(shard.blue) + 1));
            price += (uint)(CalculatePriceByType(ShardTypes.Yellow, shard.yellow) / 4 * (int)MathF.Ceiling(GetShardLogSqrLevel(shard.yellow) + 1));
            price += (uint)(CalculatePriceByType(ShardTypes.Orange, shard.orange) / 4 * (int)MathF.Ceiling(GetShardLogSqrLevel(shard.orange) + 1));
            price += (uint)(CalculatePriceByType(ShardTypes.Pink, shard.pink) / 4 * (int)MathF.Ceiling(GetShardLogSqrLevel(shard.pink) + 1));
            price += (uint)(CalculatePriceByType(ShardTypes.Violet, shard.violet) / 4 * (int)MathF.Ceiling(GetShardLogSqrLevel(shard.violet) + 1));
            price += (uint)(CalculatePriceByType(ShardTypes.Aquamarine, shard.aquamarine) / 4 * (int)MathF.Ceiling(GetShardLogSqrLevel(shard.aquamarine) + 1));

            return Math.Max(2, price);
        }
        
        public uint CalculateRemovePrice(ref Shard shard) => Math.Max(1, CalculateInsertPrice(ref shard) / 2);
        
        public uint CalculateRemoveTime(ref Shard shard) => Math.Max(0, CalculateInsertTime(ref shard) / 2);

        public uint CalculateDropPrice(ref Shard shard) => Math.Max(6, CalculateInsertPrice(ref shard) + CalculateCombinePrice(ref shard, ref shard) / 2);

        public uint CalculateCombinePrice(ref Shard targetShard, ref Shard sourceShard)
        {
            uint price = 0;

            if (targetShard.priceCombine == 0) targetShard.priceCombine = CalculateCombinePrice(ref targetShard);
            price += targetShard.priceCombine;
            
            if (sourceShard.priceCombine > 0) sourceShard.priceCombine = CalculateCombinePrice(ref sourceShard);
            price += sourceShard.priceCombine;

            return Math.Max(2, price);
        }

        public float CalculateCombineTime(ref Shard shard)
        {
            return Math.Max(0, CalculateInsertTime(ref shard) / 5f);
        }

        public uint CalculateCombineTime(ref Shard targetShard, ref Shard sourceShard)
        {
            float time = 0;

            if (targetShard.timeCombine == 0) targetShard.timeCombine = CalculateCombineTime(ref targetShard);
            time += targetShard.timeCombine;
            
            if (sourceShard.timeCombine > 0) sourceShard.timeCombine = CalculateCombineTime(ref sourceShard);
            time += sourceShard.timeCombine;

            return Math.Max(0, (uint)time);
        }

        public uint CalculateCombineIntoTowerPrice(ref Shard targetShardInTower, ref Shard sourceShard)
        {
            var combinePrice = CalculateCombinePrice(ref targetShardInTower, ref sourceShard);
            var removePrice = targetShardInTower.priceRemove > 0 ? targetShardInTower.priceRemove : CalculateRemovePrice(ref targetShardInTower);

            var combinedShard = targetShardInTower.MakeCopy();
            combinedShard.CombineWith(ref sourceShard);
            var combinedShardInsertPrice = CalculateInsertPrice(ref combinedShard);

            return Math.Max(3, combinePrice + removePrice + combinedShardInsertPrice);
        }
        
        public uint CalculateCombineIntoTowerTime(ref Shard targetShardInTower, ref Shard sourceShard)
        {
            var combineTime = CalculateCombineTime(ref targetShardInTower, ref sourceShard);
            var removeTime = targetShardInTower.timeRemove > 0 ? targetShardInTower.timeRemove : CalculateRemoveTime(ref targetShardInTower);

            var combinedShard = targetShardInTower.MakeCopy();
            combinedShard.CombineWith(ref sourceShard);
            var combinedShardInsertTime = CalculateInsertTime(ref combinedShard);

            return Math.Max(1, combineTime + removeTime + combinedShardInsertTime);
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