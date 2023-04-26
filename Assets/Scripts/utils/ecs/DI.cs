// -----------------------------------------------------------------------------------------
// The MIT License
// Dependency injection for LeoECS Lite https://github.com/Leopotam/ecslite-di
// Copyright (c) 2021-2022 Leopotam <leopotam@gmail.com>
// Copyright (c) 2022 7Bpencil <Edward.Ekb@yandex.ru>
// -----------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;
using Leopotam.EcsLite;
using UnityEngine;

namespace td.utils.ecs
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class InjectSharedAttribute : Attribute
    {
    }
    
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class InjectSystemsAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Field)]
    public sealed class InjectAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Field)]
    public sealed class InjectWorldAttribute : Attribute
    {
        public readonly string World;

        public InjectWorldAttribute(string world = default)
        {
            World = world;
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public sealed class InjectPoolAttribute : Attribute
    {
        public readonly string World;

        public InjectPoolAttribute(string world = default)
        {
            World = world;
        }
    }

    internal struct IdleCustomInject
    {
        public FieldInfo FieldInfo;
        public object Target;
    }

    public interface IHaveCostomResolve
    {
        public void ResolveDependencies();
    }

    public static class DI
    {
        private static readonly Type WorldType = typeof(EcsWorld);
        private static readonly Type SystemsType = typeof(IEcsSystems);
        private static readonly Type WorldAttrType = typeof(InjectWorldAttribute);
        private static readonly Type SystemsAttrType = typeof(InjectSystemsAttribute);
        private static readonly Type PoolType = typeof(EcsPool<>);
        private static readonly Type PoolAttrType = typeof(InjectPoolAttribute);
        private static readonly Type SharedAttrType = typeof(InjectSharedAttribute);
        private static readonly Type InjectAttrType = typeof(InjectAttribute);
        private static readonly MethodInfo WorldGetPoolMethod = typeof(EcsWorld).GetMethod(nameof(EcsWorld.GetPool));

        private static readonly Dictionary<Type, MethodInfo> GetPoolMethodsCache = new(256);

        private static readonly Dictionary<Type, object> CustomInjects = new(10);
        private static object _shared;
        private static Type _sharedType;

        private static List<IdleCustomInject> _idleCustomInjects = new ();
        private static List<IdleCustomInject> _idleCustomInjectsCopy = new ();

        public static IEcsSystems InjectLite(this IEcsSystems systems, params object[] injects)
        {
            injects ??= Array.Empty<object>();

            foreach (var inject in injects)
            {
                CustomInjects.Add(inject.GetType(), inject);
            }

            Systems = systems;

            var allSystems = systems.GetAllSystems();
            _shared = systems.GetShared<object>();
            _sharedType = _shared?.GetType();

            foreach (var system in allSystems)
            {
                ResolveInternal((object)system);
            }

            foreach (var customInject in CustomInjects)
            {
                ResolveInternal(customInject.Value);
            }

            return systems;
        }

        private static void ResolveInternal(object target)
        {
            var type = target.GetType();
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            if (target is IHaveCostomResolve targetWithRosolver)
            {
                targetWithRosolver.ResolveDependencies();
            }
                
            foreach (var f in fields)
            {
                // skip statics.
                if (f.IsStatic)
                {
                    continue;
                }

                // EcsSystems.
                if (InjectSystems(f, target))
                {
                    continue;
                }             
                
                // EcsWorld.
                if (InjectWorld(f, target))
                {
                    continue;
                }

                // EcsPool.
                if (InjectPool(f, target))
                {
                    continue;
                }

                // Shared.
                if (InjectShared(f, target))
                {
                    continue;
                }

                // Inject.
                if (InjectCustomData(f, target))
                {
                    continue;
                }
            }
        }

        public static IEcsSystems Systems { get; private set; }

        public static bool IsReady => Systems != null;
        
        public static T Resolve<T>(T target) where T : class
        {
            if (Systems == null)
            {
                throw new NullReferenceException(
                    "First you need to initialize the DI in ECS Lite by calling InjectLite from systems");
            }

            ResolveInternal(target);

            Idle();

            return target;
        }

        private static void Idle()
        {
            if (_idleCustomInjects.Count <= 0) return;
            
            // ToDo
            _idleCustomInjectsCopy.Clear();
            foreach (var inject in _idleCustomInjects)
            {
                _idleCustomInjectsCopy.Add(inject);
            }
            _idleCustomInjects.Clear();
                
            foreach (var inject in _idleCustomInjectsCopy)
            {
                InjectCustomData(inject.FieldInfo, inject.Target);
            }
            _idleCustomInjectsCopy.Clear();
        }

        [CanBeNull]
        public static T GetShared<T>() where T : class
        {
            if (_sharedType == typeof(T))
            {
                return (T)_shared;
            }

            return null;
        }

        public static EcsWorld GetWorld(string worldName = null) => Systems.GetWorld(worldName);

        [CanBeNull]
        public static T GetCustom<T>() where T : class
        {
            var type = typeof(T);
            if (CustomInjects.TryGetValue(type, out var instance))
            {
                return (T)instance;
            }

            return null;
        }

        private static bool InjectSystems(FieldInfo fieldInfo, object target)
        {
            if (fieldInfo.FieldType != SystemsType)
            {
                return false;
            }

            if (!Attribute.IsDefined(fieldInfo, SystemsAttrType))
            {
                return true;
            }
            
            if (fieldInfo.FieldType.IsAssignableFrom(SystemsType))
            {
                fieldInfo.SetValue(target, Systems);
            }

            return true;
        }
        
        private static bool InjectWorld(FieldInfo fieldInfo, object target)
        {
            if (fieldInfo.FieldType != WorldType)
            {
                return false;
            }

            if (!Attribute.IsDefined(fieldInfo, WorldAttrType))
            {
                return true;
            }

            var worldAttr = (InjectWorldAttribute)Attribute.GetCustomAttribute(fieldInfo, WorldAttrType);
            fieldInfo.SetValue(target, Systems.GetWorld(worldAttr.World));
            return true;
        }

        private static bool InjectPool(FieldInfo fieldInfo, object target)
        {
            if (!fieldInfo.FieldType.IsGenericType || fieldInfo.FieldType.GetGenericTypeDefinition() != PoolType)
            {
                return false;
            }

            if (!Attribute.IsDefined(fieldInfo, PoolAttrType))
            {
                return true;
            }

            var poolAttr = (InjectPoolAttribute)Attribute.GetCustomAttribute(fieldInfo, PoolAttrType);
            var world = Systems.GetWorld(poolAttr.World);
            var componentTypes = fieldInfo.FieldType.GetGenericArguments();
            fieldInfo.SetValue(target, GetGenericGetPoolMethod(componentTypes[0]).Invoke(world, null));
            return true;
        }

        private static MethodInfo GetGenericGetPoolMethod(Type componentType)
        {
            if (!GetPoolMethodsCache.TryGetValue(componentType, out var pool))
            {
                pool = WorldGetPoolMethod.MakeGenericMethod(componentType);
                GetPoolMethodsCache[componentType] = pool;
            }

            return pool;
        }

        private static bool InjectShared(FieldInfo fieldInfo, object system)
        {
            if (_shared == null || !Attribute.IsDefined(fieldInfo, SharedAttrType))
            {
                return false;
            }

            if (fieldInfo.FieldType.IsAssignableFrom(_sharedType))
            {
                fieldInfo.SetValue(system, _shared);
            }

            return true;
        }

        private static bool InjectCustomData(FieldInfo fieldInfo, object target, bool idleIfNotExist = true)
        {
            if (CustomInjects.Count <= 0 || !Attribute.IsDefined(fieldInfo, InjectAttrType))
            {
                return false;
            }

            var injected = false;

            foreach (var inject in CustomInjects)
            {
                if (fieldInfo.FieldType.IsInstanceOfType(inject.Value))
                {
                    injected = true;
                    fieldInfo.SetValue(target, inject.Value);
                    break;
                }
            }

            // if (!injected)
            // {
            //     Debug.Log("!!!");
            // }
            
            if (!injected && idleIfNotExist)
            {
                _idleCustomInjects.Add(new IdleCustomInject()
                {
                    FieldInfo = fieldInfo,
                    Target = target
                });
            }
            
            return true;
        }
    }
}