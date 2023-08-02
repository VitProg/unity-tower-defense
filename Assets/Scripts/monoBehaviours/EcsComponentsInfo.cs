// using System;
// using System.Collections.Generic;
// using Leopotam.EcsLite;
// using Leopotam.EcsLite.UnityEditor;
// using NaughtyAttributes;
// using td.features._common;
// using Unity.Collections;
// using UnityEngine;
//
// namespace td.monoBehaviours
// {
//     [DisallowMultipleComponent]
//     [RequireComponent(typeof(EcsEntity))]
//     public class EcsComponentsInfo : MonoBehaviour
//     {
// #if UNITY_EDITOR
//         private EcsEntity ecsEntityInternal;
//         private bool hasEcsEntityInternal;
//         // public EcsWorld world;
//
//         public EcsEntity ecsEntity
//         {
//             get
//             {
//                 if (!hasEcsEntityInternal)
//                 {
//                     hasEcsEntityInternal = true;
//                     ecsEntityInternal = GetComponent<EcsEntity>();
//                     UpdateComponents();
//                 }
//
//                 return ecsEntityInternal;
//             }
//         }
//
//         // private string data;
//         public readonly Dictionary<string, object> components = new();
//         
//         
//
//         // public object[] components = Array.Empty<object>();
//
//         // private bool diResolved;
//         
//         // public async void Awake()
//         // {
//             // await DI.Resolve(this);
//             // diResolved = true;
//         // }
//
//         private void Start()
//         {
//             ecsEntityInternal = GetComponent<EcsEntity>();
//             UpdateComponents();
//             if (ecsEntity.packedEntity.HasValue && ecsEntity.packedEntity.Value.Unpack(out var world, out var entity))
//             {
//                 var entityObserver = gameObject.AddComponent<EcsEntityDebugView>();
//                 entityObserver.Entity = entity;
//                 entityObserver.World = world;
//             }
//         }
//
//         [Button("Refresh")]
//         public void UpdateComponents()
//         {
//             if (ecsEntity.packedEntity.HasValue && ecsEntity.packedEntity.Value.Unpack(out var world, out var entity))
//             {
//                 // componnents.Clear();
//                 
//                 world = ServiceContainer.GetCurrentContainer().Get<EcsWorldsStorage_Service>().world;
//                 var componentsCache = new object[] { };
//                 var count = world.GetComponents(entity, ref componentsCache);
//
//                 foreach (var component in componentsCache)
//                 {
//                     if (component != null)
//                     {
//                         var componentName = component.GetType().Name;
//                         if (!components.TryAdd(componentName, components))
//                         {
//                             components[componentName] = component;
//                         }
//                         // components.Remove(name);
//                         // components.Add(name, ref component);
//                         // Data += "-----\n" +
//                         // component.GetType().Name +
//                         // ":\n" +
//                         // JsonUtility.ToJson(component, true);
//                     }
//                 }
//             }
//         }
// #endif
//     }
// }
