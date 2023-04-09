using System;
using Mitfart.LeoECSLite.UniLeo.Providers;

namespace td.features.fire
{
    [Serializable]
    public struct IsProjectile
    {
        public float damage;
         // [AllowNull] public Rigidbody2D rb;
    }
    
    public class IsProjectileProvider : EcsProvider<IsProjectile>
    {
    }
}