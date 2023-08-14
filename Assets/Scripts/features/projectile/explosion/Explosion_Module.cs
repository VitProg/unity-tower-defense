using Leopotam.EcsProto;
using td.utils.ecs;

namespace td.features.projectile.explosion
{
    public class Explosion_Module : IProtoModule
    {
        public void Init(IProtoSystems systems)
        {
            systems
                .AddSystem(new Explosion_System())
                //
                .AddService(new Explosion_Service(), true)
                .AddService(new Explosion_Converter(), true)
                ;
        }

        public IProtoAspect[] Aspects()
        {
            return new IProtoAspect[]
            {
                new Explosion_Aspect()
            };
        }

        public IProtoModule[] Modules()
        {
            return null;
        }
    }
}