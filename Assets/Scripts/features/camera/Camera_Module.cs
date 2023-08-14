using Leopotam.EcsProto;
using td.utils.ecs;

namespace td.features.camera
{
    public class Camera_Module : IProtoModule
    {
        public void Init(IProtoSystems systems)
        {
            systems
                .AddSystem(new CameraZoomSystem())
                .AddSystem(new CameraMoveSystem())
                //
                .AddService(new Camera_Service(), true)
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