using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
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
#if UNITY_EDITOR
                        if (mainCamera != null) throw new Exception($"На сцене не может быть более одной основной камеры");
#endif
                        mainCamera = camera;
                    }

                    if (data.renderType == CameraRenderType.Overlay)
                    {
#if UNITY_EDITOR
                        if (canvasCamera != null) throw new Exception($"На сцене не может быть более одной overlay/canvas камеры");
#endif
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
            
#if UNITY_EDITOR
            if (mainCamera == null) throw new Exception($"На сцене не найдена основная камера");
            if (canvasCamera == null) throw new Exception($"На сцене не найдена overlay/canvas камера");
            if (virtualCamera == null) throw new Exception($"На сцене не найдена CinemachineVirtualCamera");
            if (canvas == null) throw new Exception($"На сцене не найден главный Canvas");
#endif
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public Vector2 MainToCanvas(Vector2 point)
        {
            var viewportPoint = MainToViewport(point);
            return ViewportToCanves(viewportPoint);
        }
        
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public Vector2 CanvasToMain(Vector2 point)
        {
            var viewportPoint = CanvasToViewport(point);
            return ViewportToMain(viewportPoint);
        }
        
        
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public Vector2 MainToScreen(Vector2 point)
        {
            return mainCamera.WorldToScreenPoint(point);
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public Vector2 CanvesToScreen(Vector2 point)
        {
            return canvasCamera.WorldToScreenPoint(point);
        }    
        
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public Vector2 ScreenToMain(Vector2 point)
        {
            return mainCamera.ScreenToWorldPoint(point);
        }
        
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public Vector2 ScreenToCanves(Vector2 point)
        {
            return canvasCamera.ScreenToWorldPoint(point);
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public Vector2 MainToViewport(Vector2 point)
        {
            return mainCamera.WorldToViewportPoint(point);
        }
        
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public Vector2 CanvasToViewport(Vector2 point)
        {
            return canvasCamera.WorldToViewportPoint(point);
        }   
        
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public Vector2 ViewportToMain(Vector2 point)
        {
            return mainCamera.ViewportToWorldPoint(point);
        }
        
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public Vector2 ViewportToCanves(Vector2 point)
        {
            return canvasCamera.ViewportToWorldPoint(point);
        }

        // private List<RaycastResult> raycastResults = new(16);
        public void GetAllCanvasElementsByScreenCoords(in PointerEventData pointerData, in List<RaycastResult> raycastResults)
        {
            raycastResults.Clear();
            EventSystem.current.RaycastAll(pointerData, raycastResults);
        }
    }
}