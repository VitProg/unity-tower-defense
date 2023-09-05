using System.Collections.Generic;
using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using td.features.camera;
using td.features.eventBus;
using td.features.inputEvents.bus;
using UnityEngine;
using UnityEngine.EventSystems;

namespace td.features.inputEvents.systems
{
    public class InputEvents_GlobalHandler_System : IProtoRunSystem
    {
        [DI] private EventBus events;
        [DI] private Camera_Service cameraService;
        [DI] private InputEvents_Service inputEventsService;

        private List<RaycastResult> raycastResults = new(16);
        private PointerEventData pointerData = new(EventSystem.current);

        public void Run()
        {
            if (Input.touchSupported)
            {
                Touch? touch = Input.touchCount == 1 ? Input.GetTouch(0) : null;
                var hasTouch = touch.HasValue;
                var touchDown = touch is { phase: TouchPhase.Began };
                var touchUp = touch is { phase: TouchPhase.Ended };
                
                if (!touchDown && !touchUp) return;
                
                var touchScreenPosition = touch.Value.position;
                
                var isUI = inputEventsService.HasUIUnderScreenCoords(touchScreenPosition);

                if (!isUI)
                {
                    if (touchDown)
                    {
                        ref var ev = ref events.global.Add<Event_PointerDown>();
                        ev.isTouch = true;
                        ev.x = touchScreenPosition.x;
                        ev.y = touchScreenPosition.y;
                        Debug.Log(ev);
                    }

                    if (touchUp)
                    {
                        ref var ev = ref events.global.Add<Event_PointerUp>();
                        ev.isTouch = true;
                        ev.x = touchScreenPosition.x;
                        ev.y = touchScreenPosition.y;
                        Debug.Log(ev);
                    }
                }
            }
            
            if (Input.mousePresent)
            {
                var downLeft = Input.GetMouseButtonDown(0);
                var downRight = Input.GetMouseButtonDown(1);
                var upLeft = Input.GetMouseButtonUp(0);
                var upRight = Input.GetMouseButtonUp(1);

                if (downLeft || downRight || upLeft || upRight)  {
                    var screenPoint = (Vector2)Input.mousePosition;
                    var isUI = inputEventsService.HasUIUnderScreenCoords(screenPoint);

                    if (!isUI)
                    {
                        if (downLeft || downRight)
                        {
                            ref var ev = ref events.global.Add<Event_PointerDown>();
                            ev.mouseButton = (byte)(downLeft ? 0 : 1);
                            ev.x = screenPoint.x;
                            ev.y = screenPoint.y;
                            Debug.Log(ev);
                        }

                        if (upLeft)
                        {
                            ref var ev = ref events.global.Add<Event_PointerUp>();
                            ev.mouseButton = (byte)(downLeft ? 0 : 1);
                            ev.x = screenPoint.x;
                            ev.y = screenPoint.y;
                            Debug.Log(ev);
                        }
                    }
                }
            }
        }
    }
}