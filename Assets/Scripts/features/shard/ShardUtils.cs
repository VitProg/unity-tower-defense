using System;
using System.Collections.Generic;
using Leopotam.Types;
using td.features.shard.components;
using td.features.shard.data;
using td.utils;
using UnityEngine;

namespace td.features.shard
{
    public static class ShardUtils
    {
        public static Color GetHoverColor(byte[] values, float a, Shards_Config_SO configSO)
        {
            var mixedColor = GetMixedColor(values, configSO) * 1.5f;
            return new Color(mixedColor.r + 0.15f, mixedColor.g + 0.15f, mixedColor.b + 0.15f, a);
        }

        public static Color GetMixedColor(byte[] values, Shards_Config_SO configSO) =>
            GetMixedColor(values[0], values[1], values[2], values[3], values[4], values[5], values[6], values[7],
                configSO);

        public static Color GetMixedColor(ref Shard shard, Shards_Config_SO configSO) =>
            GetMixedColor(
                shard.red,
                shard.green,
                shard.blue,
                shard.aquamarine,
                shard.yellow,
                shard.orange,
                shard.pink,
                shard.violet,
                configSO
            );

        public static Color GetMixedColor(
            byte red,
            byte green,
            byte blue,
            byte aquamarine,
            byte yellow,
            byte orange,
            byte pink,
            byte violet,
            Shards_Config_SO configSO
        )
        {
            var colors = new List<Color>();
            AddColorsToList(ShardTypes.Red, red, configSO, ref colors);
            AddColorsToList(ShardTypes.Green, green, configSO, ref colors);
            AddColorsToList(ShardTypes.Blue, blue, configSO, ref colors);
            AddColorsToList(ShardTypes.Aquamarine, aquamarine, configSO, ref colors);
            AddColorsToList(ShardTypes.Yellow, yellow, configSO, ref colors);
            AddColorsToList(ShardTypes.Orange, orange, configSO, ref colors);
            AddColorsToList(ShardTypes.Pink, pink, configSO, ref colors);
            AddColorsToList(ShardTypes.Violet, violet, configSO, ref colors);

            var mixedColor = AvgColorFromList(colors);

            colors.Clear();

            return mixedColor;
        }

        public static void GetColors(byte red,
            byte green,
            byte blue,
            byte aquamarine,
            byte yellow,
            byte orange,
            byte pink,
            byte violet,
            Shards_Config_SO configSO,
            List<Color> colors,
            uint count = 32)
        {
            colors.Clear();

            var shardArray = new[] { red, green, blue, aquamarine, yellow, orange, pink, violet };
            var quantity = GetQuantity(shardArray);
            
            var index = 0;
            for (var i = 0; i < shardArray.Length; i++)
            {
                var w = (float)shardArray[i] / quantity;
                if (!(w > 0.01f)) continue;

                var wInt = (int) Math.Ceiling(w * 10);

                var color = configSO[i];
                // var size = count * w;
                var limit = (index - 1) + wInt;
                for (; index < limit && index < count; index++)
                {
                    colors.Add(color);
                }
            }
        }

        public static void MixShards(ref Shard targetShard, ref Shard sourceShard)
        {
            targetShard.red = (byte)Math.Min(100, targetShard.red + sourceShard.red);
            targetShard.green = (byte)Math.Min(100, targetShard.green + sourceShard.green);
            targetShard.blue = (byte)Math.Min(100, targetShard.blue + sourceShard.blue);
            targetShard.aquamarine = (byte)Math.Min(100, targetShard.aquamarine + sourceShard.aquamarine);
            targetShard.yellow = (byte)Math.Min(100, targetShard.yellow + sourceShard.yellow);
            targetShard.orange = (byte)Math.Min(100, targetShard.orange + sourceShard.orange);
            targetShard.pink = (byte)Math.Min(100, targetShard.pink + sourceShard.pink);
            targetShard.violet = (byte)Math.Min(100, targetShard.violet + sourceShard.violet);

            var all = (float)GetQuantity(ref targetShard);

            if (targetShard.red / all < 0.01f) targetShard.red = 0;
            if (targetShard.green / all < 0.01f) targetShard.green = 0;
            if (targetShard.blue / all < 0.01f) targetShard.blue = 0;
            if (targetShard.aquamarine / all < 0.01f) targetShard.aquamarine = 0;
            if (targetShard.yellow / all < 0.01f) targetShard.yellow = 0;
            if (targetShard.orange / all < 0.01f) targetShard.orange = 0;
            if (targetShard.pink / all < 0.01f) targetShard.pink = 0;
            if (targetShard.violet / all < 0.01f) targetShard.violet = 0;
        }

        public static byte[] ToArray(ref Shard shard) => new[]
        {
            shard.red,
            shard.green,
            shard.blue,
            shard.aquamarine,
            shard.yellow,
            shard.orange,
            shard.pink,
            shard.violet,
        };

        public static byte[] ToArray(
            byte red,
            byte green,
            byte blue,
            byte aquamarine,
            byte yellow,
            byte orange,
            byte pink,
            byte violet
        ) => new[]
        {
            red,
            green,
            blue,
            aquamarine,
            yellow,
            orange,
            pink,
            violet,
        };

        public static byte GetMin(ref Shard shard) => ByteUtils.Min(ToArray(ref shard));
        public static byte GetMax(ref Shard shard) => ByteUtils.Max(ToArray(ref shard));

        public static uint GetQuantity(ref Shard shard)
        {
            return (uint)(shard.red + shard.green + shard.blue + shard.aquamarine +
                          shard.yellow + shard.orange + shard.pink + shard.violet);
        }

        public static int GetQuantity(byte[] values)
        {
            var quantity = 0;
            foreach (var value in values)
            {
                quantity += value;
            }

            return quantity;
        }

        public static Color GetColor(ShardTypes type, Shards_Config_SO configSO)
        {
            return type switch
            {
                ShardTypes.Red => configSO.redShardColor,
                ShardTypes.Green => configSO.greenShardColor,
                ShardTypes.Blue => configSO.blueShardColor,
                ShardTypes.Yellow => configSO.yellowShardColor,
                ShardTypes.Orange => configSO.orangeShardColor,
                ShardTypes.Pink => configSO.pinkShardColor,
                ShardTypes.Violet => configSO.violetShardColor,
                ShardTypes.Aquamarine => configSO.aquamarineShardColor,
#if UNITY_EDITOR
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
#endif
            };
        }


        /////////////////\


        private static void AddColorsToList(ShardTypes type, byte quantity, Shards_Config_SO configSO, ref List<Color> colors)
        {
            var color = GetColor(type, configSO);
            for (var i = 0; i < quantity; i++)
            {
                colors.Add(color);
            }
        }

        private static Color AvgColorFromList(List<Color> colors)
        {
            double r = 0;
            double g = 0;
            double b = 0;

            foreach (var color in colors)
            {
                r += color.r;
                g += color.g;
                b += color.b;
            }

            r /= colors.Count;
            g /= colors.Count;
            b /= colors.Count;

            return new Color((float)r, (float)g, (float)b, 1f);
        }

        public static void Copy(ref Shard target, ref Shard source)
        {
            target.red = source.red;
            target.green = source.green;
            target.blue = source.blue;
            target.aquamarine = source.aquamarine;
            target.pink = source.pink;
            target.orange = source.orange;
            target.violet = source.violet;
            target.yellow = source.yellow;
            target.cost = source.cost;
            target.costCombine = source.costCombine;
            target.costInsert = source.costInsert;
            target.costRemove = source.costRemove;
            target.costDrop = source.costDrop;
        }

        public static void Copy(ref Shard target, Shard source) => Copy(ref target, ref source);

        public static void Clear(ref Shard shard)
        {
            shard.red = 0;
            shard.green = 0;
            shard.blue = 0;
            shard.aquamarine = 0;
            shard.pink = 0;
            shard.orange = 0;
            shard.violet = 0;
            shard.yellow = 0;
            shard.cost = 0;
            shard.costCombine = 0;
            shard.costInsert = 0;
            shard.costRemove = 0;
            shard.costDrop = 0;
        }

        public static void Set(ref Shard shard, ShardTypes fieldName, byte value = 1)
        {
            switch (fieldName)
            {
                case ShardTypes.Red:
                    shard.red = value;
                    break;
                case ShardTypes.Green:
                    shard.green = value;
                    break;
                case ShardTypes.Blue:
                    shard.blue = value;
                    break;
                case ShardTypes.Aquamarine:
                    shard.aquamarine = value;
                    break;
                case ShardTypes.Pink:
                    shard.pink = value;
                    break;
                case ShardTypes.Orange:
                    shard.orange = value;
                    break;
                case ShardTypes.Violet:
                    shard.violet = value;
                    break;
                case ShardTypes.Yellow:
                    shard.yellow = value;
                    break;
            }
        }

        public static void Set(ref Shard shard, string fieldName, byte value = 1)
        {
            var f = fieldName.ToLower().Trim();
            switch (f)
            {
                case "red":
                    shard.red = value;
                    break;
                case "green":
                    shard.green = value;
                    break;
                case "blue":
                    shard.blue = value;
                    break;
                case "aquamarine":
                    shard.aquamarine = value;
                    break;
                case "pink":
                    shard.pink = value;
                    break;
                case "orange":
                    shard.orange = value;
                    break;
                case "violet":
                    shard.violet = value;
                    break;
                case "yellow":
                    shard.yellow = value;
                    break;
            }
        }

        public static void ReduceToOne(ref Shard shard)
        {
            shard.red = (byte)MathFast.Min(shard.red, 1);
            shard.green = (byte)MathFast.Min(shard.green, 1);
            shard.blue = (byte)MathFast.Min(shard.blue, 1);
            shard.aquamarine = (byte)MathFast.Min(shard.aquamarine, 1);
            shard.pink = (byte)MathFast.Min(shard.pink, 1);
            shard.orange = (byte)MathFast.Min(shard.orange, 1);
            shard.violet = (byte)MathFast.Min(shard.violet, 1);
            shard.yellow = (byte)MathFast.Min(shard.yellow, 1);
        }

        public static void Multiple(ref Shard shard, int mul)
        {
            shard.red = (byte)MathFast.Min(shard.red * mul, 255);
            shard.green = (byte)MathFast.Min(shard.green * mul, 255);
            shard.blue = (byte)MathFast.Min(shard.blue * mul, 255);
            shard.aquamarine = (byte)MathFast.Min(shard.aquamarine * mul, 255);
            shard.pink = (byte)MathFast.Min(shard.pink * mul, 255);
            shard.orange = (byte)MathFast.Min(shard.orange * mul, 255);
            shard.violet = (byte)MathFast.Min(shard.violet * mul, 255);
            shard.yellow = (byte)MathFast.Min(shard.yellow * mul, 255);
        }
    }
}