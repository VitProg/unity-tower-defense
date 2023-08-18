using Leopotam.EcsProto;
using td.features._common.systems;

namespace td.features._common
{
    public class Common_Module : IProtoModule
    {
        public void Init(IProtoSystems systems)
        {
            systems
                .AddSystem(new SturtupInitSystem())
                ;
        }

        public IProtoAspect[] Aspects()
        {
            return null;
        }

        public IProtoModule[] Modules()
        {
            return null;
        }
    }
}