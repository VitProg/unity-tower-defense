using System;
using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using Leopotam.EcsProto.Unity;
using td.utils.ecs;
using UnityEngine;

namespace td.features.state
{
    public static class ProtoModulesExtensions
    {
        public static Slice<IStateExtension> BuildStateExtensiont(this ProtoModulesEx self)
        {
            var stateExtensions = new Slice<IStateExtension>(16);
            var modules = self.AllModules();
            foreach (var module in modules)
            {
                if (module is not IProtoModuleWithStateEx eventsModule) continue;
                
                var stateEx = eventsModule.StateEx();
                var exType = stateEx.GetType();
                // Debug.Log("try to add state extension " + exType.Name + " " + exType.FullName);
                
                for (var idx = 0; idx < stateExtensions.Len(); idx++)
                {
                    if (stateExtensions.Get(idx) == stateEx)
                    {
                        throw new Exception(
                            $"Failed to add an extension for the {EditorExtensions.GetCleanTypeName(exType)} state because it is already registered");
                    }
                }
                stateExtensions.Add(stateEx);
            }

            return stateExtensions;
        }
    }
}