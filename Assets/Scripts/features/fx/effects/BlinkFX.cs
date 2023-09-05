using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using Leopotam.Types;
using td.features._common;
using td.features.fx.types;
using td.features.movement;
using td.features.state;
using td.utils;
using td.utils.ecs;
using UnityEngine;

namespace td.features.fx.effects
{
    [Serializable]
    public struct BlinkFX : IEntityModifierFX, IProtoAutoReset<BlinkFX>, IWithColorFX
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
                if (!isOn) remainingTime = MathFast.Max(0f, remainingTime + (interval - i));
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
                if (isOn) remainingTime = MathFast.Max(0f, remainingTime + (duration - d));
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

    public class BlinkFX_System : ProtoIntervalableRunSystem
    {
        [DI] private Movement_Service movementService;
        [DI] private Common_Service common;
        [DI] private FX_Service fxService;
        [DI(Constants.Worlds.FX)] private FX_Aspect aspect;
        [DI] private State state;

        public override void IntervalRun(float deltaTime)
        {
            foreach (var fxEntity in aspect.itEntityModifier)
            {
                if (!aspect.blinkFXPool.Has(fxEntity)) continue;

                ref var fx = ref aspect.blinkFXPool.Get(fxEntity);
                ref var target = ref aspect.withTargetEntityPool.Get(fxEntity);

                if (!target.entity.Unpack(out _, out var targetEntity))
                {
                    aspect.needRemovePool.GetOrAdd(fxEntity).now = true;
                    continue;
                }

                var targetGO = movementService.HasTargetBody(targetEntity)
                    ? movementService.GetTargetBodyGO(targetEntity)
                    : common.GetGameObject(targetEntity);

                if (!targetGO || !targetGO.activeSelf)
                {
                    aspect.needRemovePool.GetOrAdd(fxEntity).now = true;
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

                if (aspect.needRemovePool.Has(fxEntity))
                {
                    aspect.World().DelEntity(fxEntity);
                    if (fx.sr != null) fx.sr.color = Color.white;
                    continue;
                }

                fx.remainingTime -= deltaTime * state.GetGameSpeed();

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
                        aspect.needRemovePool.GetOrAdd(fxEntity).now = true;
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