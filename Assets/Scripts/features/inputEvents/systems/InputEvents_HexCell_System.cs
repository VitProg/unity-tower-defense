using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using td.features.camera;
using td.features.level;
using td.features.movement;
using td.features.tower;
using td.utils;
using Unity.Mathematics;
using UnityEngine;

namespace td.features.inputEvents.systems
{
    public class InputEvents_HexCell_System : IProtoRunSystem
    {
        [DI] private InputEvents_Aspect aspect;
        [DI] private InputEvents_Service peService;
        [DI] private Movement_Service movementService;
        [DI] private Camera_Service cameraService;
        [DI] private Tower_Service towerService;
        [DI] private LevelMap levelMap;

        private int2? lastHoveredCoord;
        private bool lastPressed = false;

        public void Run()
        {
            //todo
            var pointerPosition = (Vector2)CameraUtils.TransformPointToCameraSpace(cameraService.GetMainCamera(), Input.mousePosition);
            var pointerCellCoord = HexGridUtils.PositionToCell(pointerPosition);

            var mouseButtonLeft = Input.GetMouseButton(0);
            var mouseButtonLeftDown = Input.GetMouseButtonDown(0);
            var mouseButtonLeftUp = Input.GetMouseButtonUp(0);

            // var mouseButtonLeftIsPressed = (mouseButtonLeftDown || mouseButtonLeft);
            // var mouseButtonLeftNotIsPressed = (mouseButtonLeftUp || !mouseButtonLeft);

            Touch? touch = Input.touchCount == 1 ? Input.GetTouch(0) : null;
            var hasTouch = touch.HasValue;
            var touchDown = touch is { phase: TouchPhase.Began };
            var touchUp = touch is { phase: TouchPhase.Ended };
            Vector2? touchPosition = hasTouch
                ? (Vector2)CameraUtils.TransformPointToCameraSpace(cameraService.GetMainCamera(), touch.Value.position)
                : null;
            int2? touchCellCoord = hasTouch ? HexGridUtils.PositionToCell(touchPosition.Value) : null;

            var down = mouseButtonLeftDown || touchDown;
            var up = mouseButtonLeftUp || touchUp;
            var pressed = mouseButtonLeft;

            var cellCoord = hasTouch ? touchCellCoord.Value : pointerCellCoord;

            if (lastHoveredCoord.HasValue && cellCoord.Equals(lastHoveredCoord.Value) && lastPressed == pressed)
            {
                if (levelMap.HasCell(cellCoord))
                {
                    ref var cell = ref levelMap.GetCell(cellCoord);
                    foreach (var handler in cell.inputEventsHandlers)
                    {
                        if (handler.IsPressed/* && handler.TimeFromDown < Constants.UI.LongClickTime*/)
                        {
                            handler.TimeFromDown += Time.deltaTime;
                            // Debug.Log(handler.TimeFromDown);
                        }
                    }
                }

                return;
            }

            lastPressed = pressed;

            if (lastHoveredCoord.HasValue)
            {
                ref var lastHoveredCell = ref levelMap.GetCell(lastHoveredCoord.Value);
                if (!lastHoveredCell.coords.Equals(cellCoord))
                {
                    foreach (var handler in lastHoveredCell.inputEventsHandlers)
                    {
                        Process(
                            false,
                            handler,
                            hasTouch ? touchPosition.Value.x : pointerPosition.x,
                            hasTouch ? touchPosition.Value.y : pointerPosition.y,
                            down,
                            up
                        );
                    }
                }
                lastHoveredCoord = null;
            }

            if (levelMap.HasCell(cellCoord)) {
                ref var cell = ref levelMap.GetCell(cellCoord);
                foreach (var handler in cell.inputEventsHandlers)
                {
                    Process(
                        true,
                        handler,
                        hasTouch ? touchPosition.Value.x : pointerPosition.x,
                        hasTouch ? touchPosition.Value.y : pointerPosition.y,
                        mouseButtonLeftDown || touchDown,
                        mouseButtonLeftUp || touchUp
                    );
                }
                lastHoveredCoord = cellCoord;
            }
        }

        private static void Process(
            bool inCell,
            IInputEventsHandler handler,
            float x,
            float y,
            bool down,
            bool up
        )
        {
            if (inCell && !handler.IsHovered)
            {
                handler.IsHovered = true;
                handler.TimeFromDown = 0f;
                handler.OnPointerEnter(x, y);
                // Debug.Log("OnPointerEnter");
            }

            if (!inCell && handler.IsHovered)
            {
                handler.IsHovered = false;
                handler.TimeFromDown = 0f;
                handler.OnPointerLeave(x, y);
                // Debug.Log("OnPointerLeave");
            }

            if (inCell && !handler.IsPressed && down)
            {
                handler.IsPressed = true;
                handler.TimeFromDown = 0f;
                handler.OnPointerDown(x, y);
                // Debug.Log("OnPointerDown");
            }

            if (handler.IsPressed && up)
            {
                handler.IsPressed = false;
                handler.OnPointerUp(x, y, inCell);
                // Debug.Log("OnPointerUp");
                if (inCell)
                {
                    handler.OnPointerClick(x, y, handler.TimeFromDown > Constants.UI.LongClickTime);
                }
                handler.TimeFromDown = 0f;
            }
        }
    }
}