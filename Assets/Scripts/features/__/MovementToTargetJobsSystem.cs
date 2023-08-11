// using System;
// using Leopotam.EcsLite;
// using Leopotam.EcsLite.Di;
// using td.features._common.components;
// using td.features._common.flags;
// using td.features.movement;
// using td.features.state;
// using td.utils.ecs;
// using Unity.Burst;
// using Unity.Collections;
// using UnityEngine;
// using UnityEngine.Jobs;
// using UnityEngine.Serialization;
//
// namespace td.features._common.systems
// {
//     [Serializable]
//     internal struct TargetPoint
//     {
//         public Vector2 target;
//         [FormerlySerializedAs("gap")] public float gapSqr;
//     }
//
//     public class MovementToTargetJobsSystem : IEcsRunSystem
//     {
//         private readonly EcsWorldInject world;
//         private readonly EcsInject<IState> state;
//         private readonly EcsInject<Common_Pools> commonPools;
//         private readonly EcsInject<Common_Service> common;
//
//         private readonly EcsFilterInject<
//             Inc<Ref<GameObject>, Movement>,
//             Exc<IsSmoothRotation, IsDisabled, IsDestroyed, IsFreezed>
//         > filter = default;
//
//         public void Run(IEcsSystems systems)
//         {
//             var entitiesCount = filter.Value.GetEntitiesCount();
//
//             var entitiesNativeArray = new NativeArray<EcsPackedEntity>(entitiesCount, Allocator.TempJob);
//             var deltaTimeNativeArray = new NativeArray<float>(entitiesCount, Allocator.TempJob);
//             var targetNativeArray = new NativeArray<TargetPoint>(entitiesCount, Allocator.TempJob);
//             var speedNativeArray = new NativeArray<float>(entitiesCount, Allocator.TempJob);
//             var resetZNativeArray = new NativeArray<bool>(entitiesCount, Allocator.TempJob);
//             var transforms = new TransformAccessArray(entitiesCount, 3);
//             var onTargetNativeList = new NativeList<int>(Allocator.TempJob);
//
//             var index = 0;
//             foreach (var entity in filter.Value)
//             {
//                 entitiesNativeArray[index] = world.Value.PackEntity(entity);
//
//                 ref var gameObjectLink = ref filter.Pools.Inc1.Get(entity);
//                 ref var movementToTarget = ref filter.Pools.Inc2.Get(entity);
//
//                 targetNativeArray[index] = new TargetPoint
//                     { target = movementToTarget.target, gapSqr = movementToTarget.gapSqr };
//                 speedNativeArray[index] = movementToTarget.speed;
//
//                 resetZNativeArray[index] = movementToTarget.resetAnchoredPositionZ;
//
//                 deltaTimeNativeArray[index] = movementToTarget.speedOfGameAffected
//                     ? Time.deltaTime * state.Value.GameSpeed
//                     : Time.deltaTime;
//
//                 if (gameObjectLink.reference && gameObjectLink.reference.activeSelf)
//                 {
//                     transforms.Add(gameObjectLink.reference.transform);
//                 }
//
//                 index++;
//             }
//
//             var newJob = new MoveToTargetSystemJob
//             {
//                 // DeltaTime = Time.deltaTime * state.TimeFlow,
//                 DeltaTimeArray = deltaTimeNativeArray,
//                 TargetArray = targetNativeArray,
//                 SpeedArray = speedNativeArray,
//                 OnTargetNativeList = onTargetNativeList,
//             };
//
//             var jobHandle = newJob.Schedule(transforms);
//             jobHandle.Complete();
//
//             foreach (var onTargetIndex in onTargetNativeList)
//             {
//                 if (onTargetIndex >= 0 && onTargetIndex < entitiesNativeArray.Length &&
//                     entitiesNativeArray[onTargetIndex].Unpack(world.Value, out var entity))
//                 {
//                     // todo
//                     commonPools.Value.reachingTargetEventPool.Value.SafeAdd(entity);
//                 }
//             }
//
//             index = 0;
//             foreach (var entity in filter.Value)
//             {
//                 if (resetZNativeArray[index])
//                 {
//                     entitiesNativeArray[index] = world.Value.PackEntity(entity);
//                     ref var gameObjectLink = ref filter.Pools.Inc1.Get(entity);
//                     
//                     var rectTransform = ((RectTransform)gameObjectLink.reference.transform);
//                     var ap = rectTransform.anchoredPosition3D;
//                     rectTransform.anchoredPosition3D = new Vector3(ap.x, ap.y, 0.0f);
//                 }
//
//                 index++;
//             }
//
//             targetNativeArray.Dispose();
//             speedNativeArray.Dispose();
//             transforms.Dispose();
//             onTargetNativeList.Dispose();
//             entitiesNativeArray.Dispose();
//             resetZNativeArray.Dispose();
//         }
//     }
//
//
//     [BurstCompile]
//     internal struct MoveToTargetSystemJob : IJobParallelForTransform
//     {
//         [NativeDisableParallelForRestriction] public NativeArray<TargetPoint> TargetArray;
//         [NativeDisableParallelForRestriction] public NativeArray<float> SpeedArray;
//         [NativeDisableParallelForRestriction] public NativeList<int> OnTargetNativeList;
//         [NativeDisableParallelForRestriction] public NativeArray<float> DeltaTimeArray;
//
//         public void Execute(int index, TransformAccess transform)
//         {
//             var target = TargetArray[index];
//             var speed = SpeedArray[index];
//             var deltaTime = DeltaTimeArray[index];
//
//             //-----
//
//             transform.position = Vector3.MoveTowards(transform.position, target.target, deltaTime * speed);
//             
//             //-----
//
//             var dx = target.target.x - transform.position.x;
//             var dy = target.target.y - transform.position.y;
//             var dsqr = dx * dx - dy * dy;
//
//             // var distance = (target.target - (Vector2)transform.position).sqrMagnitude;
//
//             if (dsqr <= target.gapSqr)
//             {
//                 OnTargetNativeList.Add(index);
//             }
//         }
//     }
// }
//     