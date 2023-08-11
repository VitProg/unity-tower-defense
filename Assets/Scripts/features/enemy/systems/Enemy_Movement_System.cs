using System;
using Leopotam.EcsProto.QoL;
using td.features.movement.systems;

namespace td.features.enemy.systems
{
    public class Enemy_MovementSub_System : 
    {
        [DI] private Enemy_Aspect aspect;

        protected void Init()
        {
            
        }

        public Enemy_MovementSub_System(float interval, float timeShift, Func<float> getDeltaTime) : base(interval, timeShift, getDeltaTime)
        {
        }
    }
}