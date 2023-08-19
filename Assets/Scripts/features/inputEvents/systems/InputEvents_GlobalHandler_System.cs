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
            if (Input.mousePresent) // todo add touch
            {
                var downLeft = Input.GetMouseButtonDown(0);
                var downRight = Input.GetMouseButtonDown(1);
                var upLeft = Input.GetMouseButtonUp(0);
                var upRight = Input.GetMouseButtonUp(1);

                if (!downLeft && !downRight && !upLeft && !upRight) return;
                
                var screePoint = (Vector2)Input.mousePosition;
                var isUI = inputEventsService.HasUIUnderScreenCoords(screePoint);
                    
                if (isUI) return;
                    
                if (Input.GetMouseButtonDown(0))
                {
                    ref var ev = ref events.global.Add<Event_PointerDown>();
                    ev.mouseButton = 0;
                    ev.x = screePoint.x;
                    ev.y = screePoint.y;
                }

                if (Input.GetMouseButtonDown(1))
                {
                    ref var ev = ref events.global.Add<Event_PointerDown>();
                    ev.mouseButton = 1;
                    ev.x = screePoint.x;
                    ev.y = screePoint.y;
                }

                if (Input.GetMouseButtonUp(0))
                {
                    ref var ev = ref events.global.Add<Event_PointerUp>();
                    ev.mouseButton = 0;
                    ev.x = screePoint.x;
                    ev.y = screePoint.y;
                }

                if (Input.GetMouseButtonUp(1))
                {
                    ref var ev = ref events.global.Add<Event_PointerUp>();
                    ev.mouseButton = 1;
                    ev.x = screePoint.x;
                    ev.y = screePoint.y;
                }
            }
        }
    }
}