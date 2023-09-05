using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using td.features.camera;
using td.features.movement;
using td.utils;
using Unity.Mathematics;
using UnityEngine;

namespace td.features.inputEvents.systems
{
    public class InputEvents_HexCellCollider_System : IProtoRunSystem
    {
        [DI] private InputEvents_Aspect aspect;
        [DI] private InputEvents_Service peService;
        [DI] private Movement_Service movementService;
        [DI] private Camera_Service cameraService;

        public void Run()
        {
            //todo
            var pointerPosition = (Vector2)CameraUtils.TransformPointToCameraSpace(cameraService.GetMainCamera(), Input.mousePosition);
            var pointerCell = HexGridUtils.PositionToCell(pointerPosition);
            
            var mouseButtonLeft = Input.GetMouseButton(0);
            var mouseButtonLeftDown = Input.GetMouseButtonDown(0);
            var mouseButtonLeftUp = Input.GetMouseButtonUp(0);

            var mouseButtonLeftIsPressed = (mouseButtonLeftDown || mouseButtonLeft);
            var mouseButtonLeftNotIsPressed = (mouseButtonLeftUp || !mouseButtonLeft);

            Touch? touch = Input.touchCount == 1 ? Input.GetTouch(0) : null;
            var hasTouch = touch.HasValue;
            var touchDown = touch is { phase: TouchPhase.Began };
            var touchUp = touch is { phase: TouchPhase.Ended };
            Vector2? touchPosition = hasTouch ? (Vector2)CameraUtils.TransformPointToCameraSpace(cameraService.GetMainCamera(), touch.Value.position) : null;
            int2? touchCell = hasTouch ? HexGridUtils.PositionToCell(touchPosition.Value) : null;

            // todo use chached iterator
            foreach (var entity in aspect.itHexCellCollider)
            {
                var cell = aspect.hexCellColliderPool.Get(entity).coords;
                var handlers = aspect.refPointerHandlersPool.Get(entity).references;

                foreach (var handler in handlers)
                {
                    if (handler == null) continue;

                    var inCell = cell.Equals(pointerCell) || (hasTouch && cell.Equals(touchCell.Value));

                    if (inCell && !handler.IsHovered)
                    {
                        handler.IsHovered = true;
                        handler.OnPointerEnter(pointerPosition.x, pointerPosition.y);
                    }

                    if (!inCell && handler.IsHovered)
                    {
                        handler.IsHovered = false;
                        handler.OnPointerLeave(pointerPosition.x, pointerPosition.y);
                    }

                    if (inCell && !handler.IsPressed && (mouseButtonLeftDown || touchDown))
                    {
                        handler.IsPressed = true;
                        handler.OnPointerDown(pointerPosition.x, pointerPosition.y);
                    }

                    if (handler.IsPressed && (mouseButtonLeftUp || touchUp))
                    {
                        handler.IsPressed = false;
                        handler.OnPointerUp(pointerPosition.x, pointerPosition.y, inCell);
                        if (inCell) handler.OnPointerClick(pointerPosition.x, pointerPosition.y);
                    }

                    if (inCell && !handler.IsPressed && (mouseButtonLeftUp || touchUp))
                    {
                        handler.IsPressed = false;
                        handler.OnPointerUp(pointerPosition.x, pointerPosition.y, true);
                    }
                }
            }
        }
    }
}