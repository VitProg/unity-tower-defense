using Leopotam.EcsProto.QoL;
using Leopotam.Types;
using td.features._common;
using td.features.eventBus;
using td.features.impactEnemy.components;
using td.features.movement;
using td.utils;
using td.utils.ecs;

namespace td.features.impactEnemy
{
    public class ImpactEnemy_Service
    {
        [DI] private ImpactEnemy_Aspect aspect;
        [DI] private Movement_Service movementService;
        [DI] private EventBus events;

        public ref TakeDamage TakeDamage(int entity, float damage, DamageType type = DamageType.Casual)
        {
            // Debug.Log($"TakeDamage: {entity}, {damage}, {type}");
            ref var takeDamage =ref events.global.Add<TakeDamage>();
            takeDamage.entity = aspect.World().PackEntityWithWorld(entity);
            takeDamage.damage = damage;
            takeDamage.type = type;
            return ref takeDamage;
        }
        
        public void SpeedDebuff(int target, float duration, float speedMultipler)
        {
            ref var debuf = ref aspect.speedDebuffPool.GetOrAdd(target);
            debuf.duration = MathFast.Max(duration, debuf.duration);
            debuf.speedMultipler = MathFast.Max(speedMultipler, debuf.speedMultipler);
        }
            
        public void PoisonDebuff(int target, float damage, float duration)
        {
            ref var debuf = ref aspect.poisonDebuffPool.GetOrAdd(target);
            debuf.damage = MathFast.Max(debuf.damage, damage);
            debuf.duration = MathFast.Max(debuf.duration, duration);
        }

        public void ShockingDebuff(int target, float probability, float duration)
        {
            if (!aspect.shockingDebuffPool.Has(target) && RandomUtils.Bool(probability))
            {
                ref var debuf = ref aspect.shockingDebuffPool.GetOrAdd(target);
                debuf.timeRemains = duration;
                debuf.started = false;
            }
        }

        public void RemoveAllDebuffs(int entity)
        {
            RemoveSpeedDebuff(entity);
            RemovePoisonDebuff(entity);
            RemoveShockingDebuff(entity);
        }

        public void RemovePoisonDebuff(int entity)
        {
            aspect.poisonDebuffPool.Del(entity);
        }

        public void RemoveSpeedDebuff(int entity)
        {
            aspect.speedDebuffPool.Del(entity);
        }

        public void RemoveShockingDebuff(int entity)
        {
            aspect.shockingDebuffPool.Del(entity);
            movementService.SetIsFreezed(entity, false);
        }
    }
}