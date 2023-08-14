using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using td.features._common.components;
using td.features.destroy.flags;
using td.features.movement.components;
using td.features.movement.flags;
using td.features.movement.systems;
using UnityEngine;

namespace td.features.movement
{
    public class Movement_Aspect : ProtoAspectInject, IMovementAspect
    {
        public ProtoPool<Movement> movementPool;
        public ProtoPool<ObjectTransform> objectTransformPool;
        public ProtoPool<RefTargetBody> refTargetBodyPool;
        public ProtoPool<IsCustomMovement> isCustomMovementPool;
        public ProtoPool<IsFreezed> isFreezedPool;
        public ProtoPool<IsSmoothRotation> isSmoothRotationPool;
        public ProtoPool<IsTargetReached> isTargetReachedPool;
        public ProtoPool<Ref<GameObject>> refGoPool = default;

        private ProtoItExc itBaseMovement = It
            .Chain<ObjectTransform>()
            .Inc<Movement>()
            .Exc<IsCustomMovement>()
            .Exc<IsSmoothRotation>()
            .Exc<IsDestroyed>()
            .Exc<IsDisabled>()
            .Exc<IsHidden>()
            .Exc<IsFreezed>()
            .End();
        /*private ProtoItExc itBaseMovement = new ProtoItExc(
            It.Inc<ObjectTransform, Movement>(),
            It.Exc(
                typeof(IsCustomMovement),
                typeof(IsDestroyed),
                typeof(IsDisabled),
                typeof(IsHidden),
                typeof(IsFreezed),
                typeof(IsSmoothRotation)
            )
        );*/         
        public ProtoItExc itObjectTransform = new ProtoItExc(
            It.Inc<ObjectTransform, Ref<GameObject>>(),
            It.Exc<IsDestroyed, IsDisabled>()
        ); 
        
        public ProtoItExc itSmoothRotation = new ProtoItExc(
            It.Inc<ObjectTransform, IsSmoothRotation>(),
            It.Exc<IsDestroyed, IsDisabled>()
        );

        public ProtoItExc GetIt() => itBaseMovement;

        public ProtoPool<IsTargetReached> GetIsTargetReachedPool() => isTargetReachedPool;
    }
}