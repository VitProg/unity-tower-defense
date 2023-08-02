using Leopotam.EcsLite;
using td.common;
using td.features._common;
using td.utils;
using UnityEngine;

namespace td.features.camera
{
    public class CameraZoomSystem : IEcsRunSystem, IEcsInitSystem
    {
        private readonly EcsInject<SharedData> shared;
        
        private float zoom = 0;
        private float perspectiveZoomOnePercent;

        public void Run(IEcsSystems systems)
        {
            var mouseZoom = 0f;

            if (Input.GetKeyUp(KeyCode.Plus) || Input.GetKeyUp(KeyCode.KeypadPlus))
            {
                mouseZoom = -(shared.Value.IsPerspectiveCameraMode ? Constants.Camera.PerspectiveZoomStep : Constants.Camera.OrthographicZoomStep);
            }

            if (Input.GetKeyUp(KeyCode.Minus) || Input.GetKeyUp(KeyCode.KeypadMinus))
            {
                mouseZoom = shared.Value.IsPerspectiveCameraMode ? Constants.Camera.PerspectiveZoomStep : Constants.Camera.OrthographicZoomStep;
            }
            
            if (shared.Value.virtualCamera != null) {

                if (shared.Value.IsPerspectiveCameraMode)
                {
                    zoom += mouseZoom;
                    zoom = Mathf.Clamp(zoom, Constants.Camera.MinPerspectiveZoom, Constants.Camera.MaxPerspectiveZoom);
                    
                    var cameraTransform = shared.Value.virtualCamera.transform;
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
                    shared.Value.virtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(
                        shared.Value.virtualCamera.m_Lens.OrthographicSize,
                        zoom,
                        EasingUtils.EaseOutSine(Time.deltaTime * Constants.Camera.OrthographicZoomSpeed)
                    );
                }
            }
        }

        public void Init(IEcsSystems systems)
        {
            if (shared.Value.virtualCamera != null)
            {
                zoom = shared.Value.IsPerspectiveCameraMode ?
                    shared.Value.virtualCamera.transform.position.z :
                    shared.Value.virtualCamera.m_Lens.OrthographicSize;
            }
        }
    }
}