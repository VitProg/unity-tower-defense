using Leopotam.EcsLite;
using td.features._common;
using td.utils;
using UnityEngine;
using UnityEngine.Networking;

namespace td.features.inputEvents
{
    public class InputEvents_System : IEcsRunSystem
    {
        private readonly EcsInject<InputEvents_Pools> pools;
        private readonly EcsInject<InputEvents_Service> peService;
        private readonly EcsInject<SharedData> shared;

        public void Run(IEcsSystems systems)
        {
            var pointerPosition = (Vector2)CameraUtils.ToWorldPoint(shared.Value.mainCamera, Input.mousePosition);
            
            var mouseButtonLeft = Input.GetMouseButton(0);
            var mouseButtonLeftDown = Input.GetMouseButtonDown(0);
            var mouseButtonLeftUp = Input.GetMouseButtonUp(0);

            var mouseButtonLeftIsPressed = (mouseButtonLeftDown || mouseButtonLeft);
            var mouseButtonLeftNotIsPressed = (mouseButtonLeftUp || !mouseButtonLeft);

            Touch? touch = Input.touchCount == 1 ? Input.GetTouch(0) : null;
            var hasTouch = touch.HasValue;
            var touchDown = touch is { phase: TouchPhase.Began };
            var touchUp = touch is { phase: TouchPhase.Ended };
            Vector2? touchPosition = touch.HasValue ? (Vector2)CameraUtils.ToWorldPoint(shared.Value.mainCamera, touch.Value.position) : null;

            foreach (var entity in pools.Value.filter.Value)
            {
                var position = pools.Value.filter.Pools.Inc1.Get(entity).position;
                var size = pools.Value.filter.Pools.Inc2.Get(entity);
                var handlers = pools.Value.filter.Pools.Inc3.Get(entity).references;

                foreach (var handler in handlers)
                {
                    if (handler == null) continue;

                    var inRadius = false;

                    if (FloatUtils.IsEquals(size.yScale, 1f))
                    {
                        var sqrDistanseToPointer = (pointerPosition - position).sqrMagnitude;
                        var sqrDistanseToTouch =
                            hasTouch ? (touchPosition.Value - position).sqrMagnitude : float.MaxValue;
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