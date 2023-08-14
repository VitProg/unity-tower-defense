using System;
using Leopotam.EcsProto.QoL;
using td.features._common.components;
using td.features.inputEvents.components;
using td.utils.ecs;

namespace td.features.inputEvents
{
    public class InputEvents_Service
    {
        [DI] private InputEvents_Aspect aspect;

        public bool HasCicleCollider(int entity) => aspect.cicleColliderPool.Has(entity);
        public ref ObjectCicleCollider GetCicleCollider(int entity) => ref aspect.cicleColliderPool.GetOrAdd(entity);
        public void DelCicleCollider(int entity) => aspect.cicleColliderPool.Del(entity);

        public bool HasHandlers(int entity) => aspect.refPointerHandlersPool.Has(entity) &&
                                               aspect.refPointerHandlersPool.Get(entity).references != null &&
                                               aspect.refPointerHandlersPool.Get(entity).count > 0;

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

        public ref RefMany<IInputEventsHandler> GetRefHandlers(int entity)
        {
            return ref aspect.refPointerHandlersPool.GetOrAdd(entity);
        }
    }
}