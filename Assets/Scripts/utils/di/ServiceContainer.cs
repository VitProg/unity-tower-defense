using System;
using System.Collections.Generic;
using Leopotam.EcsProto.Unity;

namespace td.utils.di
{
    public static class ServiceContainer
    {
        private static readonly Dictionary<Type, object> Container = new(10);

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
    }
}