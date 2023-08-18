using Leopotam.EcsProto;
using td.utils.ecs;

namespace td.features.window
{
    public class Window_Module : IProtoModule
    {
        public void Init(IProtoSystems systems)
        {
            // Debug.Log($"{GetType().Name} Init");
            systems
                .AddService(new Window_Service(), true)
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