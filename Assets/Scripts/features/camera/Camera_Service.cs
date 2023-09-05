using System;
using System.Runtime.CompilerServices;
// using Cinemachine;
using Com.LuisPedroFonseca.ProCamera2D;
using td.utils;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace td.features.camera
{
    public class Camera_Service
    {
        private readonly Camera mainCamera;
        private readonly Camera canvasCamera;
        // private readonly CinemachineVirtualCamera virtualCamera;
        private readonly ProCamera2D proCamera2D;
        private readonly ProCamera2DNumericBoundaries numericBoundaries;
        private readonly ProCamera2DPanAndZoom panAndZoom;
        private readonly ProCamera2DShake shake;
        private readonly Canvas canvas;

        public bool IsPerspectiveCameraMode() => false;/*
            virtualCamera && mainCamera && 
            !(virtualCamera.m_Lens.Orthographic || mainCamera.orthographic);*/

        public Camera GetMainCamera() => mainCamera;
        public Camera GetCanvasCamera() => canvasCamera;
        // public CinemachineVirtualCamera GetVirtualCamera() => virtualCamera;
        public ProCamera2D GetProCamera2D() => proCamera2D;
        // public ProCamera2DPanAndZoom GetPanAndZoom() => panAndZoom;
        // public ProCamera2DNumericBoundaries GetNumericBoundaries() => numericBoundaries;
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
                        proCamera2D = cameraGO.GetComponent<ProCamera2D>();
                        numericBoundaries = proCamera2D.GetComponent<ProCamera2DNumericBoundaries>();
                        panAndZoom = proCamera2D.GetComponent<ProCamera2DPanAndZoom>();
                        shake = proCamera2D.GetComponent<ProCamera2DShake>();
                    }

                    if (data.renderType == CameraRenderType.Overlay)
                    {
#if UNITY_EDITOR
                        if (canvasCamera != null) throw new Exception($"На сцене не может быть более одной overlay/canvas камеры");
#endif
                        canvasCamera = camera;
                    }
                }
                // else 
                // {
                    // virtualCamera = cameraGO.GetComponent<CinemachineVirtualCamera>();
                // }
            }

            var canvasGO = GameObject.FindGameObjectWithTag(Constants.Tags.MainCanvas);
            if (canvasGO)
            {
                canvas = canvasGO.GetComponent<Canvas>();
            }

#if UNITY_EDITOR
            if (mainCamera == null) throw new Exception($"No main camera is found on the scene");
            if (canvasCamera == null) throw new Exception($"No overlay/canvas camera found on scene");
            // if (virtualCamera == null) throw new Exception($"На сцене не найдена CinemachineVirtualCamera");
            if (proCamera2D == null) throw new Exception($"No ProCamera2D found on scene");
            if (numericBoundaries == null) throw new Exception($"Numberic Boundaries extension is not attached to ProCamera2D");
            if (panAndZoom == null) throw new Exception($"Pan And Zoom extension is not attached to ProCamera2D");
            if (canvas == null) throw new Exception($"No main Canvas is found on the scene");
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
            var v = mainCamera.ScreenToWorldPoint(point);
            v.z = 0f;
            return v;
        }
        
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public Vector2 ScreenToCanves(Vector2 point, float z = 0f)
        {
            var p = canvasCamera.ScreenToWorldPoint(point);
            p.z = z;
            return p;
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
        
        //

        public void SetBoundingRect(float minX, float maxX, float minY, float maxY)
        {
            numericBoundaries.LeftBoundary = minX;
            numericBoundaries.RightBoundary = maxX;
            numericBoundaries.TopBoundary = maxY;
            numericBoundaries.BottomBoundary = minY;
        }
        public void SetBoundingRect(Rect rect)
        {
            numericBoundaries.LeftBoundary = rect.xMin;
            numericBoundaries.RightBoundary = rect.xMax;
            numericBoundaries.TopBoundary = rect.yMax;
            numericBoundaries.BottomBoundary = rect.yMin;
        }

        public void MoveTo(Vector2 position, bool immediatly)
        {
            if (immediatly)
            {
                proCamera2D.MoveCameraInstantlyToPosition(position);
            }
            else
            {
                // todo ApplyInfluence...
            }
        }

        public void Shake(ShakeType type)
        {
            shake.Shake(type.ToString());
        }
        
        public enum ShakeType
        {
            KernelDamage,
        }

        public void MutePanAndZoom()
        {
            panAndZoom.AllowPan = false;
            panAndZoom.AllowZoom = false;
        }

        public void ResumePanAndZoom()
        {
            panAndZoom.AllowPan = true;
            panAndZoom.AllowZoom = true;
        }
    }
}