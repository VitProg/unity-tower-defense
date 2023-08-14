using System;
using Cinemachine;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace td.features.camera
{
    public class Camera_Service
    {
        private readonly Camera mainCamera;
        private readonly Camera canvasCamera;
        private readonly CinemachineVirtualCamera virtualCamera;
        private readonly Canvas canvas;
        
        public bool IsPerspectiveCameraMode() =>
            virtualCamera && mainCamera && 
            !(virtualCamera.m_Lens.Orthographic || mainCamera.orthographic);

        public Camera GetMainCamera() => mainCamera;
        public Camera GetCanvasCamera() => canvasCamera;
        public CinemachineVirtualCamera GetVirtualCamera() => virtualCamera;
        public Canvas GetCanvas() => canvas;

        public Camera_Service()
        {
            var cameras = GameObject.FindGameObjectsWithTag(Constants.Tags.Camera);
            foreach (var cameraGO in cameras)
            {
                var camera = cameraGO.GetComponent<Camera>();
                if (camera)
                {
                    var data = camera.GetUniversalAdditionalCameraData();
                    if (data.renderType == CameraRenderType.Base)
                    {
                        if (mainCamera != null)
                            throw new Exception($"На сцене не может быть более одной основной камеры");
                        mainCamera = camera;
                    }

                    if (data.renderType == CameraRenderType.Overlay)
                    {
                        if (canvasCamera != null)
                            throw new Exception($"На сцене не может быть более одной overlay/canvas камеры");
                        canvasCamera = camera;
                    }
                }
                else 
                {
                    virtualCamera = cameraGO.GetComponent<CinemachineVirtualCamera>();
                }
            }

            var canvasGO = GameObject.FindGameObjectWithTag(Constants.Tags.MainCanvas);
            if (canvasGO)
            {
                canvas = canvasGO.GetComponent<Canvas>();
            }
            
            if (mainCamera == null) throw new Exception($"На сцене не найдена основная камера");
            if (canvasCamera == null) throw new Exception($"На сцене не найдена overlay/canvas камера");
            if (virtualCamera == null) throw new Exception($"На сцене не найдена CinemachineVirtualCamera");
            if (canvas == null) throw new Exception($"На сцене не найден главный Canvas");
        }
        

    }
}