using System;
using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using Leopotam.EcsProto.Unity;
using td.features.state.interfaces;
using UnityEngine;

namespace td.features.state
{
    public static class ProtoModulesExtensions
    {
        public static Slice<IStateExtension> BuildStateExtensions(this ProtoModules self)
        {
            var stateExtensions = new Slice<IStateExtension>(32);
            var modules = self.Modules();
            foreach (var module in modules)
            {
                if (module is not IProtoModuleWithStateEx eventsModule) continue;
                
                var stateEx = eventsModule.StateEx();
#if UNITY_EDITOR
                var exType = stateEx.GetType();
                for (var idx = 0; idx < stateExtensions.Len(); idx++)
                {
                    if (stateExtensions.Get(idx) == stateEx)
                    {
                        throw new Exception(
                            $"Failed to add an extension for the {EditorExtensions.GetCleanTypeName(exType)} state because it is already registered");
                    }
                }
#endif
                Debug.Log($">>> stateExtensions.Add({stateEx});");
                stateExtensions.Add(stateEx);
            }

            return stateExtensions;
        }
    }
}