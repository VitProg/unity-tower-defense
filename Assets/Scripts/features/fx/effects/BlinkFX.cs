using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.features._common;
using td.features.fx.types;
using td.features.state;
using td.utils.ecs;
using UnityEngine;

namespace td.features.fx.effects
{
    [Serializable]
    public struct BlinkFX : IEntityModifierFX, IEcsAutoReset<BlinkFX>, IWithColorFX
    {
        public byte count;
        public float interval;
        public float duration;
        public Color Color { get; set; }

        internal bool isStarted;
        internal float remainingTime;
        internal int remaining;
        internal bool isOn;
#if !UNITY_SERVER
        [CanBeNull] internal SpriteRenderer sr;
#endif

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void SetCount(byte c) => SetCount(c, (byte)(c + 1));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void SetCount(byte c, byte max)
        {
            if (isStarted)
            {
                var newCount = (byte)Math.Min(max, count + c);
                remaining = (byte)Math.Max(0, remaining + (newCount - count));
                count = newCount;
            }
            else
            {
                count = c > max ? max : c;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void SetInterval(float i)
        {
            if (isStarted)
            {
                if (!isOn) remainingTime = Mathf.Max(0f, remainingTime + (interval - i));
                interval = i;
            }
            else
            {
                interval = i;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void SetDuration(float d)
        {
            if (isStarted)
            {
                if (isOn) remainingTime = Mathf.Max(0f, remainingTime + (duration - d));
                duration = d;
            }
            else
            {
                duration = d;
            }
        }

        public void AutoReset(ref BlinkFX c)
        {
            c.count = 1;
            c.interval = 0.1f;
            c.duration = 0.05f;
            c.Color = Color.grey;

            c.isStarted = false;
            c.remainingTime = 0f;
            c.remaining = c.count;
            c.isOn = false;
#if !UNITY_SERVER
            c.sr = null;
#endif
        }
    }

    public class BlinkFX_System : EcsIntervalableRunSystem
    {
        private readonly EcsInject<Common_Service> common;
        private readonly EcsInject<FX_Service> fxService;
        private readonly EcsWorldInject fxWorld = Constants.Worlds.FX;
        private readonly EcsInject<FX_Pools> pools;
        private readonly EcsInject<State> state;
        
        private readonly EcsPoolInject<BlinkFX> pool = Constants.Worlds.FX;
        
        public override void IntervalRun(IEcsSystems systems, float dt)
        {
            foreach (var fxEntity in pools.Value.entityModifierFilter.Value)
            {
                if (!pool.Value.Has(fxEntity)) continue;

                ref var fx = ref pool.Value.Get(fxEntity);
                // ref var fxType = ref pools.Value.entityModifierFilter.Pools.Inc1.Get(fxEntity);
                ref var target = ref pools.Value.entityModifierFilter.Pools.Inc2.Get(fxEntity);

                if (!target.entity.Unpack(out _, out var targetEntity))
                {
                    pools.Value.needRemovePool.Value.SafeAdd(fxEntity).now = true;
                    continue;
                }

                var targetGO = common.Value.HasTargetBody(targetEntity)
                    ? common.Value.GetTargetBodyGO(targetEntity)
                    : common.Value.GetGameObject(targetEntity);

                if (!targetGO || !targetGO.activeSelf)
                {
                    pools.Value.needRemovePool.Value.SafeAdd(fxEntity).now = true;
                    continue;
                }

                if (!fx.isStarted)
                {
                    fx.isStarted = true;
                    fx.isOn = false;
                    fx.remaining = fx.count;
                    fx.remainingTime = 0f;
#if !UNITY_SERVER
                    if (!targetGO.transform.TryGetComponent(out SpriteRenderer sr))
                    {
                        sr = targetGO.transform.GetComponentInChildren<SpriteRenderer>();
                    }
                    fx.sr = sr;
#endif
                }

                if (pools.Value.needRemovePool.Value.Has(fxEntity))
                {
                    fxWorld.Value.DelEntity(fxEntity);
                    if (fx.sr != null) fx.sr.color = Color.white;
                    continue;
                }

                fx.remainingTime -= dt * state.Value.GameSpeed;

                if (fx.remainingTime > 0f) continue;
                
                if (fx.isOn)
                {
                    fx.isOn = false;
                    fx.remainingTime = fx.interval;
                    fx.remaining--;

                    if (fx.sr != null) fx.sr.color = Color.white;

                    if (fx.remaining <= 0)
                    {
                        if (fx.sr != null) fx.sr.color = Color.white;
                        pools.Value.needRemovePool.Value.SafeAdd(fxEntity).now = true;
                    }
                }
                else
                {
                    fx.isOn = true;
                    fx.remainingTime = fx.duration;
                    if (fx.sr != null) fx.sr.color = fx.Color;
                }
            }
        }
        
        public BlinkFX_System(float interval, float timeShift, Func<float> getDeltaTime) : base(interval, timeShift, getDeltaTime)
        {
        }
    }
}