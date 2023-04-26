using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.components.behaviors;
using td.components.commands;
using td.components.events;
using td.components.flags;
using td.components.refs;
using td.utils.ecs;
using Unity.Burst;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Jobs;

namespace td.systems.behaviors
{
    internal struct TargetPoint
    {
        public Vector2 Target;
        public float Gap;
    }

    public class MoveToTargetSystem : IEcsRunSystem
    {
        [InjectWorld] private EcsWorld world;

        private readonly EcsFilterInject<
            Inc<Ref<GameObject>, LinearMovementToTarget>,
            Exc<SmoothRotation, IsDisabled, RemoveGameObjectCommand, IsDestroyed>
        > entities = default;

        public void Run(IEcsSystems systems)
        {
            var entitiesCount = entities.Value.GetEntitiesCount();

            var entitiesNativeArray = new NativeArray<EcsPackedEntity>(entitiesCount, Allocator.TempJob);
            var targetNativeArray = new NativeArray<TargetPoint>(entitiesCount, Allocator.TempJob);
            var speedNativeArray = new NativeArray<float>(entitiesCount, Allocator.TempJob);
            var transforms = new TransformAccessArray(entitiesCount, 3);
            var onTargetNativeList = new NativeList<int>(Allocator.TempJob);

            var index = 0;
            foreach (var entity in entities.Value)
            {
                entitiesNativeArray[index] = world.PackEntity(entity);

                ref var gameObjectLink = ref entities.Pools.Inc1.Get(entity);
                ref var movementToTarget = ref entities.Pools.Inc2.Get(entity);

                targetNativeArray[index] = new TargetPoint
                    { Target = movementToTarget.target, Gap = movementToTarget.gap };
                speedNativeArray[index] = movementToTarget.speed;

                if (gameObjectLink.reference && gameObjectLink.reference.activeSelf)
                {
                    transforms.Add(gameObjectLink.reference.transform);
                }

                index++;
            }

            var newJob = new MoveToTargetSystemJob
            {
                DeltaTime = Time.deltaTime,
                TargetArray = targetNativeArray,
                SpeedArray = speedNativeArray,
                OnTargetNativeList = onTargetNativeList,
            };

            var jobHandle = newJob.Schedule(transforms);
            jobHandle.Complete();

            foreach (var onTargetIndex in onTargetNativeList)
            {
                if (onTargetIndex >= 0 && onTargetIndex < entitiesNativeArray.Length &&
                    entitiesNativeArray[onTargetIndex].Unpack(world, out var entity))
                {
                    world.GetComponent<ReachingTargetEvent>(entity);
                }
            }

            targetNativeArray.Dispose();
            speedNativeArray.Dispose();
            transforms.Dispose();
            onTargetNativeList.Dispose();
            entitiesNativeArray.Dispose();
        }
    }


    [BurstCompile]
    internal struct MoveToTargetSystemJob : IJobParallelForTransform
    {
        public float DeltaTime;

        [NativeDisableParallelForRestriction] public NativeArray<TargetPoint> TargetArray;
        [NativeDisableParallelForRestriction] public NativeArray<float> SpeedArray;
        [NativeDisableParallelForRestriction] public NativeList<int> OnTargetNativeList;

        public void Execute(int index, TransformAccess transform)
        {
            var target = TargetArray[index];
            var speed = SpeedArray[index];

            //-----

            transform.position = Vector3.MoveTowards(transform.position, target.Target, DeltaTime * speed);

            //-----

            var distance = (target.Target - (Vector2)transform.position).sqrMagnitude;
            var gap2 = target.Gap * target.Gap;

            if (distance <= gap2)
            {
                OnTargetNativeList.Add(index);
            }
        }
    }
}
    