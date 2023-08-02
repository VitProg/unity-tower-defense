using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Leopotam.EcsLite;
using td.features._common.components;
using td.utils.ecs;
using UnityEngine.EventSystems;

namespace td.features.inputEvents
{
    public class InputEvents_Service
    {
        private readonly EcsInject<InputEvents_Pools> pool;

        public bool HasCicleCollider(int entity) => pool.Value.cicleColliderPool.Value.Has(entity);
        public ref ObjectCicleCollider GetCicleCollider(int entity) => ref pool.Value.cicleColliderPool.Value.GetOrAdd(entity);
        public void DelCicleCollider(int entity) => pool.Value.cicleColliderPool.Value.SafeDel(entity);
        
        public bool HasHandlers(int entity) => pool.Value.refPointerHandlers.Value.Has(entity) && pool.Value.refPointerHandlers.Value.Get(entity).references != null && pool.Value.refPointerHandlers.Value.Get(entity).count > 0;

        public void AddHandler(int entity, IInputEventsHandler handler)
        {
            ref var many = ref GetRefHandlers(entity);
            if (many.references == null) many.references = new IInputEventsHandler[5];

            if (!many.references.Contains(handler))
            {
                if (many.references.Length < many.count + 1)
                {
                    Array.Resize(ref many.references, many.references.Length * 2);
                }
                many.references[many.count] = handler;
                many.count++;
            }
        }
        // public void RemoveHandler(int entity, IInputEventsHandler handler)
        // {
        //     if (GetRefHandlers(entity).references == null) GetRefHandlers(entity).references = new List<IInputEventsHandler>();
        //     if (GetRefHandlers(entity).references.Contains(handler)) GetRefHandlers(entity).references.Remove(handler);
        // }
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
        public ref RefMany<IInputEventsHandler> GetRefHandlers(int entity) => ref pool.Value.refPointerHandlers.Value.GetOrAdd(entity);
    }
}