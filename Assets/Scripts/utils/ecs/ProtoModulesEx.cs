using System;
using System.Collections.Generic;
using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using UnityEngine;

namespace td.utils.ecs
{
    public class ProtoModulesEx: IProtoModule 
    {
        readonly List<IProtoModule> _modules;
        List<IProtoAspect> _aspects;

        public IReadOnlyList<IProtoModule> AllModules() => _modules;

        public ProtoModulesEx (params IProtoModule[] modules) {
            _aspects = new();
            _modules = new(modules?.Length ?? 4);
            if (modules != null) {
                foreach (var mod in modules) {
                    AddModule (mod);
                }
            }
        }

        public ProtoModulesEx AddModule (IProtoModule module) {
#if DEBUG
            if (module == null) { throw new Exception ("экземпляр модуля должен существовать"); }
#endif
            _modules.Add (module);
            var subMods = module.Modules ();
            if (subMods != null) {
                foreach (var subMod in subMods) {
                    AddModule (subMod);
                }
            }
            return this;
        }

        public ProtoModulesEx AddAspect (IProtoAspect aspect) {
            _aspects.Add (aspect);
            return this;
        }

        public IProtoAspect BuildAspect () {
            return new ComposedAspect (_modules, _aspects);
        }

        public IProtoModule BuildModule () {
            return new ComposedModule (_modules);
        }

        sealed class ComposedAspect : IProtoAspect {
            readonly List<IProtoModule> _modules;
            readonly List<IProtoAspect> _aspects;

            public ComposedAspect (List<IProtoModule> modules, List<IProtoAspect> aspects) {
                _modules = modules;
                _aspects = aspects;
            }

            public void Init (ProtoWorld world) {
                foreach (var mod in _modules) {
                    var aspects = mod.Aspects ();
                    if (aspects != null) {
                        foreach (var aspect in aspects) {
                            aspect.Init (world);
                        }
                    }
                }
                foreach (var aspect in _aspects) {
                    aspect.Init (world);
                }
            }
        }

        sealed class ComposedModule : IProtoModule {
            readonly List<IProtoModule> _modules;

            public ComposedModule (List<IProtoModule> modules) {
                _modules = modules;
            }

            public void Init (IProtoSystems systems) {
                foreach (var mod in _modules) {
                    systems.AddModule (mod);
                }
            }

            public IProtoAspect[] Aspects () => null;
            public IProtoModule[] Modules () => null;
        }

        public void Init (IProtoSystems systems) {
            systems.AddModule (BuildModule ());
        }

        public IProtoAspect[] Aspects () {
            return new[] { BuildAspect () };
        }

        public IProtoModule[] Modules () => null;
    }
}