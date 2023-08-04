using System;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.features._common;
using td.features.enemy;
using td.features.fx;
using td.features.fx.effects;
using td.features.impactEnemy.components;
using UnityEngine;

namespace td.features.impactEnemy.systems
{
    public class TakeDamageSystem : IEcsInitSystem, IEcsDestroySystem
    {
        private readonly EcsInject<Enemy_Service> enemyService;
        private readonly EcsInject<Common_Service> common;
        private readonly EcsInject<IEventBus> events;
        private readonly EcsInject<FX_Service> fxService;
        private readonly EcsWorldInject world = default;

        public void Init(IEcsSystems systems)
        {
            events.Value.Entity.ListenTo<TakeDamage>(OnTakeDamage);
        }

        public void Destroy(IEcsSystems systems)
        {
            events.Value.Entity.RemoveListener<TakeDamage>(OnTakeDamage);
        }

        private void OnTakeDamage(EcsPackedEntityWithWorld enemyPackedEntity, ref TakeDamage takeDamage)
        {
            if (takeDamage.damage < 0.0001f) return;
            if (!enemyService.Value.IsAlive(enemyPackedEntity, out var enemyEntity)) return;
            enemyService.Value.ChangeHealthRelative(enemyEntity, -takeDamage.damage);
            
            Debug.Log("takeDamage: " + takeDamage.type + "; " + takeDamage.damage);
            
            var transform = common.Value.GetTransform(enemyEntity);
            ref var blinkFx = ref fxService.Value.EntityModifier.GetOrAdd<BlinkFX>(enemyPackedEntity, 0.3f);
            blinkFx.SetCount(1);
            blinkFx.SetInterval(0.3f);
            blinkFx.SetDuration(0.15f);
            ref var hitFx = ref fxService.Value.EntityFallow.Add<HitFX>(enemyPackedEntity, 0.3f, transform.scale * 1.5f);

            switch (takeDamage.type)
            {
                case DamageType.Casual:
                {
                    blinkFx.Color = Constants.FX.CasualDamageColor;
                    hitFx.Color = Constants.FX.CasualDamageColor;
                    break;
                }
                case DamageType.Fire:
                {
                    blinkFx.Color = Constants.FX.FireDamageColor;
                    hitFx.Color = Constants.FX.FireDamageColor;
                    break;
                }
                case DamageType.Explosion:
                {
                    blinkFx.Color = Constants.FX.ExplosionDamageColor;
                    hitFx.Color = Constants.FX.ExplosionDamageColor;
                    break;
                }
                case DamageType.Poison:
                {
                    blinkFx.SetDuration(0.3f);
                    blinkFx.Color = Constants.FX.PoisonDamageColor;
                    hitFx.Color = Constants.FX.PoisonDamageColor;
                    break;
                }
                case DamageType.Cold:
                {
                    blinkFx.Color = Constants.FX.ColdDamageColor;
                    hitFx.Color = Constants.FX.ColdDamageColor;
                    break;
                }
                case DamageType.Electro:
                {
                    blinkFx.SetCount(2, 3);
                    blinkFx.SetInterval(0.15f);
                    blinkFx.Color = Constants.FX.ElectroDamageColor;
                    hitFx.Color = Constants.FX.ElectroDamageColor;
                    break;
                }
                default:
                    break;
            }
        }
    }
}