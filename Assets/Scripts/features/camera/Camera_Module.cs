using System;
using Leopotam.EcsProto;
using td.features.camera.bus;
using td.features.eventBus;
using td.utils.ecs;

namespace td.features.camera
{
    public class Camera_Module : IProtoModuleWithEvents
    {
        public void Init(IProtoSystems systems)
        {
            systems
                // .AddSystem(new CameraZoomSystem())
                // .AddSystem(new CameraMoveSystem())
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

        public Type[] Events() => Ev.E<Event_Camera_Moved>();
    }
}