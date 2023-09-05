using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Leopotam.EcsProto.Unity;
using UnityEngine;

namespace td.utils.di
{
    public static class ServiceContainer
    {
        private static readonly Dictionary<Type, object> Container = new(10);

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public static T Get<T>()
        {
            var type = typeof(T);
#if UNITY_EDITOR
            if (!Container.ContainsKey(type))
            {
                throw new Exception($"Инстанс для типа {EditorExtensions.GetCleanTypeName(type)} не найен в контейнере");
            }
#endif
            return (T)Container[type];
        }       
        
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public static object Get(Type type)
        {
#if UNITY_EDITOR
            if (!Container.ContainsKey(type))
            {
                throw new Exception($"Инстанс для типа {EditorExtensions.GetCleanTypeName(type)} не найен в контейнере");
            }
#endif
            return Container[type];
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public static void Set<T>(T value)
        {
            var type = typeof(T);
#if UNITY_EDITOR
            if (Container.ContainsKey(type))
            {
                throw new Exception($"Инстанс для типа {EditorExtensions.GetCleanTypeName(type)} уже зарегистрирован в контейнере");
            }
#endif
            Container[type] = value;
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public static void Set(Type type, object value)
        {
#if UNITY_EDITOR
            if (Container.ContainsKey(type))
            {
                throw new Exception($"Инстанс для типа {EditorExtensions.GetCleanTypeName(type)} уже зарегистрирован в контейнере");
            }
#endif
            Container[type] = value;
        }
        
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public static void Update<T>(T value)
        {
            var type = typeof(T);
#if UNITY_EDITOR
            if (!Container.ContainsKey(type))
            {
                throw new Exception($"Инстанс для типа {EditorExtensions.GetCleanTypeName(type)} не зарегистрирован в контейнере");
            }
#endif
            Container[type] = value;
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public static void Update(Type type, object value)
        {
#if UNITY_EDITOR
            if (Container.ContainsKey(type))
            {
                throw new Exception($"Инстанс для типа {EditorExtensions.GetCleanTypeName(type)} не зарегистрирован в контейнере");
            }
#endif
            Container[type] = value;
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public static bool Has<T>() => Container.ContainsKey(typeof(T));
        
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public static bool Has(Type type) => Container.ContainsKey(type);
    }
}