using System;
using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using td.utils.di;
using UnityEngine;

namespace td.utils.ecs
{
    public static class ProtoSystemsExtensions
    {
        // private static readonly Type ServiceGenericType = typeof(Service<>);
        
        public static IProtoSystems AddService(this IProtoSystems self, object injectInstance, bool registrateInServiceLocator = false) =>
            AddService(self, injectInstance, default, registrateInServiceLocator);
        
        public static IProtoSystems AddService(this IProtoSystems self, object injectInstance, Type asType = default, bool registrateInServiceLocator = false)
        {
            if (registrateInServiceLocator)
            {
                var type = asType ?? injectInstance.GetType ();
                ServiceContainer.Set(type, injectInstance);
            }
            self.AddService(injectInstance, asType);
            return self;
        }
    }
}