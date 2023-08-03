using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.features._common;
using td.features.impactEnemy.components;
using td.utils;
using td.utils.ecs;
using UnityEngine;

namespace td.features.impactEnemy
{
    public class ImpactEnemy_Service
    {
        private readonly EcsWorldInject world = default;
        private readonly EcsWorldInject eventsWorld = Constants.Worlds.EventBus;
        
        private readonly EcsPoolInject<PoisonDebuff> poisonDebuffPool = default;
        private readonly EcsPoolInject<ShockingDebuff> shockingDebuffPool = default;
        private readonly EcsPoolInject<SpeedDebuff> speedDebuffPool = default;
        
        private readonly EcsInject<Common_Service> common;
        private readonly EcsInject<IEventBus> events;

        public ref TakeDamage TakeDamage(int entity, float damage, DamageType type = DamageType.Casual)
        {
            Debug.Log($"TakeDamage: {entity}, {damage}, {type}");
            ref var takeDamage =ref events.Value.Entity.Add<TakeDamage>(world.Value.PackEntityWithWorld(entity));
            takeDamage.damage = damage;
            takeDamage.type = type;
            return ref takeDamage;
        }
        
        public void SpeedDebuff(int target, float duration, float speedMultipler)
        {
            ref var debuf = ref speedDebuffPool.Value.GetOrAdd(target);
            debuf.duration = Mathf.Max(duration, debuf.duration);
            debuf.speedMultipler = Mathf.Max(speedMultipler, debuf.speedMultipler);
        }
            
        public void PoisonDebuff(int target, float damage, float duration)
        {
            ref var debuf = ref poisonDebuffPool.Value.GetOrAdd(target);
            debuf.damage = Mathf.Max(debuf.damage, damage);
            debuf.duration = Mathf.Max(debuf.duration, duration);
        }

        public void ShockingDebuff(int target, float probability, float duration)
        {
            if (!shockingDebuffPool.Value.Has(target) && RandomUtils.Bool(probability))
            {
                ref var debuf = ref shockingDebuffPool.Value.GetOrAdd(target);
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
            poisonDebuffPool.Value.SafeDel(entity);
        }

        public void RemoveSpeedDebuff(int entity)
        {
            speedDebuffPool.Value.SafeDel(entity);
        }

        public void RemoveShockingDebuff(int entity)
        {
            shockingDebuffPool.Value.SafeDel(entity);
            common.Value.SetIsFreezed(entity, false);
        }
    }
}