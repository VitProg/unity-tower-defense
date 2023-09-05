using System;
using System.Collections.Generic;
using Leopotam.EcsProto.QoL;
using td.features._common.components;
using td.features._common.interfaces;
using td.features.inputEvents.components;
using td.utils.ecs;
using UnityEngine;
using UnityEngine.EventSystems;

namespace td.features.inputEvents
{
    public class InputEvents_Service
    {
        [DI] private InputEvents_Aspect aspect;

        // public bool HasCicleCollider(int entity) => aspect.cicleColliderPool.Has(entity);
        // public ref CicleCollider GetCicleCollider(int entity) => ref aspect.cicleColliderPool.GetOrAdd(entity);
        // public void DelCicleCollider(int entity) => aspect.cicleColliderPool.Del(entity);

        // public bool HasHandlers(int entity) => aspect.refPointerHandlersPool.Has(entity) &&
                                               // aspect.refPointerHandlersPool.Get(entity).references != null &&
                                               // aspect.refPointerHandlersPool.Get(entity).count > 0;
        
        // public ref HexCellCollider GetHexCellCollider(int entity) => ref aspect.hexCellColliderPool.GetOrAdd(entity);

        public void AddHandler(int entity, IInputEventsHandler handler)
        {
            ref var many = ref GetRefHandlers(entity);
            if (many.references == null) many.references = new IInputEventsHandler[5];

            var contains = false;
            foreach (var reference in many.references)
            {
                if (reference != handler) continue;
                contains = true;
                break;
            }
            
            if (!contains)
            {
                if (many.references.Length < many.count + 1)
                {
                    Array.Resize(ref many.references, many.references.Length * 2);
                }
                many.references[many.count] = handler;
                many.count++;
            }
        }

        public void ClearHandler(int entity, IInputEventsHandler handler)
        {
            ref var many = ref GetRefHandlers(entity);
            if (many.references == null) many.references = new IInputEventsHandler[5];

            var length = many.references.Length;
            for (var index = 0; index < length; index++)
            {
                many.references[index] = null;
            }

            many.count = 0;
        }

        public ref RefMany<IInputEventsHandler> GetRefHandlers(int entity)
        {
            return ref aspect.refPointerHandlersPool.GetOrAdd(entity);
        }
        
        private PointerEventData pointerData = new(EventSystem.current);
        private List<RaycastResult> raycastResults = new(16);
        public void GetAllCanvasElementsByScreenCoords(in Vector2 screenCoords, in List<RaycastResult> raycastResults)
        {
            pointerData.position = screenCoords;
            raycastResults.Clear();
            EventSystem.current.RaycastAll(pointerData, raycastResults);
            // Debug.Log("EventSystem.current.currentSelectedGameObject = " + EventSystem.current.currentSelectedGameObject);
            // Debug.Log("EventSystem.current.firstSelectedGameObject = " + EventSystem.current.firstSelectedGameObject);
            
            var modules = RaycasterManager.GetRaycasters();
            var modulesCount = modules.Count;
            for (int i = 0; i < modulesCount; ++i)
            {
                var module = modules[i];
                if (module == null || !module.IsActive())
                    continue;

                module.Raycast(pointerData, raycastResults);
            }
            
        }


        public bool HasUIUnderScreenCoords(in Vector2 screenCoords)
        {
            // raycastResults.Clear();
            // GetAllCanvasElementsByScreenCoords(screenCoords, raycastResults);
            // return raycastResults.Count > 0;
            pointerData.position = screenCoords;
            var modules = RaycasterManager.GetRaycasters();
            var modulesCount = modules.Count;
            for (var i = 0; i < modulesCount; ++i)
            {
                var module = modules[i];
                if (module == null || !module.IsActive())
                    continue;

                module.Raycast(pointerData, raycastResults);
                if (raycastResults.Count > 0)
                {
                    raycastResults.Clear();
                    return true;
                }
            }

            return false;
        }
    }
}