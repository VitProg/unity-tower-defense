using System;
using Leopotam.EcsProto;
using UnityEngine;

namespace td.utils.ecs
{
    public abstract class ProtoIntervalableRunSystem : IProtoRunSystem
    {
        private float countdown;
        private readonly float interval;
        private float deltaTime;
        private readonly Func<float> getDeltaTime;
        private readonly bool withInterval;

        protected ProtoIntervalableRunSystem(float interval, float timeShift, Func<float> getDeltaTime)
        {
            this.interval = interval;
            this.countdown = timeShift;
            this.getDeltaTime = getDeltaTime;
            withInterval = !Mathf.Approximately(interval, 0f);
        }
    
        public void Run()
        {
            var dt = getDeltaTime();
            countdown -= dt;
            deltaTime += dt;

            if (!(countdown < 0.0001f) && withInterval) return;
            
            IntervalRun(deltaTime);
            countdown = interval;
            deltaTime = 0;
        }

        public abstract void IntervalRun(float deltaTime);
    }
}