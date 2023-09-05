using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using td.features.eventBus;
using td.features.impactEnemy.bus;
using td.features.movement;
using td.features.state;
using td.utils;
using Unity.Mathematics;
using UnityEngine;

namespace td.features.impactEnemy.systems
{
    public class ShockingDebuffSystem: IProtoRunSystem
    {
        [DI] private ImpactEnemy_Aspect aspect;
        [DI] private State state;
        [DI] private ImpactEnemy_Service impactEnemy;
        [DI] private Movement_Service movementService;
        [DI] private EventBus events;
        
        
        //
        private float2 shift;
        //
        
        public void Run()
        {
            foreach (var enemyEntity in aspect.itShockingDebuff)
            {
                ref var debuff = ref aspect.shockingDebuffPool.Get(enemyEntity);
                var transform = movementService.GetTransform(enemyEntity);

                if (!debuff.started)
                {
                    movementService.SetIsFreezed(enemyEntity, true);
                    debuff.originalPosition = transform.position;
                    debuff.shiftPositionTimeRemains = Constants.Debuff.ShockingShiftPositionTimeRemains;
                    debuff.started = true;
                    
                    ref var gotEvent = ref events.global.Add<Event_GotShockingDebuff>();
                    gotEvent.entity = aspect.World().PackEntityWithWorld(enemyEntity);
                    gotEvent.duration = debuff.timeRemains;
                }
                
                debuff.shiftPositionTimeRemains -= Time.deltaTime * state.GetGameSpeed();
                if (debuff.shiftPositionTimeRemains < 0f)
                {
                    shift.x = RandomUtils.Range(-Constants.Debuff.ShockingShiftRange, Constants.Debuff.ShockingShiftRange);
                    shift.y = RandomUtils.Range(-Constants.Debuff.ShockingShiftRange, Constants.Debuff.ShockingShiftRange);
                    
                    transform.position.x = debuff.originalPosition.x + shift.x;
                    transform.position.y = debuff.originalPosition.y + shift.y;
                    
                    debuff.shiftPositionTimeRemains = Constants.Debuff.ShockingShiftPositionTimeRemains;
                }
                
                debuff.timeRemains -= Time.deltaTime;
                if (debuff.timeRemains < 0f)
                {
                    transform.position = debuff.originalPosition;
                    impactEnemy.RemoveShockingDebuff(enemyEntity);
                }
            }
        }
    }
}