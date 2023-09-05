using System.Runtime.CompilerServices;
using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using td.features.fx.types;
using td.utils;
using td.utils.ecs;
using UnityEngine;

namespace td.features.fx.subServices
{
    public class FX_Screen_SubService
    {
        [DI(Constants.Worlds.FX)] private FX_Aspect aspect;
        
        public ref T Add<T>(
            Vector2 position,
            float? duration = null,
            Vector2? scale = null,
            Quaternion? rotation = null
        ) where T : struct, IScreenFX
        {
            var pool = (ProtoPool<T>)aspect.World().Pool(typeof(T));
            var fxEntity = aspect.World().NewEntity();
            
            ref var fx = ref pool.Add(fxEntity);
            
            aspect.isScreenPool.Add(fxEntity);

            ref var p = ref aspect.withTransformPool.Add(fxEntity);
            p.SetPosition(position);
            p.SetScale(scale?? Vector2.one);
            p.SetRotation(rotation?? Quaternion.identity);
            
            ref var d = ref aspect.withDurationPool.Add(fxEntity);
            d.SetDuration(duration);
            d.remainingTime = d.duration;

            return ref fx;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Has<T>(int fxEntity) where T : struct, IScreenFX
        {
            var pool = (ProtoPool<T>)aspect.World().Pool(typeof(T));
            return pool.Has(fxEntity);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T Get<T>(int fxEntity) where T : struct, IScreenFX
        {
            var pool = (ProtoPool<T>)aspect.World().Pool(typeof(T));
            return ref pool.Get(fxEntity);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Remove<T>(int fxEntity) where T : struct, IScreenFX
        {
            if (!Has<T>(fxEntity)) return;
            aspect.needRemovePool.GetOrAdd(fxEntity);
        }
    }
}