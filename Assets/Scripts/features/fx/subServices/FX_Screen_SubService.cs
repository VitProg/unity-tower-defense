using System.Runtime.CompilerServices;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.features.fx.types;
using td.utils.ecs;
using UnityEngine;

namespace td.features.fx.subServices
{
    public class FX_Screen_SubService
    {
        private readonly EcsWorldInject fxWorld = Constants.Worlds.FX;
        private readonly EcsInject<FX_Pools> pools;
        
        public ref T Add<T>(
            Vector2 position,
            float? duration = null,
            Vector2? scale = null,
            Quaternion? rotation = null
        ) where T : struct, IScreenFX
        {
            var pool = fxWorld.Value.GetPool<T>();
            var fxEntity = fxWorld.Value.NewEntity();
            
            ref var fx = ref pool.Add(fxEntity);
            
            pools.Value.isScreenPool.Value.Add(fxEntity);

            ref var p = ref pools.Value.withTransformPool.Value.Add(fxEntity);
            p.SetPosition(position);
            p.SetScale(scale?? Vector2.one);
            p.SetRotation(rotation?? Quaternion.identity);
            
            ref var d = ref pools.Value.withDurationPool.Value.Add(fxEntity);
            d.SetDuration(duration);
            d.remainingTime = d.duration;

            return ref fx;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Has<T>(int fxEntity) where T : struct, IScreenFX
        {
            return fxWorld.Value.GetPool<T>().Has(fxEntity);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T Get<T>(int fxEntity) where T : struct, IScreenFX
        {
            return ref fxWorld.Value.GetPool<T>().Get(fxEntity);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Remove<T>(int fxEntity) where T : struct, IScreenFX
        {
            if (!Has<T>(fxEntity)) return;
            pools.Value.needRemovePool.Value.SafeAdd(fxEntity);
        }
    }
}