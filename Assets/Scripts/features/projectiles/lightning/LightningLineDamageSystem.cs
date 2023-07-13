using System;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.components.commands;
using td.components.flags;
using td.components.refs;
using td.features.enemies.components;
using td.features.impactsEnemy;
using td.features.projectiles.attributes;
using td.utils.ecs;
using UnityEngine;

namespace td.features.projectiles.lightning
{
    /**
     * Periodically deals damage to enemies in chains, with damage fading from the first to the last one
     */
    public class LightningLineDamageSystem : IEcsRunSystem
    {
        [InjectWorld] private EcsWorld world;
        
        private readonly EcsFilterInject<Inc<LightningLine, LightningAttribute, Ref<GameObject>>, Exc<IsDisabled, IsDestroyed>> entities = default;

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in entities.Value)
            {
                ref var lightningLine = ref entities.Pools.Inc1.Get(entity);
                ref var lightning = ref entities.Pools.Inc2.Get(entity);
                
                if (!lightningLine.started)
                {
                    lightningLine.timeRemains = lightning.duration;
                    lightningLine.damageIntervalRemains = lightning.damageInterval;
                    lightningLine.started = true;
                }
                
                lightningLine.timeRemains -= Time.deltaTime;
                lightningLine.damageIntervalRemains -= Time.deltaTime;

                if (lightningLine.timeRemains < Constants.ZeroFloat)
                {
                    world.GetComponent<RemoveGameObjectCommand>(entity);
                    continue;
                }

                if (lightningLine.damageIntervalRemains < 0f)
                {
                    var damage = lightning.damage;
                    for (var index = 0; index < lightningLine.length; index++)
                    {
                        if (!lightningLine.chainEntities[index].Unpack(world, out var chainEntity)) continue;
                        
                        if (!world.HasComponent<Enemy>(chainEntity) ||
                            world.HasComponent<IsDisabled>(chainEntity) || 
                            world.HasComponent<IsDestroyed>(chainEntity)) continue;
                        
                        ref var takeDamage = ref systems.Outer<TakeDamageOuter>();
                        takeDamage.targetEntity = world.PackEntity(chainEntity);
                        takeDamage.damage = damage;
                        takeDamage.type = DamageType.Electro;

                        damage /= lightning.damageReduction;
                    }
                    lightningLine.damageIntervalRemains = lightning.damageInterval;
                }
            }
        }
    }
}