using System;
using Leopotam.EcsProto.QoL;
using td.features.state;
using td.utils;
using td.utils.ecs;

namespace td.features.fx.systems
{
    public class FX_ApplyTransformSystem : ProtoIntervalableRunSystem
    {
        [DI(Constants.Worlds.FX)] private FX_Aspect aspect;
        [DI] private State state;

        public override void IntervalRun(float deltaTime)
        {
            if (!state.GetSimulationEnabled()) return;
            
            foreach (var entity in aspect.itEntityFallow) 
            {
                ref var t = ref aspect.withTransformPool.Get(entity);
                if (!t.IsChanged()) continue;
                
                if (!aspect.refGOPool.Has(entity)) continue;

                ref var goRef = ref aspect.refGOPool.Get(entity).reference;
                
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