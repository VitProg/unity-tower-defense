using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using td.features.camera;
using td.features.movement;
using td.utils;
using UnityEngine;

namespace td.features.inputEvents.systems
{
    public class InputEvents_CicleCollider_System : IProtoRunSystem
    {
        [DI] private InputEvents_Aspect aspect;
        [DI] private InputEvents_Service peService;
        [DI] private Movement_Service movementService;
        [DI] private Camera_Service cameraService;

        public void Run()
        {
            var pointerPosition = CameraUtils.TransformPointToCameraSpace(cameraService.GetMainCamera(), Input.mousePosition).ToFloat2();
            
            var mouseButtonLeft = Input.GetMouseButton(0);
            var mouseButtonLeftDown = Input.GetMouseButtonDown(0);
            var mouseButtonLeftUp = Input.GetMouseButtonUp(0);

            var mouseButtonLeftIsPressed = (mouseButtonLeftDown || mouseButtonLeft);
            var mouseButtonLeftNotIsPressed = (mouseButtonLeftUp || !mouseButtonLeft);

            Touch? touch = Input.touchCount == 1 ? Input.GetTouch(0) : null;
            var hasTouch = touch.HasValue;
            var touchDown = touch is { phase: TouchPhase.Began };
            var touchUp = touch is { phase: TouchPhase.Ended };
            Vector2? touchPosition = touch.HasValue ? (Vector2)CameraUtils.TransformPointToCameraSpace(cameraService.GetMainCamera(), touch.Value.position) : null;

            foreach (var entity in aspect.itCicleCollider)
            {
                var position = movementService.GetTransform(entity).position;
                var size = aspect.cicleColliderPool.Get(entity);
                var handlers = aspect.refPointerHandlersPool.Get(entity).references;

                foreach (var handler in handlers)
                {
                    if (handler == null) continue;

                    var inRadius = false;

                    if (FloatUtils.IsEquals(size.yScale, 1f))
                    {
                        var sqrDistanseToPointer = (pointerPosition - position).SqrMagnitude();
                        var sqrDistanseToTouch =
                            hasTouch ? (touchPosition.Value - position.ToVector2()).sqrMagnitude : float.MaxValue;
                        inRadius = sqrDistanseToPointer < size.sqrRadius || sqrDistanseToTouch < size.sqrRadius;
                    }
                    else
                    {
                        var dx = pointerPosition.x - position.x;
                        var dy = (pointerPosition.y - position.y) / size.yScale;
                        var sqrDistanseToPointer = dx * dx + dy * dy;
                        var sqrDistanseToTouch = float.MaxValue;
                        if (hasTouch)
                        {
                            dx = touchPosition.Value.x - position.x;
                            dy = (touchPosition.Value.y - position.y) / size.yScale;
                            sqrDistanseToTouch = dx * dx + dy * dy;
                        }
                        inRadius = sqrDistanseToPointer < size.sqrRadius || sqrDistanseToTouch < size.sqrRadius;
                    }

                    if (inRadius && !handler.IsHovered)
                    {
                        handler.IsHovered = true;
                        handler.OnPointerEnter(pointerPosition.x, pointerPosition.y);
                    }

                    if (!inRadius && handler.IsHovered)
                    {
                        handler.IsHovered = false;
                        handler.OnPointerLeave(pointerPosition.x, pointerPosition.y);
                    }

                    if (inRadius && !handler.IsPressed && (mouseButtonLeftDown || touchDown))
                    {
                        handler.IsPressed = true;
                        handler.OnPointerDown(pointerPosition.x, pointerPosition.y);
                    }

                    if (handler.IsPressed && (mouseButtonLeftUp || touchUp))
                    {
                        handler.IsPressed = false;
                        handler.OnPointerUp(pointerPosition.x, pointerPosition.y, inRadius);
                        if (inRadius) handler.OnPointerClick(pointerPosition.x, pointerPosition.y);
                    }

                    if (inRadius && !handler.IsPressed && (mouseButtonLeftUp || touchUp))
                    {
                        handler.IsPressed = false;
                        handler.OnPointerUp(pointerPosition.x, pointerPosition.y, true);
                    }
                }
            }
        }
    }
}