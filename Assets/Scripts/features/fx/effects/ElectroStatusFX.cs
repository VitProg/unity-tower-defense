using System;
using Leopotam.EcsLite;
using td.features.fx.types;
using UnityEngine;

namespace td.features.fx.effects
{
    [Serializable]
    public struct ElectroStatusFX : IEntityFallowFX, IWithColorFX, IWithSpriteAnimatorFX, IEcsAutoReset<ElectroStatusFX>
    {
        public Color Color { get; set; }
        public string PrefabName { get; set; }
        public bool? IsReverse { get; set; }
        public float? Speed { get; set; }
        public bool? IsLoop { get; set; }

        public void AutoReset(ref ElectroStatusFX c)
        {
            c.Color = Constants.FX.ElectroDamageColor;
            c.IsReverse = true;
            c.Speed = 0f;
            c.IsLoop = true;
        }
    }
}