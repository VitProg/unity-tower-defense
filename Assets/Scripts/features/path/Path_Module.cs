using Leopotam.EcsProto;
using td.utils.ecs;

namespace td.features.path
{
    public class Path_Module : IProtoModule
    {
        public void Init(IProtoSystems systems)
        {
            // Debug.Log($"{GetType().Name} Init");
            
            systems
                .AddService(new Path_Service(), true)
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