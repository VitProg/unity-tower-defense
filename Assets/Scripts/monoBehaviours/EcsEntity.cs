using System;
using JetBrains.Annotations;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using NaughtyAttributes;
using td.features._common;
using td.utils.di;
using UnityEngine;

namespace td.monoBehaviours
{
    public class EcsEntity : MonoBehaviour
    {
        public EcsPackedEntityWithWorld? packedEntity = null;

        [ShowNativeProperty] public bool HasEntity => packedEntity.HasValue;
        // public string worldName = Constants.Worlds.Default;
        
        // private readonly EcsWorldInject world = default;
        // private readonly EcsWorldInject outerWorld = Constants.Worlds.Outer;
//
// #if UNITY_EDITOR
//         [ReadOnly] public int entityID = -1;
//
//         [Button("Unpack Entity")]
//         public void Unpack()
//         {
//             if (!packedEntity.HasValue)
//             {
//                 worldName = "";
//                 entityID = -1;
//                 return;
//             }
//             
//             worldName = Constants.Worlds.Default;
//             if (packedEntity.Value.Unpack(ServiceContainer.Get<EcsWorldsStorage_Service>().world, out entityID)) return;
//             worldName = Constants.Worlds.EventBus;
//             if (packedEntity.Value.Unpack(ServiceContainer.Get<EcsWorldsStorage_Service>().eventsWorld, out entityID)) return;
//             // world = "ui";
//             // PackedEntity.Value.Unpack(DI.GetWorld(Constants.Worlds.UI), out entityID);
//         } 
// #endif

        // [CanBeNull] private EcsWorld world = null;
        
        // [Obsolete]
        // public bool TryGetEntity(out int entity)
        // {
        //     if (packedEntity != null)
        //     {
        //         if (world == null)
        //         {
        //             var s = ServiceContainer.Get<EcsWorldsStorage_Service>();
        //             world = worldName == Constants.Worlds.EventBus ? s.eventsWorld : s.world;
        //         }
        //
        //         if (packedEntity.Value.Unpack(world, out entity))
        //         {
        //             return true;
        //         }
        //     }
        //
        //     entity = -1;
        //     return false;
        // }
    }
}