using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using Leopotam.EcsProto.Unity;

namespace td.features.state
{
    public class State_Aspect : ProtoAspectInject
    {
        public readonly Slice<IStateExtension> extensions = new();
        private readonly Dictionary<Type, int> extensionsHash = new (10);
        public bool release;

        public override void Init(ProtoWorld world)
        {
            // Debug.Log("State_Aspect.Init() " + world);
            base.Init(world);
            release = true;
        }

        public void AddEx<T>(T ex) where T : IStateExtension
        {
            var type = ex.GetType();
#if UNITY_EDITOR
            if (extensionsHash.TryGetValue(type, out _))
            {
                throw new Exception($"State extension {EditorExtensions.GetCleanTypeName(type)} already registered");
            }
#endif
            extensions.Add(ex);
            var idx = extensions.Len() - 1;
            extensionsHash[type] = idx;
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public bool HasEx(Type type, out int idx) => extensionsHash.TryGetValue(type, out idx);

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public int LenEx() => extensions.Len();
        
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public IStateExtension GetExByIndex(int idx) => extensions.Get(idx);
    }
}