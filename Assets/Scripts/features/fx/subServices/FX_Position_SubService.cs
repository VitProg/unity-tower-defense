using System.Runtime.CompilerServices;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.features.fx.events;
using td.features.fx.types;
using td.utils.ecs;
using UnityEngine;

namespace td.features.fx.subServices
{
    public class FX_Position_SubService
    {
        private readonly EcsWorldInject fxWorld = Constants.Worlds.FX;
        private readonly EcsInject<FX_Pools> pools;
        private readonly EcsInject<IEventBus> events;
        
        public ref T Add<T>(
            Vector2 position,
            float? duration = null,
            Vector2? scale = null,
            Quaternion? rotation = null
        ) where T : struct, IPositionFX
        {
            var pool = fxWorld.Value.GetPool<T>();
            var fxEntity = fxWorld.Value.NewEntity();
            
            ref var fx = ref pool.Add(fxEntity);

            pools.Value.isPositionPool.Value.Add(fxEntity);

            ref var p = ref pools.Value.withTransformPool.Value.Add(fxEntity);
            p.SetPosition(position);
            p.SetScale(scale?? Vector2.one);
            p.SetRotation(rotation?? Quaternion.identity);
            
            ref var d = ref pools.Value.withDurationPool.Value.Add(fxEntity);
            d.SetDuration(duration);
            d.remainingTime = d.duration;
            
            events.Value.Entity.Add<FX_Event_Position_Spawned<T>>(fxEntity, fxWorld.Value);

            return ref fx;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Has<T>(int fxEntity) where T : struct, IPositionFX
        {
            return fxWorld.Value.GetPool<T>().Has(fxEntity);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T Get<T>(int fxEntity) where T : struct, IPositionFX
        {
            return ref fxWorld.Value.GetPool<T>().Get(fxEntity);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Remove<T>(int fxEntity) where T : struct, IPositionFX
        {
            if (!Has<T>(fxEntity)) return;
            pools.Value.needRemovePool.Value.SafeAdd(fxEntity);
        }
    }
}