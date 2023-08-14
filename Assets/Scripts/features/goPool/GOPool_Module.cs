using Leopotam.EcsProto;
using td.utils.ecs;

namespace td.features.goPool
{
    public class GOPool_Module : IProtoModule
    {
        public void Init(IProtoSystems systems)
        {
            systems
                .AddService(new GOPool_Service(), true);
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