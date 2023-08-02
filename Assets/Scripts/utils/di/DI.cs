// // -----------------------------------------------------------------------------------------
// // The MIT License
// // Dependency injection for LeoECS Lite https://github.com/Leopotam/ecslite-di
// // Copyright (c) 2021-2022 Leopotam <leopotam@gmail.com>
// // Copyright (c) 2022 7Bpencil <Edward.Ekb@yandex.ru>
// // -----------------------------------------------------------------------------------------
//
// using System;
// using System.Collections.Generic;
// using System.Reflection;
// using System.Threading.Tasks;
// using JetBrains.Annotations;
// using Leopotam.EcsLite;
// using Leopotam.EcsLite.Di;
// using Leopotam.EcsLite.ExtendedSystems;
// using UnityEngine;
//

using System.Linq;
using System.Reflection;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using Object = UnityEngine.Object;

namespace td.utils.di
{
    public class MonoInjectable : MonoBehaviour
    {
        protected void Awake()
        {
            var container = ServiceContainer.GetCurrentContainer();
            if (container != null && container.TryGet<IEcsSystems>(out var systems))
            {
                systems.ResolveMonoBehaviour(this, container);
            }
        }
    }
    
    public static class DIExt
    {
        public static void ResolveMonoBehaviours(this IEcsSystems systems, IServiceContainer container)
        {
            var monoInjectables = Object.FindObjectsOfType<MonoInjectable>(true);
            var services = container.Services.Values.ToArray();
            
            foreach (var injectable in monoInjectables)
            {
                systems.ResolveMonoBehaviour(injectable, container, services);
                // foreach (var service in services)
                // {
                    // InjectToService(injectable, systems, services);
                // }
            }
        }

        public static void ResolveMonoBehaviour(this IEcsSystems systems, MonoBehaviour injectable, IServiceContainer container, object[] _services = null)
        {
            // var monoInjectables = Object.FindObjectsOfType<MonoInjectable>(true);
            var services = _services ?? container.Services.Values.ToArray();
            
            // foreach (var injectable in monoInjectables)
            // {
                foreach (var service in container.Services)
                {
                    InjectToService(injectable, systems, services);
                }
            // }
        }
        
        public static void InjectEx(this IEcsSystems systems, IServiceContainer container, params object[] injects)
        {
            systems.Inject(injects);
            var services = container.Services.Values.ToArray();
            foreach (var service in services)
            {
                InjectToService(service, systems, services);
            }
        }
        
        static void InjectToService (object service, IEcsSystems systems, object[] injects)
        {
            var type = service.GetType();
            foreach (var f in type.GetFields (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)) {
                // skip statics.
                if (f.IsStatic) { continue; }
                // EcsWorldInject, EcsFilterInject, EcsPoolInject, EcsSharedInject.
                if (InjectBuiltIns (f, service, systems)) { continue; }
                // IEcsCustomDataInject derivatives (EcsCustomInject, etc).
                if (InjectCustoms (f, service, injects)) { }
            }

            if (type.BaseType == null || type.BaseType == typeof(System.Object) || type.BaseType == typeof(ScriptableObject) || type.BaseType == typeof(GameObject)) return; 
            
            foreach (var f in type.BaseType.GetFields (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)) {
                // skip statics.
                if (f.IsStatic) { continue; }
                // EcsWorldInject, EcsFilterInject, EcsPoolInject, EcsSharedInject.
                if (InjectBuiltIns (f, service, systems)) { continue; }
                // IEcsCustomDataInject derivatives (EcsCustomInject, etc).
                if (InjectCustoms (f, service, injects)) { }
            }
        }
        
        static bool InjectBuiltIns (FieldInfo fieldInfo, object service, IEcsSystems systems) {
            if (typeof (IEcsDataInject).IsAssignableFrom (fieldInfo.FieldType)) {
                var instance = (IEcsDataInject) fieldInfo.GetValue (service);
                instance.Fill (systems);
                fieldInfo.SetValue (service, instance);
                return true;
            }
            return false;
        }

        static bool InjectCustoms (FieldInfo fieldInfo, object service, object[] injects) {
            if (typeof (IEcsCustomDataInject).IsAssignableFrom (fieldInfo.FieldType)) {
                var instance = (IEcsCustomDataInject) fieldInfo.GetValue (service);
                instance.Fill (injects);
                fieldInfo.SetValue (service, instance);
                return true;
            }
            return false;
        }
        

    }
}
//     [AttributeUsage(AttributeTargets.Class)]
//     public sealed class AutoInjectAttribute : Attribute
//     {
//     }
//     
//     [AttributeUsage(AttributeTargets.Field)]
//     public sealed class InjectSharedAttribute : Attribute
//     {
//     }
//     
//     [AttributeUsage(AttributeTargets.Field)]
//     public sealed class InjectSystemsAttribute : Attribute
//     {
//     }
//
//     [AttributeUsage(AttributeTargets.Field)]
//     public sealed class InjectAttribute : Attribute
//     {
//     }
//
//     [AttributeUsage(AttributeTargets.Field)]
//     public sealed class InjectWorldAttribute : Attribute
//     {
//         public readonly string World;
//
//         public InjectWorldAttribute(string world = default)
//         {
//             World = world;
//         }
//     }
//
//     [AttributeUsage(AttributeTargets.Field)]
//     public sealed class InjectPoolAttribute : Attribute
//     {
//         public readonly string World;
//
//         public InjectPoolAttribute(string world = default)
//         {
//             World = world;
//         }
//     }
//
//     internal struct IdleCustomInject
//     {
//         public FieldInfo fieldInfo;
//         public object target;
//     }
//
//     public interface IHaveCostomResolve
//     {
//         public void ResolveDependencies();
//     }
//
//     public static class DI
//     {
//         private static readonly Type WorldType = typeof(EcsWorld);
//         private static readonly Type SystemsType = typeof(IEcsSystems);
//         private static readonly Type WorldAttrType = typeof(InjectWorldAttribute);
//         private static readonly Type SystemsAttrType = typeof(InjectSystemsAttribute);
//         private static readonly Type PoolType = typeof(EcsPool<>);
//         private static readonly Type PoolAttrType = typeof(InjectPoolAttribute);
//         private static readonly Type SharedAttrType = typeof(InjectSharedAttribute);
//         private static readonly Type InjectAttrType = typeof(InjectAttribute);
//         private static readonly Type AutoInjectAttrType = typeof(AutoInjectAttribute);
//         private static readonly MethodInfo WorldGetPoolMethod = typeof(EcsWorld).GetMethod(nameof(EcsWorld.GetPool));
//
//         private static readonly Dictionary<Type, MethodInfo> GetPoolMethodsCache = new(256);
//
//         private static readonly Dictionary<Type, object> CustomInjects = new(20);
//         private static readonly Dictionary<Type, object> CustomInjectsAuto = new(20);
//         private static object _shared;
//         private static Type _sharedType;
//
//         private static List<IdleCustomInject> _idleCustomInjects = new ();
//         private static List<IdleCustomInject> _idleCustomInjectsCopy = new ();
//
//         public static IEcsSystems InjectLite(this IEcsSystems systems, params object[] injects)
//         {
//             injects ??= Array.Empty<object>();
//
//             foreach (var inject in injects)
//             {
//                 CustomInjects.Add(inject.GetType(), inject);
//             }
//
//             Systems = systems;
//
//             var allSystems = systems.GetAllSystems();
//             _shared = systems.GetShared<object>();
//             _sharedType = _shared?.GetType();
//
//             foreach (var system in allSystems)
//             {
//                 if (system is EcsGroupSystem groupSystem)
//                 {
//                     foreach (var nestedSystem in groupSystem.GetNestedSystems())
//                     {
//                         ResolveInternal((object)nestedSystem);
//                     }
//                 }
//                 else
//                 {
//                     ResolveInternal((object)system);
//                 }
//             }
//
//             foreach (var customInject in CustomInjects)
//             {
//                 ResolveInternal(customInject.Value, true);
//             }
//
//             return systems;
//         }
//
//         private static void ResolveInternal(object target, bool resolveStandartDI = false)
//         {
//             var type = target.GetType();
//             var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
//
//             if (target is IHaveCostomResolve targetWithRosolver)
//             {
//                 targetWithRosolver.ResolveDependencies();
//             }
//                 
//             foreach (var fieldInfo in fields)
//             {
//                 // skip statics.
//                 if (fieldInfo.IsStatic)
//                 {
//                     continue;
//                 }
//
//                 // EcsSystems.
//                 if (InjectSystems(fieldInfo, target))
//                 {
//                     continue;
//                 }             
//                 
//                 // EcsWorld.
//                 if (InjectWorld(fieldInfo, target))
//                 {
//                     continue;
//                 }
//
//                 // EcsPool.
//                 if (InjectPool(fieldInfo, target))
//                 {
//                     continue;
//                 }
//
//                 // Shared.
//                 if (InjectShared(fieldInfo, target))
//                 {
//                     continue;
//                 }
//
//                 // Inject.
//                 if (InjectCustomData(fieldInfo, target))
//                 {
//                     continue;
//                 }
//
//                 if (resolveStandartDI)
//                 {
//                     if (typeof (IEcsDataInject).IsAssignableFrom (fieldInfo.FieldType)) {
//                         var instance = (IEcsDataInject) fieldInfo.GetValue (target);
//                         instance.Fill (Systems);
//                         fieldInfo.SetValue (target, instance);
//                     }
//                 }
//             }
//         }
//
//         public static IEcsSystems Systems { get; private set; }
//
//         public static bool IsReady => Systems != null;
//         
//         public static async Task Resolve<T>(T target) where T : class
//         {
//             while (true)
//             {
//                 if (Systems != null) break;
//                 await Task.Delay(10);
//             }
//             
//             // if (Systems == null)
//             // {
//                 // throw new NullReferenceException(
//                     // "First you need to initialize the DI in ECS Lite by calling InjectLite from systems");
//             // }
//
//             ResolveInternal(target, true);
//
//             Idle();
//         }
//
//         private static void Idle()
//         {
//             if (_idleCustomInjects.Count <= 0) return;
//             
//             // ToDo
//             _idleCustomInjectsCopy.Clear();
//             foreach (var inject in _idleCustomInjects)
//             {
//                 _idleCustomInjectsCopy.Add(inject);
//             }
//             _idleCustomInjects.Clear();
//                 
//             foreach (var inject in _idleCustomInjectsCopy)
//             {
//                 InjectCustomData(inject.fieldInfo, inject.target);
//             }
//             _idleCustomInjectsCopy.Clear();
//         }
//
//         [CanBeNull]
//         public static T GetShared<T>() where T : class
//         {
//             if (_sharedType == typeof(T))
//             {
//                 return (T)_shared;
//             }
//
//             return null;
//         }
//
//         public static IEcsSystems GetSystems() => Systems;
//
//         public static EcsWorld GetWorld(string worldName = null) => Systems.GetWorld(worldName);
//
//         [CanBeNull]
//         public static T Get<T>() where T : class
//         {
//             var type = typeof(T);
//             if (CustomInjects.TryGetValue(type, out var instance))
//             {
//                 return (T)instance;
//             }
//             if (CustomInjectsAuto.TryGetValue(type, out var instanceAuto))
//             {
//                 return (T)instanceAuto;
//             }
//
//             return null;
//         }
//
//         private static bool InjectSystems(FieldInfo fieldInfo, object target)
//         {
//             if (fieldInfo.FieldType != SystemsType)
//             {
//                 return false;
//             }
//
//             if (!Attribute.IsDefined(fieldInfo, SystemsAttrType))
//             {
//                 return true;
//             }
//             
//             if (fieldInfo.FieldType.IsAssignableFrom(SystemsType))
//             {
//                 fieldInfo.SetValue(target, Systems);
//             }
//
//             return true;
//         }
//         
//         private static bool InjectWorld(FieldInfo fieldInfo, object target)
//         {
//             if (fieldInfo.FieldType != WorldType)
//             {
//                 return false;
//             }
//
//             if (!Attribute.IsDefined(fieldInfo, WorldAttrType))
//             {
//                 return true;
//             }
//
//             var worldAttr = (InjectWorldAttribute)Attribute.GetCustomAttribute(fieldInfo, WorldAttrType);
//             fieldInfo.SetValue(target, Systems.GetWorld(worldAttr.World));
//             return true;
//         }
//
//         private static bool InjectPool(FieldInfo fieldInfo, object target)
//         {
//             if (!fieldInfo.FieldType.IsGenericType || fieldInfo.FieldType.GetGenericTypeDefinition() != PoolType)
//             {
//                 return false;
//             }
//
//             if (!Attribute.IsDefined(fieldInfo, PoolAttrType))
//             {
//                 return true;
//             }
//
//             var poolAttr = (InjectPoolAttribute)Attribute.GetCustomAttribute(fieldInfo, PoolAttrType);
//             var world = Systems.GetWorld(poolAttr.World);
//             var componentTypes = fieldInfo.FieldType.GetGenericArguments();
//             fieldInfo.SetValue(target, GetGenericGetPoolMethod(componentTypes[0]).Invoke(world, null));
//             return true;
//         }
//
//         private static MethodInfo GetGenericGetPoolMethod(Type componentType)
//         {
//             if (!GetPoolMethodsCache.TryGetValue(componentType, out var pool))
//             {
//                 pool = WorldGetPoolMethod.MakeGenericMethod(componentType);
//                 GetPoolMethodsCache[componentType] = pool;
//             }
//
//             return pool;
//         }
//
//         private static bool InjectShared(FieldInfo fieldInfo, object system)
//         {
//             if (_shared == null || !Attribute.IsDefined(fieldInfo, SharedAttrType))
//             {
//                 return false;
//             }
//
//             if (fieldInfo.FieldType.IsAssignableFrom(_sharedType))
//             {
//                 fieldInfo.SetValue(system, _shared);
//             }
//
//             return true;
//         }
//
//         private static bool InjectCustomData(FieldInfo fieldInfo, object target, bool idleIfNotExist = true)
//         {
//             if (!Attribute.IsDefined(fieldInfo, InjectAttrType)) {
//                 return false;
//             }
//
//             var injected = false;
//
//             foreach (var inject in CustomInjects)
//             {
//                 if (fieldInfo.FieldType.IsInstanceOfType(inject.Value))
//                 {
//                     injected = true;
//                     fieldInfo.SetValue(target, inject.Value);
//                     break;
//                 }
//             }
//
//             if (!injected)
//             {
//                 foreach (var inject in CustomInjectsAuto)
//                 {
//                     if (fieldInfo.FieldType.IsInstanceOfType(inject.Value))
//                     {
//                         injected = true;
//                         fieldInfo.SetValue(target, inject.Value);
//                         break;
//                     }
//                 }
//             }
//
//             // if (!injected)
//             // {
//             //     Debug.Log("!!!");
//             // }
//
//             if (!injected)
//             {
//                 var fieldType = fieldInfo.FieldType;
//                 var constructor = fieldType.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, Type.EmptyTypes,null) ??
//                                   (fieldType.GetConstructors().Length > 0 ? fieldType.GetConstructors()[0] : null);
//                 if (
//                     fieldType is { IsClass: true, IsGenericType: false, IsAbstract: false } &&
//                     constructor != null &&
//                     Attribute.IsDefined(fieldType, AutoInjectAttrType)
//                 )
//                 {
//                     var constructorParameters = constructor.GetParameters();
//                     var constructorParameterValues = new object[constructorParameters.Length];
//                     
//                     if (constructorParameters.Length > 0)
//                     {
//                         for (var index = 0; index < constructorParameters.Length; index++)
//                         {
//                             var parameter = constructorParameters[index];
//
//                             object value = null;
//
//                             var parameterType = parameter.ParameterType;
//
//                             if (parameterType == SystemsType)
//                             {
//                                 value = Systems;
//                             }
//                             else if (parameterType == WorldType)
//                             {
//                                 value = GetWorld();//todo (string)parameter.DefaultValue);
//                             }
//                             //todo add other type 
//                             else
//                             {
//                                 if (!CustomInjects.TryGetValue(parameterType, out value))
//                                 {
//                                     CustomInjectsAuto.TryGetValue(parameterType, out value);
//                                 }
//                             }
//
//                             constructorParameterValues[index] = value;
//                         }
//                     }
//
//                     var instance = Activator.CreateInstance(fieldInfo.FieldType, constructorParameterValues);
//                     CustomInjectsAuto.Add(instance.GetType(), instance);
//                     ResolveInternal(instance, true);
//                     fieldInfo.SetValue(target, instance);
//                     injected = true;
//                 }
//                 else
//                 {
//                     if (idleIfNotExist) 
//                     {
//                         _idleCustomInjects.Add(new IdleCustomInject()
//                         {
//                             fieldInfo = fieldInfo,
//                             target = target
//                         });
//                     }
//                 }
//             }
//
//             return true;
//         }
//     }
// }