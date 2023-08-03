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
            
            switch (takeDamage.type)
            {
                case DamageType.Casual:
                {
                    ref var fx = ref fxService.Value.EntityModifier.GetOrAdd<BlinkFX>(enemyPackedEntity, 0.3f);
                    fx.SetCount(1);
                    fx.color = Constants.FX.CasualDamageColor;
                    if (fx is { isStarted: true, remaining: > 0 }) fx.remaining = Math.Min(2, fx.remaining + 2);
                    break;
                }
                case DamageType.Fire:
                {
                    ref var fx = ref fxService.Value.EntityModifier.GetOrAdd<BlinkFX>(enemyPackedEntity, 0.6f);
                    fx.SetCount(2);
                    fx.color = Constants.FX.FireDamageColor;

                    var transform = common.Value.GetTransform(enemyEntity);
                    ref var fx2 = ref fxService.Value.Position.Add<FireFX>(transform.position, 0.3f, transform.scale);
                    fx2.color = Constants.FX.FireDamageColor;
                    break;
                }
                case DamageType.Explosion:
                {
                    ref var fx = ref fxService.Value.EntityModifier.GetOrAdd<BlinkFX>(enemyPackedEntity, 0.6f);
                    fx.SetCount(2);
                    fx.color = Constants.FX.ExplosionDamageColor;

                    var transform = common.Value.GetTransform(enemyEntity);
                    ref var fx2 = ref fxService.Value.Position.Add<FireFX>(transform.position, 0.3f, transform.scale);
                    fx2.color = Constants.FX.ExplosionDamageColor;
                    break;
                }
                case DamageType.Poison:
                {
                    ref var fx = ref fxService.Value.EntityModifier.GetOrAdd<BlinkFX>(enemyPackedEntity, 0.5f);
                    fx.SetCount(1);
                    fx.SetInterval(0.3f);
                    fx.SetDuration(0.3f);
                    fx.color = Constants.FX.PoisonDamageColor;
                    
                    var transform = common.Value.GetTransform(enemyEntity);
                    // ref var fx2 = ref fxService.Value.Position.Add<PoisonFX>(transform.position, 0.3f, transform.scale);
                    ref var fx2 = ref fxService.Value.EntityFallow.Add<PoisonFX>(enemyPackedEntity, 0.3f, transform.scale);
                    fx2.color = Constants.FX.PoisonDamageColor;
                    break;
                }
                case DamageType.Cold:
                {
                    ref var fx = ref fxService.Value.EntityModifier.GetOrAdd<BlinkFX>(enemyPackedEntity, 0.5f);
                    fx.SetCount(1);
                    fx.SetInterval(0.3f);
                    fx.SetDuration(0.3f);
                    fx.color = Constants.FX.ColdDamageColor;

                    var transform = common.Value.GetTransform(enemyEntity);
                    ref var fx2 = ref fxService.Value.Position.Add<ColdFX>(transform.position, 0.3f, transform.scale);
                    fx2.color = Constants.FX.ColdDamageColor;
                    break;
                }
                case DamageType.Electro:
                {
                    ref var fx = ref fxService.Value.EntityModifier.GetOrAdd<BlinkFX>(enemyPackedEntity, 0.55f);
                    fx.SetCount(2, 3);
                    fx.SetInterval(0.2f);
                    fx.SetDuration(0.25f);
                    fx.color = Constants.FX.ElectroDamageColor;

                    var transform = common.Value.GetTransform(enemyEntity);
                    ref var fx2 = ref fxService.Value.Position.Add<ElectroFX>(transform.position, 0.5f, transform.scale);
                    fx2.color = Constants.FX.ElectroDamageColor;
                    break;
                }
                default:
                    break;
            }
        }
    }
}