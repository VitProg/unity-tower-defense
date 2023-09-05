using Leopotam.EcsProto;
using td.utils.ecs;

namespace td.features._common
{
    public class Common_Module : IProtoModule
    {
        public void Init(IProtoSystems systems)
        {
            systems
                .AddService(new Common_Service(), true)
                ;
        }

        public IProtoAspect[] Aspects()
        {
            return new IProtoAspect[]
            {
                new Common_Aspect(),
            };
        }

        public IProtoModule[] Modules()
        {
            return null;
        }
    }
}