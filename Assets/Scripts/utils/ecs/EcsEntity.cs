using System.Runtime.CompilerServices;
using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using NaughtyAttributes;
using UnityEngine;

namespace td.utils.ecs
{
    public class EcsEntity : MonoBehaviour
    {
        public ProtoPackedEntityWithWorld packedEntity = default;

        [ShowNativeProperty] 
        public bool HasEntity {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => packedEntity.Unpack(out _, out _);
        }
        
#if UNITY_EDITOR
        [ShowNativeProperty] public int DbgEntity => packedEntity.Id;
        [ShowNativeProperty] public int DbgGen => packedEntity.Gen;
        [ShowNativeProperty] public bool DbgWorldAlive => packedEntity.World.IsAlive();
#endif
    }
}