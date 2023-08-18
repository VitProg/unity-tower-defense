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
    public class InputEvent_GlobalHandler_System : IProtoRunSystem
    {
        [DI] private EventBus events;
        [DI] private Camera_Service cameraService;

        private List<RaycastResult> raycastResults = new(16);
        private PointerEventData pointerData = new(EventSystem.current);
        
        public void Run()
        {
            if (Input.mousePresent)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    ref var ev = ref events.global.Add<Event_PointerDown>();
                    ev.mouseButton = 0;
                    ev.x = Input.mousePosition.x;
                    ev.y = Input.mousePosition.y;
                    pointerData.pointerId = -1;
                    pointerData.position = Input.mousePosition;
                    cameraService.GetAllCanvasElementsByScreenCoords(pointerData, raycastResults);
                    Debug.Log(raycastResults);
                    
                }
                if (Input.GetMouseButtonDown(1))
                {
                    ref var ev = ref events.global.Add<Event_PointerDown>();
                    ev.mouseButton = 1;
                    ev.x = Input.mousePosition.x;
                    ev.y = Input.mousePosition.y;
                }
            }
            else
            {
                if (Input.GetMouseButtonUp(0))
                {
                    ref var ev = ref events.global.Add<Event_PointerUp>();
                    ev.mouseButton = 0;
                    ev.x = Input.mousePosition.x;
                    ev.y = Input.mousePosition.y;
                }
                if (Input.GetMouseButtonUp(1))
                {
                    ref var ev = ref events.global.Add<Event_PointerUp>();
                    ev.mouseButton = 1;
                    ev.x = Input.mousePosition.x;
                    ev.y = Input.mousePosition.y;
                }
            }
        }
    }
}