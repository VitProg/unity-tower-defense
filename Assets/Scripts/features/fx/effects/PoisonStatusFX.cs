using System;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.features._common;
using td.features.fx.events;
using td.features.fx.types;
using td.features.goPool;
using td.monoBehaviours;
using td.utils.ecs;
using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

namespace td.features.fx.effects
{
    [Serializable]
    public struct PoisonStatusFX : IEntityFallowFX, IWithColorFX, IWithSpriteAnimatorFX, IEcsAutoReset<PoisonStatusFX>
    {
        public Color Color { get; set; }
        public string PrefabName { get; set; }
        public bool? IsReverse { get; set; }
        public float? Speed { get; set; }
        public bool? IsLoop { get; set; }

        public void AutoReset(ref PoisonStatusFX c)
        {
            Debug.Log($"PoisonStatusFX AutoReset");
            c.PrefabName = "PoisonStatusFX";
            c.Color = Constants.FX.PoisonDamageColor;
            c.IsReverse = true;
            c.Speed = 0f;
            c.IsLoop = true;
        }
    }
}