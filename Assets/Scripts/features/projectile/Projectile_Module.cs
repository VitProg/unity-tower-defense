using System;
using Leopotam.EcsProto;
using td.features.projectile.explosion;
using td.features.projectile.lightning;
using td.features.projectile.systems;
using td.utils.ecs;
using UnityEngine;

namespace td.features.projectile
{
    public class Projectile_Module : IProtoModule
    {
        private readonly Func<float> getDeltaTime;
        
        public Projectile_Module(Func<float> getDeltaTime)
        {
            // Debug.Log($"{GetType().Name} Init");
            this.getDeltaTime = getDeltaTime;
        }
        
        public void Init(IProtoSystems systems)
        {
            systems
                .AddSystem(new ProjectileMovementSystem(1/60f, 0f, getDeltaTime))
                .AddSystem(new ProjectileReachTargetSystem())
                //
                .AddService(new Projectile_Service(), true)
                .AddService(new Projectile_Converter(), true)
                ;
        }

        public IProtoAspect[] Aspects()
        {
            return new IProtoAspect[]
            {
                new Projectile_Aspect(),
            };
        }

        public IProtoModule[] Modules()
        {
            return new IProtoModule[]
            {
                new Explosion_Module(),
                new Lightning_Module(getDeltaTime),
            };
        }
    }
}