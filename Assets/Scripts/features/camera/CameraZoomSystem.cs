using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using Leopotam.Types;
using td.features.camera.bus;
using td.features.eventBus;
using td.utils;
using UnityEngine;
using UnityEngine.Rendering;

namespace td.features.camera
{
    public class CameraZoomSystem : IProtoRunSystem, IProtoInitSystem
    {
        [DI] private Camera_Service cameraService;
        [DI] private EventBus events;
        
        private float zoom = 0;
        private float perspectiveZoomOnePercent;
        private float lastZoom;

        public void Run()
        {
            var mouseZoom = 0f;
            var isPerspective = cameraService.IsPerspectiveCameraMode();

            if (Input.GetKeyUp(KeyCode.Plus) || Input.GetKeyUp(KeyCode.KeypadPlus))
            {
                mouseZoom = -(isPerspective ? Constants.Camera.PerspectiveZoomStep : Constants.Camera.OrthographicZoomStep);
            }

            if (Input.GetKeyUp(KeyCode.Minus) || Input.GetKeyUp(KeyCode.KeypadMinus))
            {
                mouseZoom = isPerspective ? Constants.Camera.PerspectiveZoomStep : Constants.Camera.OrthographicZoomStep;
            }

            if (isPerspective)
            {
                zoom += mouseZoom;
                zoom = MathFast.Clamp(zoom, Constants.Camera.MinPerspectiveZoom, Constants.Camera.MaxPerspectiveZoom);

                if (!FloatUtils.IsEquals(lastZoom, zoom))
                {
                    lastZoom = zoom;
                    var cameraTransform = cameraService.GetVirtualCamera().transform;
                    var cameraPosition = cameraTransform.position;
                    var position = new Vector3(
                        cameraPosition.x,
                        cameraPosition.y,
                        zoom
                    );
                    cameraTransform.position = Vector3.Lerp(
                        cameraPosition,
                        position,
                        EasingUtils.EaseOutQuad(Time.deltaTime * Constants.Camera.PerspectiveZoomSpeed)
                    );
                    events.unique.GetOrAdd<Event_Camera_Moved>();
                }
            }
            else
            {
                zoom += mouseZoom;
                zoom = MathFast.Clamp(zoom, Constants.Camera.MaxOrthographicZoom, Constants.Camera.MinOrthographicZoom);
                
                if (!FloatUtils.IsEquals(lastZoom, zoom))
                {
                    lastZoom = zoom;
                    cameraService.GetVirtualCamera().m_Lens.OrthographicSize = MathFast.Lerp(
                        cameraService.GetVirtualCamera().m_Lens.OrthographicSize,
                        zoom,
                        EasingUtils.EaseOutQuad(Time.deltaTime * Constants.Camera.OrthographicZoomSpeed)
                    );
                    events.unique.GetOrAdd<Event_Camera_Moved>();
                }
            }
        }

        public void Init(IProtoSystems systems)
        {
            zoom = cameraService.IsPerspectiveCameraMode() ?
                cameraService.GetVirtualCamera().transform.position.z :
                cameraService.GetVirtualCamera().m_Lens.OrthographicSize;
            lastZoom = zoom;
        }
    }
}