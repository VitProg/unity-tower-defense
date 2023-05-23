using System;
using System.Collections.Generic;
using Leopotam.EcsLite;
using td.features.shards.config;
using td.features.towers;
using td.utils;
using td.utils.ecs;
using UnityEngine;

namespace td.features.shards
{
    public static class ShardUtils
    {
        public static Color GetHoverColor(byte[] values, float a, ShardsConfig config) {
            var mixedColor = GetMixedColor(values, config) * 1.5f;
            return new Color(mixedColor.r + 0.15f, mixedColor.g + 0.15f, mixedColor.b + 0.15f, a);
        }

        public static Color GetMixedColor(byte[] values, ShardsConfig config) =>
            GetMixedColor(values[0], values[1], values[2], values[3], values[4], values[5], values[6], values[7], config);
        public static Color GetMixedColor(ref Shard shard, ShardsConfig config) =>
            GetMixedColor(
                shard.red,
                shard.green,
                shard.blue,
                shard.aquamarine,
                shard.yellow,
                shard.orange,
                shard.pink,
                shard.violet,
                config
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
            ShardsConfig config
        )
        {
            var colors = new List<Color>();
            AddColorsToList(ShardTypes.Red, red, config, ref colors);
            AddColorsToList(ShardTypes.Green, green, config, ref colors);
            AddColorsToList(ShardTypes.Blue, blue, config, ref colors);
            AddColorsToList(ShardTypes.Aquamarine, aquamarine, config, ref colors);
            AddColorsToList(ShardTypes.Yellow, yellow, config, ref colors);
            AddColorsToList(ShardTypes.Orange, orange, config, ref colors);
            AddColorsToList(ShardTypes.Pink, pink, config, ref colors);
            AddColorsToList(ShardTypes.Violet, violet, config, ref colors);

            var mixedColor = AvgColorFromList(colors);
            
            colors.Clear();

            return mixedColor;
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

        public static byte GetMin(ref Shard shard) => ByteUtils.Min(ToArray(ref shard));
        public static byte GetMax(ref Shard shard) => ByteUtils.Max(ToArray(ref shard));

        public static int GetQuantity(ref Shard shard)
        {
            return shard.red + shard.green + shard.blue + shard.aquamarine +
                   shard.yellow + shard.orange + shard.pink + shard.violet;
        }
        
        public static int GetQuantity(byte[] values) {
            var quantity = 0;
            foreach (var value in values)
            {
                quantity += value;
            }

            return quantity;
        }

        public static Color GetColor(ShardTypes type, ShardsConfig config) {
            return type switch
            {
                ShardTypes.Red => config.redShardColor,
                ShardTypes.Green => config.greenShardColor,
                ShardTypes.Blue => config.blueShardColor,
                ShardTypes.Yellow => config.yellowShardColor,
                ShardTypes.Orange => config.orangeShardColor,
                ShardTypes.Pink => config.pinkShardColor,
                ShardTypes.Violet => config.violetShardColor,
                ShardTypes.Aquamarine => config.aquamarineShardColor,
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }
        
        
        /////////////////\
        
        
        
        private static void AddColorsToList(ShardTypes type, byte quantity, ShardsConfig config, ref List<Color> colors)
        {
            var color = GetColor(type, config);
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


        public static bool HasShardInTower(EcsWorld world, int towerEntity)
        {
            if (world.HasComponent<ShardTower>(towerEntity))
            {
                ref var shardTower = ref world.GetComponent<ShardTower>(towerEntity);
                if (shardTower.shard.Unpack(world, out var shardEntity))
                {
                    return true;
                }
            }

            return false;
        }

        public static ref Shard GetShardInTower(EcsWorld world, int towerEntity)
        {
            ref var shardTower = ref world.GetComponent<ShardTower>(towerEntity);
            if (shardTower.shard.Unpack(world, out var shardEntity))
            {
                return ref world.GetComponent<Shard>(shardEntity);
            }

            throw new NullReferenceException("Shard not found in Tower");
        }
    }

    public enum ShardTypes
    {
        Red = 1,
        Green = 2,
        Blue = 3,
        Yellow = 4,
        Orange = 5,
        Pink = 6,
        Violet = 7,
        Aquamarine = 8,
    }
}