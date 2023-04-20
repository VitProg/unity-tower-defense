using Cinemachine;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.common;
using td.components.refs;
using td.utils;
using td.utils.ecs;
using UnityEngine;

namespace td.features.camera
{
    public class CameraZoomSystem : IEcsRunSystem, IEcsInitSystem
    {
        [EcsShared] private SharedData shared;
        
        private float zoom = 0;
        
        public void Run(IEcsSystems systems)
        {
            var mouseZoom = 0;//Input.mouseScrollDelta.y;

            if (Input.GetKeyUp(KeyCode.Plus) || Input.GetKeyUp(KeyCode.KeypadPlus))
            {
                mouseZoom = -2;
            }

            if (Input.GetKeyUp(KeyCode.Minus) || Input.GetKeyUp(KeyCode.KeypadMinus))
            {
                mouseZoom = 2;
            }
            
            if (shared.VirtualCamera != null) {

                zoom += mouseZoom * Constants.Camera.ZoomIncreaseAmount;

                zoom = Mathf.Clamp(zoom, Constants.Camera.MaxZoom, Constants.Camera.MinZoom);

                shared.VirtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(
                    shared.VirtualCamera.m_Lens.OrthographicSize,
                    zoom,
                    EasingUtils.EaseOutSine(Time.deltaTime * Constants.Camera.ZoomSpeed)
                );
            }
        }

        public void Init(IEcsSystems systems)
        {
            if (shared.VirtualCamera != null) {
                zoom = shared.VirtualCamera.m_Lens.OrthographicSize;
            }
        }
    }
}