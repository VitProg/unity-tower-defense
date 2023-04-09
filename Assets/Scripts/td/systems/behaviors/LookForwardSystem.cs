// using Leopotam.EcsLite;
// using Leopotam.EcsLite.Di;
// using td.components.behaviors;
// using td.components.flags;
// using td.components.links;
// using UnityEngine;
//
// namespace td.systems.behaviors
// {
//     public class LookForwardSystem: IEcsRunSystem
//     {
//         private readonly EcsFilterInject<Inc<Movement, TransformLink, IsLookForward>> entities = default;
//
//         public void Run(IEcsSystems systems)
//         {
//             foreach (var entity in entities.Value)
//             {
//                 ref var movable = ref entities.Pools.Inc1.Get(entity);
//                 ref var transformLink = ref entities.Pools.Inc2.Get(entity);
//                 
//                 transformLink.transform.rotation = Quaternion.LookRotation(Vector3.forward, movable.vector);
//                 
//                 // Debug.Log($"LookForwardSystem: {transformLink.transform.rotation} for entity #{entity}");
//             }
//         }
//     }
// }