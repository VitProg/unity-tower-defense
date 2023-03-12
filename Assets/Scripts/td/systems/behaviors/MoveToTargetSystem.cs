using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.components.attributes;
using td.components.behaviors;
using td.components.commands;
using td.components.events;
using td.components.links;
using td.utils;
using td.utils.ecs;
using Unity.Burst;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Jobs;

namespace td.systems.behaviors
{
    public class MoveToTargetSystem : IEcsRunSystem
    {
        private readonly EcsWorldInject world = default;
        private readonly EcsFilterInject<Inc<GameObjectLink, Target, MoveToTarget>, Exc<SmoothRotateCommand>> entities = default;
        
        public void Run(IEcsSystems systems)
        {
            var entitiesCount = entities.Value.GetEntitiesCount();
            
            var entitiesNativeArray = new NativeArray<int>(entitiesCount, Allocator.Temp);
            var targetNativeArray = new NativeArray<Target>(entitiesCount, Allocator.TempJob);
            var speedNativeArray = new NativeArray<float>(entitiesCount, Allocator.TempJob);
            var transforms = new TransformAccessArray(entitiesCount, 3);
            var onTargetNativeList = new NativeList<int>(Allocator.TempJob);

            var index = 0;
            foreach (var entity in entities.Value)
            {
                entitiesNativeArray[index] = entity;
                
                ref var gameObjectLink = ref entities.Pools.Inc1.Get(entity);
                ref var targetPoint = ref entities.Pools.Inc2.Get(entity);
                ref var movement = ref entities.Pools.Inc3.Get(entity);

                targetNativeArray[index] = targetPoint;
                speedNativeArray[index] = movement.speed;
                transforms.Add(gameObjectLink.gameObject.transform);

                index++;
            }

            var newJob = new MoveToTargetAndLookForwardJob {
                DeltaTime = Time.deltaTime,
                TargetArray = targetNativeArray,
                SpeedArray = speedNativeArray,
                OnTargetNativeList = onTargetNativeList,
            };
            
            var jobHandle = newJob.Schedule(transforms);
            jobHandle.Complete();

            foreach (var onTargetIndex in onTargetNativeList)
            {
                EcsEventUtils.Send(systems, new ReachingTargetEvent()
                {
                    TargetEntity = world.Value.PackEntity(entitiesNativeArray[onTargetIndex])
                });
            }

            targetNativeArray.Dispose();
            speedNativeArray.Dispose();
            transforms.Dispose();
            onTargetNativeList.Dispose();
            entitiesNativeArray.Dispose();
        }
    }


    [BurstCompile] 
    public struct MoveToTargetAndLookForwardJob : IJobParallelForTransform {
        public float DeltaTime;
    
        [NativeDisableParallelForRestriction]
        public NativeArray<Target> TargetArray;

        [NativeDisableParallelForRestriction]
        public NativeArray<float> SpeedArray;

        [NativeDisableParallelForRestriction]
        public NativeList<int> OnTargetNativeList;

        public void Execute(int index, TransformAccess transform)
        {
            var speed = SpeedArray[index];
            var target = TargetArray[index];

            transform.position = Vector3.MoveTowards(transform.position, target.target, DeltaTime * speed);
        
            var distance = (target.target - (Vector2)transform.position).sqrMagnitude;
            if (distance <= target.gap * target.gap)
            {
                OnTargetNativeList.Add(index);
            }
        }
    }
}