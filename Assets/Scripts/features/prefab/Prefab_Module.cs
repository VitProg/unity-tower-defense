using Leopotam.EcsProto;
using td.utils.ecs;

namespace td.features.prefab
{
    public class Prefab_Module : IProtoModule
    {
        public void Init(IProtoSystems systems)
        {
            // Debug.Log($"{GetType().Name} Init");
            systems
                .AddService(new Prefab_Service(), true)
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