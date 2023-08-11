using System;
using System.Collections.Generic;
using Leopotam.EcsLite;
using Leopotam.EcsProto;
using UnityEngine;
// #if ENABLE_IL2CPP
// using System;
// using Unity.IL2CPP.CompilerServices;
// #endif

namespace td.utils.ecs
{
    public abstract class ProtoIntervalableRunSystem : IProtoRunSystem
    {
        protected float countdown;
        protected readonly float interval;
        protected float deltaTime;
        protected readonly Func<float> getDeltaTime;
        protected readonly bool withInterval;

        public ProtoIntervalableRunSystem(float interval, float timeShift, Func<float> getDeltaTime)
        {
            this.interval = interval;
            this.countdown = timeShift;
            this.getDeltaTime = getDeltaTime;
            withInterval = !Mathf.Approximately(interval, 0f);
        }

        protected virtual float GetNewInterval()
        {
            return interval;
        }
            
        public void Run(IEcsSystems systems)
        {
            var dt = getDeltaTime();
            countdown -= dt;
            deltaTime += dt;

            if (!(countdown < 0.0001f) && withInterval) return;
            
            IntervalRun(systems, deltaTime);
            countdown = GetNewInterval();
            deltaTime = 0;
        }

        public abstract void IntervalRun(IEcsSystems systems, float dt);
        public void Run()
        {
            throw new NotImplementedException();
        }
    }
}