// using Leopotam.EcsLite;
// using Leopotam.EcsLite.Di;
// using td.components.attributes;
// using td.components.behaviors;
// using td.components.links;
// using UnityEngine;
//
// namespace td.systems.behaviors
// {
//     public class MoveSystem: IEcsRunSystem
//     {
//         private readonly EcsFilterInject<Inc<Movement, Position, TransformLink>> entities = default;
//         
//         public void Run(IEcsSystems systems)
//         {
//             foreach (var entity in entities.Value)
//             {
//                 ref var movable = ref entities.Pools.Inc1.Get(entity);
//                 ref var positionComponent = ref entities.Pools.Inc2.Get(entity);
//                 ref var transformLink = ref entities.Pools.Inc3.Get(entity);
//
//                 positionComponent.position += movable.vector * movable.speed * Time.deltaTime;
//                 transformLink.transform.position = positionComponent.position;
//             }
//         }
//     }
// }