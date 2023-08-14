using System;
using Leopotam.EcsProto;
using td.utils.ecs;

namespace td.features.projectile.lightning
{
    public class Lightning_Module : IProtoModule
    {
        private readonly Func<float> getDeltaTime;
        
        public Lightning_Module(Func<float> getDeltaTime)
        {
            this.getDeltaTime = getDeltaTime;
        }
        
        public void Init(IProtoSystems systems)
        {
            systems
                .AddSystem(new LightningCorrectionSystem(1/30f, 0f, getDeltaTime))
                .AddSystem(new LightningNeighborsSystem(1/5f, 0f, getDeltaTime))
                .AddSystem(new LightningDamageSystem())
                //
                .AddService(new Lightning_Converter(), true)
                .AddService(new Lightning_Service(), true)
                ;
        }

        public IProtoAspect[] Aspects()
        {
            return new IProtoAspect[]
            {
                new Lightning_Aspect(),
            };
        }

        public IProtoModule[] Modules()
        {
            return null;
        }
    }
}