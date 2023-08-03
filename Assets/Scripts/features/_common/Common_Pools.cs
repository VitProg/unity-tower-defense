using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Leopotam.EcsLite.ExtendedSystems;
using td.features._common.bus;
using td.features._common.components;
using td.features._common.flags;
using UnityEngine;

namespace td.features._common
{
    public class Common_Pools
    {
        // behaviors
        public readonly EcsPoolInject<MovementToTarget> movementPool = default;
        
        // events
        public readonly EcsPoolInject<IsTargetReached> reachingTargetEventPool = default;
        
        // flags
        public readonly EcsPoolInject<IsDestroyed> isDestroyedPool = default;
        public readonly EcsPoolInject<IsDisabled> isDisabledPool = default;
        public readonly EcsPoolInject<IsFreezed> isFreezedPool = default;
        public readonly EcsPoolInject<IsHidden> isHiddenPool = default;
        public readonly EcsPoolInject<IsOnlyOnLevel> onlyOnLevelPool = default;
        public readonly EcsPoolInject<ObjectTransform> objectTransformPool = default;
        public readonly EcsPoolInject<RefTargetBody> refTargetBodyPool = default;
        
        // commands
        public readonly EcsPoolInject<Command_Idle_Remove> idleRemoveCommandPool = default;
        // public readonly EcsPoolInject<Command_Remove> removeCommandPool = default;
        public readonly EcsPoolInject<IsSmoothRotation> smoothRotationCommandPool = default;
        
        // public readonly EcsPoolInject<IsLoadingOuter> isLoadingOuterPool = Constants.Worlds.Outer;
        
        public readonly EcsPoolInject<Ref<GameObject>> refGoPool = default;
        
        public readonly EcsPoolInject<EcsGroupSystemState> ecsGroupSystemStatePool = Constants.Worlds.EventBus;
        
        public readonly EcsPoolInject<CustomMovement> customMovementPool = default;

        public readonly EcsFilterInject<Inc<IsOnlyOnLevel>> onlyOnLevelFilter = default;
        public readonly EcsFilterInject<Inc<ObjectTransform, Ref<GameObject>>, ExcludeNotAlive> objectTransformFilter = default;
        
        public readonly EcsFilterInject<
            Inc<ObjectTransform, MovementToTarget>,
            ExcludeImmoveable<CustomMovement>
        > baseMovementFilter = default;
    }
    
    public struct ExcludeNotAlive : IEcsExclude
    {
        public EcsWorld.Mask Fill(EcsWorld.Mask mask) =>
            mask.Exc<IsDestroyed>().Exc<IsDisabled>();
    }

    public struct ExcludeImmoveable : IEcsExclude
    {
        public EcsWorld.Mask Fill(EcsWorld.Mask mask) =>
            mask.Exc<IsSmoothRotation>().Exc<IsDisabled>().Exc<IsDestroyed>().Exc<IsFreezed>().Exc<IsHidden>();
    }
    public struct ExcludeImmoveable<T1> : IEcsExclude where T1 : struct
    {
        public EcsWorld.Mask Fill(EcsWorld.Mask mask) =>
            mask.Exc<IsSmoothRotation>().Exc<IsDisabled>().Exc<IsDestroyed>().Exc<IsFreezed>().Exc<IsHidden>().Exc<T1>();
    }

    public struct ExcludeImmoveable<T1, T2> : IEcsExclude where T1 : struct where T2 : struct 
    {
        public EcsWorld.Mask Fill(EcsWorld.Mask mask) =>
            mask.Exc<IsSmoothRotation>().Exc<IsDisabled>().Exc<IsDestroyed>().Exc<IsFreezed>().Exc<IsHidden>().Exc<T1>().Exc<T2>();
    }
}