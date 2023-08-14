using System;
using System.Collections.Generic;
using System.Reflection;
using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using Leopotam.EcsProto.Unity;
using td.utils.di;
using UnityEngine;
using UnityEngine.Rendering;

namespace td.utils.ecs
{
#if ENABLE_IL2CPP
    [Il2CppSetOption (Option.NullChecks, false)]
    [Il2CppSetOption (Option.ArrayBoundsChecks, false)]
#endif
    public class TotalAutoInjectModule : IProtoModule
    {
        private static readonly Type DIAttrType = typeof(DIAttribute);
        public static readonly Type AspectType = typeof(IProtoAspect);
        private static readonly Type ItType = typeof(IProtoIt);
        
        public void Init(IProtoSystems systems)
        {
            systems.AddSystem(new TotalAutoInjectSystem());
        }

        public IProtoAspect[] Aspects() => null;
        public IProtoModule[] Modules() => null;

        public static void Inject(object target, IProtoSystems systems, Dictionary<Type, object> services)
        {
            var type = target.GetType();
            // Debug.Log("INJECT TO TARGET " + type);
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var fi in fields)
            {
                if (fi.IsStatic)
                {
                    continue;
                }

                if (Attribute.IsDefined(fi, DIAttrType))
                {
                    var worldName = ((DIAttribute)Attribute.GetCustomAttribute(fi, DIAttrType)).WorldName;
                    // аспекты.
                    if (AspectType.IsAssignableFrom(fi.FieldType))
                    {
                        fi.SetValue(target, systems.World(worldName).Aspect(fi.FieldType));
                        continue;
                    }

                    // итераторы.
                    if (ItType.IsAssignableFrom(fi.FieldType))
                    {
                        var it = (IProtoIt)fi.GetValue(target);
#if DEBUG
                        if (it == null)
                        {
                            throw new Exception(
                                $"итератор \"{fi.Name}\" в \"{EditorExtensions.GetCleanTypeName(target.GetType())}\" должен быть создан заранее");
                        }
#endif
                        var world = systems.World(worldName);
                        fi.SetValue(target, it.Init(world));
                        continue;
                    }

                    // сервисы.
                    if (services.TryGetValue(fi.FieldType, out var injectObj))
                    {
                        fi.SetValue(target, injectObj);
                    }
                    else
                    {
                        var value = ServiceContainer.Get(fi.FieldType);
                        if (value != null)
                        {
                            fi.SetValue(target, value);
                        }
                        else
                        {
#if DEBUG
                            throw new Exception(
                                $"ошибка инъекции пользовательских данных в \"{EditorExtensions.GetCleanTypeName(target.GetType())}\" - тип поля \"{fi.Name}\" отсутствует в списке сервисов");
#endif
                        }
                    }
                }
            }
        }

#if ENABLE_IL2CPP
    [Il2CppSetOption (Option.NullChecks, false)]
    [Il2CppSetOption (Option.ArrayBoundsChecks, false)]
#endif
        internal sealed class TotalAutoInjectSystem : IProtoPreInitSystem
        {
            public void PreInit(IProtoSystems systems)
            {
                var allServices = systems.Services();
                var allSystems = systems.Systems();

                for (int i = 0, iMax = allSystems.Len(); i < iMax; i++)
                {
                    Inject(allSystems.Get(i), systems, allServices);
                }

                foreach (var (type, service) in allServices)
                {
                    Inject(service, systems, allServices);
                }
            }
        }
    }
}