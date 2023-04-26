using Leopotam.EcsLite;
using td.common;
using td.utils;
using td.utils.ecs;
using UnityEngine;

namespace td.features.camera
{
    public class CameraMoveSystem : IEcsRunSystem, IEcsInitSystem
    {
        [InjectShared] private SharedData shared;

        private Vector3 startCursorScreenPosition;
        private Vector3 lastCursorScreenPosition;
        private Vector3 startCameraPosition;
        private Vector3 currentCameraPosition;

        private float mouseTime;
        private float keyboardTimeX;
        private float keyboardTimeY;

        private Vector2 targetPosition;
        private Vector2 mouseVector = Vector2.zero;
        private Vector2 keyboardVector = Vector2.zero;

        private Camera camera;

        public void Init(IEcsSystems systems)
        {
            camera = Camera.main;
            Debug.Assert(camera != null, "Main Camera Is Null");
        }

        public void Run(IEcsSystems systems)
        {
            if (shared.VirtualCamera == null) return;

            ///// MOUSE ////
            var cursorScreenPosition = Input.mousePosition;

            var mouseInertia = mouseVector.sqrMagnitude > 0.0001f;
            if (Input.GetMouseButtonDown(1))
            {
                lastCursorScreenPosition = cursorScreenPosition;
                mouseTime = 0;
            }

            if (Input.GetMouseButton(1))
            {
                mouseVector = lastCursorScreenPosition - cursorScreenPosition;
                mouseVector *= Constants.Camera.MoveSpeedMouse * Time.deltaTime;
                mouseInertia = false;
            }

            if (Input.GetMouseButtonUp(1))
            {
                mouseVector = lastCursorScreenPosition - cursorScreenPosition;
                mouseVector *= Constants.Camera.MoveSpeedMouse * Time.deltaTime;
                mouseTime = 0;
            }

            mouseVector = Vector2.ClampMagnitude(mouseVector, Constants.Camera.MaxMoveSpeedMouse);
            lastCursorScreenPosition = cursorScreenPosition;

            ///// KEYBOARD ////
            var keyboardInertiaX = Mathf.Abs(keyboardVector.x) > 0.0001f;
            var keyboardInertiaY = Mathf.Abs(keyboardVector.y) > 0.0001f;
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            {
                keyboardVector.y = Constants.Camera.MoveSpeedKeyborad * Time.deltaTime;
                keyboardInertiaY = false;
                keyboardTimeY = 0;
            }

            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            {
                keyboardVector.y = -Constants.Camera.MoveSpeedKeyborad * Time.deltaTime;
                keyboardInertiaY = false;
                keyboardTimeY = 0;
            }

            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                keyboardVector.x = -Constants.Camera.MoveSpeedKeyborad * Time.deltaTime;
                keyboardInertiaX = false;
                keyboardTimeX = 0;
            }

            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                keyboardVector.x = Constants.Camera.MoveSpeedKeyborad * Time.deltaTime;
                keyboardInertiaX = false;
                keyboardTimeX = 0;
            }

            //////////////////

            if (mouseVector.sqrMagnitude > 0.0001f || keyboardVector.sqrMagnitude > 0.0001f)
            {
                mouseTime += Time.deltaTime;

                var speed = (mouseVector.magnitude + keyboardVector.magnitude) *
                            (shared.VirtualCamera.m_Lens.OrthographicSize / Constants.Camera.MinOrthographicZoom) *
                            (Mathf.Max(Screen.width, Screen.height) / 1000f);

                speed = Mathf.Clamp(speed, 0, Constants.Camera.MaxMoveSpeed);

                // Debug.Log($"Speed: {speed}, MVector: {mouseVector}, KVector: {keyboardVector}");

                var vector = mouseVector + keyboardVector;
                vector.Normalize();
                vector *= speed;

                /////

                targetPosition = camera.transform.position + (Vector3)vector;
                currentCameraPosition = targetPosition;
                currentCameraPosition.z = camera.transform.position.z;

                var pos = currentCameraPosition;

                pos.z = camera.transform.position.z;

                shared.VirtualCamera.ForceCameraPosition(pos, camera.transform.rotation);

                if (mouseInertia)
                {
                    mouseVector = Vector2.Lerp(
                        mouseVector,
                        Vector2.zero,
                        EasingUtils.EaseOutQuad(mouseTime * Constants.Camera.MoveMouseInertiatiaAttenuation)
                    );
                }

                if (keyboardInertiaX)
                {
                    keyboardTimeX += Time.deltaTime;
                    keyboardVector.x = Mathf.Lerp(
                        keyboardVector.x,
                        0,
                        EasingUtils.EaseOutQuad(keyboardTimeX * Constants.Camera.MoveKeyboardInertiatiaAttenuation)
                    );
                }

                if (keyboardInertiaY)
                {
                    keyboardTimeY += Time.deltaTime;
                    keyboardVector.y = Mathf.Lerp(
                        keyboardVector.y,
                        0,
                        EasingUtils.EaseOutQuad(keyboardTimeY * Constants.Camera.MoveKeyboardInertiatiaAttenuation)
                    );
                }

                if (mouseInertia || keyboardInertiaX || keyboardInertiaY)
                {
                    if ((mouseVector + keyboardVector).sqrMagnitude < 0.00001f)
                    {
                        if (mouseInertia) mouseVector = Vector2.zero;
                        if (keyboardInertiaX) keyboardVector.x = 0f;
                        if (keyboardInertiaY) keyboardVector.y = 0f;
                    }
                }
            }
        }
    }
}