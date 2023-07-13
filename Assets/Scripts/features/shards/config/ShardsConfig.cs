using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace td.features.shards.config
{
    [CreateAssetMenu(menuName = "TD/ShardsConfig")]
    public class ShardsConfig : ScriptableObject
    {
        [Header("Shard Colors")] public Color redShardColor;
        public Color greenShardColor;
        public Color blueShardColor;
        public Color aquamarineShardColor;
        public Color yellowShardColor;
        public Color orangeShardColor;
        public Color pinkShardColor;
        public Color violetShardColor;

        [Header("Shard Coeficients")] [Header("* Speed")]
        public float speedBase = 5f;

        // public float speedReduce = 5f;
        public float speedImpactOfRed = 50f;

        [Header("* Base Damage")] public float baseDamage = 1f;
        public float baseDamageImpactOfRed = 2f;

        [Header("* Slow")] public float slowPowerDivider = 300f;
        public float slowPowerMin = 0.1f;
        public float slowDurationDevider = 10f;
        public float slowDurationMin = 3f;

        [Header("* Explosive")] public float explosiveDamageReducer = 6f;
        public float explosiveRadiusAdd = 1f;
        public float explosiveFadingReducer = 6f;

        [Header("* Poison")] public float poisonDamageBase = 0.5f;
        public float poisonDamageReducer = 2f;
        public float poisonMinInterval = 1f;
        public float poisonMinDuration = 2f;

        [Header("* Fire Rate")] public float fireRateBase = 0.5f;
        public float fireRatePinkReduser = 2f;

        [Header("* Radius")] public float radiusBase = 1.5f;

        [FormerlySerializedAs("radiusYellowMul")]
        public float radiusImpactOfYellow = 1.1f;

        [Header("Levels Cooficients")]
        public readonly int[] triangularPyramids = {
            /* 1*/ 1,
            /* 2*/ 4,
            /* 3*/ 10,
            /* 4*/ 20,
            /* 5*/ 35,
            /* 6*/ 56,
            /* 7*/ 84,
            /* 8*/ 120,
            /* 9*/ 165,
            /*10*/ 220,
            /*11*/ 286,
            /*12*/ 364,
            /*13*/ 455,
            /*14*/ 560,
            /*15*/ 680,
        };

        [Header("Levels Sprites")]
        public Sprite[] levelSprites;

        public Color GetColor(int index)
        {
            return (index % 8) switch
            {
                0 => redShardColor,
                1 => greenShardColor,
                2 => blueShardColor,
                3 => aquamarineShardColor,
                4 => yellowShardColor,
                5 => orangeShardColor,
                6 => pinkShardColor,
                7 => violetShardColor,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public byte GetColorIndex(string color)
        {
            return (color.ToLower().Trim()) switch
            {
                "red" => 0,
                "green" => 1,
                "blue" => 2,
                "aquamarine" => 3,
                "yellow" => 4,
                "orange" => 5,
                "pink" => 6,
                "violet" => 7,
                _ => throw new ArgumentOutOfRangeException()
            };
            
        }

        public int GetLevelCoefficient(int quantity)
        {
            var o = triangularPyramids;
            for (var i = o.Length - 1; i >= 0; i--) {
                if (o[i] <= quantity) {
                    return i + 1;
                }
            }
            return 1;
        }

        public Color this[int index] => GetColor(index);
    }
}