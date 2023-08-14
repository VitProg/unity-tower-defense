using System;
using System.Collections.Generic;
using Leopotam.EcsProto.QoL;
using Leopotam.EcsProto.Unity;
using td.features.eventBus.types;
using td.utils.ecs;
using UnityEngine;

namespace td.features.eventBus
{
    public static class ProtoModulesExtensions
    {
        private static readonly Type EventType = typeof(IEvent);
        
        public static List<Type> BuildEvents(this ProtoModulesEx self)
        {
            var events = new List<Type>(16);
            var modules = self.AllModules();
            foreach (var module in modules)
            {
                if (module is IProtoModuleWithEvents eventsModule)
                {
                    var eventsInModule = eventsModule.Events();
                    foreach (var evType in eventsInModule)
                    {
                        var hasEventInterface = false;
                        foreach (var i in evType.GetInterfaces())
                        {
                            if (i != EventType) continue;
                            hasEventInterface = true;
                            break;
                        }
                        if (!hasEventInterface)
                        {
                            throw new Exception($"Failed to add the {EditorExtensions.GetCleanTypeName(evType)} event because it does not implement the IEvent interface");
                        }
                        if (events.Contains(evType))
                        {
                            throw new Exception($"Failed to add the {EditorExtensions.GetCleanTypeName(evType)} event because it is already registered");
                        }
                        events.Add(evType);
                        // Debug.Log("- добавленно событие " + evType.Name);
                    }
                }
            }

            return events;
        }
    }

    public static class Ev
    {
        public static Type[] E<T>() 
            where T : IEvent =>
            new []
            {
                typeof(T),
            };

        public static Type[] E<T1, T2>() 
            where T1 : IEvent
            where T2 : IEvent =>
            new []
            {
                typeof(T1),
                typeof(T2),
            };
        
        public static Type[] E<T1, T2, T3>() 
            where T1 : IEvent
            where T3 : IEvent
            where T2 : IEvent =>
            new []
            {
                typeof(T1),
                typeof(T2),
                typeof(T3),
            };
        
        public static Type[] E<T1, T2, T3, T4>() 
            where T1 : IEvent
            where T3 : IEvent
            where T2 : IEvent
            where T4 : IEvent =>
            new []
            {
                typeof(T1),
                typeof(T2),
                typeof(T3),
                typeof(T4),
            };
        
        public static Type[] E<T1, T2, T3, T4, T5>() 
            where T1 : IEvent
            where T3 : IEvent
            where T2 : IEvent
            where T4 : IEvent
            where T5 : IEvent =>
            new []
            {
                typeof(T1),
                typeof(T2),
                typeof(T3),
                typeof(T4),
                typeof(T5),
            };
        
        public static Type[] E<T1, T2, T3, T4, T5, T6>() 
            where T1 : IEvent
            where T3 : IEvent
            where T2 : IEvent
            where T4 : IEvent
            where T5 : IEvent
            where T6 : IEvent =>
            new []
            {
                typeof(T1),
                typeof(T2),
                typeof(T3),
                typeof(T4),
                typeof(T5),
                typeof(T6),
            };
        
        public static Type[] E<T1, T2, T3, T4, T5, T6, T7>() 
            where T1 : IEvent
            where T3 : IEvent
            where T2 : IEvent
            where T4 : IEvent
            where T5 : IEvent
            where T6 : IEvent
            where T7 : IEvent =>
            new []
            {
                typeof(T1),
                typeof(T2),
                typeof(T3),
                typeof(T4),
                typeof(T5),
                typeof(T6),
                typeof(T7),
            };
        
        public static Type[] E<T1, T2, T3, T4, T5, T6, T7, T8>() 
            where T1 : IEvent
            where T3 : IEvent
            where T2 : IEvent
            where T4 : IEvent
            where T5 : IEvent
            where T6 : IEvent
            where T7 : IEvent
            where T8 : IEvent =>
            new []
            {
                typeof(T1),
                typeof(T2),
                typeof(T3),
                typeof(T4),
                typeof(T5),
                typeof(T6),
                typeof(T7),
                typeof(T8),
            };
        
        public static Type[] E<T1, T2, T3, T4, T5, T6, T7, T8, T9>() 
            where T1 : IEvent
            where T3 : IEvent
            where T2 : IEvent
            where T4 : IEvent
            where T5 : IEvent
            where T6 : IEvent
            where T7 : IEvent
            where T8 : IEvent
            where T9 : IEvent =>
            new []
            {
                typeof(T1),
                typeof(T2),
                typeof(T3),
                typeof(T4),
                typeof(T5),
                typeof(T6),
                typeof(T7),
                typeof(T8),
                typeof(T9),
            };
        
        public static Type[] E<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>() 
            where T1 : IEvent
            where T3 : IEvent
            where T2 : IEvent
            where T4 : IEvent
            where T5 : IEvent
            where T6 : IEvent
            where T7 : IEvent
            where T8 : IEvent
            where T9 : IEvent
            where T10 : IEvent =>
            new []
            {
                typeof(T1),
                typeof(T2),
                typeof(T3),
                typeof(T4),
                typeof(T5),
                typeof(T6),
                typeof(T7),
                typeof(T8),
                typeof(T9),
                typeof(T10),
            };
        
        
    }
}