using System;
using Leopotam.EcsProto;
using td.features.fx.types;
using UnityEngine;

namespace td.features.fx.effects
{
    [Serializable]
    public struct ColdStatusFX : IEntityFallowFX, IWithColorFX, IWithSpriteAnimatorFX, IProtoAutoReset<ColdStatusFX>
    {
        public Color Color { get; set; }
        public string PrefabName { get; set; }
        public bool? IsReverse { get; set; }
        public float? Speed { get; set; }
        public bool? IsLoop { get; set; }

        public void AutoReset(ref ColdStatusFX c)
        {
            c.Color = Constants.FX.ColdDamageColor;
            c.IsReverse = true;
            c.Speed = 0f;
            c.IsLoop = true;
        }
    }
}