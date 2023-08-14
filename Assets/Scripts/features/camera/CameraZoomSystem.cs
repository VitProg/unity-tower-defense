using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using td.features._common;
using td.utils;
using UnityEngine;

namespace td.features.camera
{
    public class CameraZoomSystem : IProtoRunSystem, IProtoInitSystem
    {
        [DI] private Camera_Service cameraService;
        
        private float zoom = 0;
        private float perspectiveZoomOnePercent;

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
                zoom = Mathf.Clamp(zoom, Constants.Camera.MinPerspectiveZoom, Constants.Camera.MaxPerspectiveZoom);
                
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
                    EasingUtils.EaseOutSine(Time.deltaTime * Constants.Camera.PerspectiveZoomSpeed)
                );
            }
            else
            {
                zoom += mouseZoom;
                zoom = Mathf.Clamp(zoom, Constants.Camera.MaxOrthographicZoom, Constants.Camera.MinOrthographicZoom);
                cameraService.GetVirtualCamera().m_Lens.OrthographicSize = Mathf.Lerp(
                    cameraService.GetVirtualCamera().m_Lens.OrthographicSize,
                    zoom,
                    EasingUtils.EaseOutSine(Time.deltaTime * Constants.Camera.OrthographicZoomSpeed)
                );
            }
        }

        public void Init(IProtoSystems systems)
        {
            zoom = cameraService.IsPerspectiveCameraMode() ?
                cameraService.GetVirtualCamera().transform.position.z :
                cameraService.GetVirtualCamera().m_Lens.OrthographicSize;
        }
    }
}