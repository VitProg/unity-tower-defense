using Leopotam.EcsLite;
using td.common;
using td.utils;
using td.utils.ecs;
using UnityEngine;

namespace td.features.camera
{
    public class CameraZoomSystem : IEcsRunSystem, IEcsInitSystem
    {
        [InjectShared] private SharedData shared;
        
        private float zoom = 0;
        private float perspectiveZoomOnePercent;

        public void Run(IEcsSystems systems)
        {
            var mouseZoom = 0f;

            if (Input.GetKeyUp(KeyCode.Plus) || Input.GetKeyUp(KeyCode.KeypadPlus))
            {
                mouseZoom = -(shared.IsPerspectiveCameraMode ? Constants.Camera.PerspectiveZoomStep : Constants.Camera.OrthographicZoomStep);
            }

            if (Input.GetKeyUp(KeyCode.Minus) || Input.GetKeyUp(KeyCode.KeypadMinus))
            {
                mouseZoom = shared.IsPerspectiveCameraMode ? Constants.Camera.PerspectiveZoomStep : Constants.Camera.OrthographicZoomStep;
            }
            
            if (shared.virtualCamera != null) {

                if (shared.IsPerspectiveCameraMode)
                {
                    zoom += mouseZoom;
                    zoom = Mathf.Clamp(zoom, Constants.Camera.MinPerspectiveZoom, Constants.Camera.MaxPerspectiveZoom);
                    
                    var cameraTransform = shared.virtualCamera.transform;
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
                    shared.virtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(
                        shared.virtualCamera.m_Lens.OrthographicSize,
                        zoom,
                        EasingUtils.EaseOutSine(Time.deltaTime * Constants.Camera.OrthographicZoomSpeed)
                    );
                }
            }
        }

        public void Init(IEcsSystems systems)
        {
            if (shared.virtualCamera != null)
            {
                zoom = shared.IsPerspectiveCameraMode ?
                    shared.virtualCamera.transform.position.z :
                    shared.virtualCamera.m_Lens.OrthographicSize;
            }
        }
    }
}