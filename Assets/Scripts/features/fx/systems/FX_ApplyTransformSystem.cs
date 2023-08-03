using System;
using Leopotam.EcsLite;
using td.utils.ecs;

namespace td.features.fx.systems
{
    public class FX_ApplyTransformSystem : EcsIntervalableRunSystem
    {
        private readonly EcsInject<FX_Pools> pools;

        public override void IntervalRun(IEcsSystems systems, float dt)
        {
            var count = pools.Value.entityFallowFilter.Value.GetEntitiesCount();
            var arr = pools.Value.entityFallowFilter.Value.GetRawEntities();
            for (var index = 0; index < count; index += 1)
            {
                var entity = arr[index];
                ref var t = ref pools.Value.withTransformPool.Value.Get(entity);
                if (!t.IsChanged()) continue;
                
                if (!pools.Value.refGOPoolFX.Value.Has(entity)) continue;

                ref var goRef = ref pools.Value.refGOPoolFX.Value.Get(entity).reference;
                
                if (goRef == null || !goRef.activeSelf) continue;

                var got = goRef.transform;
                
                if (t.positionChanged) got.position = t.position;
                if (t.scaleChanged) got.localScale = t.GetScaleVector(got.localScale.z);
                if (t.rotationChanged)
                {
                    got.rotation = t.rotation;
                }
            }
        }

        public FX_ApplyTransformSystem(float interval, float timeShift, Func<float> getDeltaTime) : base(interval, timeShift, getDeltaTime)
        {
        }
    }
}