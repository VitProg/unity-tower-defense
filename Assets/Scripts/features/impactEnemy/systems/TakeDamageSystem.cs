using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using td.features._common;
using td.features.enemy;
using td.features.eventBus;
using td.features.fx;
using td.features.fx.effects;
using td.features.impactEnemy.components;
using td.features.movement;
using td.features.state;
using UnityEngine;

namespace td.features.impactEnemy.systems
{
    public class TakeDamageSystem : IProtoInitSystem, IProtoDestroySystem
    {
        // [DI] private ImpactEnemy_Aspect aspect;
        // [DI] private State state;
        // [DI] private ImpactEnemy_Service impactEnemy;
        [DI] private Enemy_Service enemyService;
        // [DI] private Movement_Service movementService;
        // [DI] private FX_Service fxService;
        [DI] private EventBus events;

        public void Init(IProtoSystems systems)
        {
            events.global.ListenTo<TakeDamage>(OnTakeDamage);
        }

        public void Destroy()
        {
            events.global.RemoveListener<TakeDamage>(OnTakeDamage);
        }

        private void OnTakeDamage(ref TakeDamage takeDamage)
        {
            if (takeDamage.damage < 0.0001f) return;
            if (!enemyService.IsAlive(takeDamage.entity, out var enemyEntity)) return;
            enemyService.ChangeHealthRelative(enemyEntity, -takeDamage.damage);
            
            // Debug.Log("takeDamage: " + takeDamage.type + "; " + takeDamage.damage);
            //
            // var transform = movementService.GetTransform(enemyEntity);
            // ref var blinkFx = ref fxService.entityModifier.GetOrAdd<BlinkFX>(takeDamage.entity, 0.3f);
            // blinkFx.SetCount(1);
            // blinkFx.SetInterval(0.3f);
            // blinkFx.SetDuration(0.15f);
            // ref var hitFx = ref fxService.entityFallow.Add<HitFX>(takeDamage.entity, 0.3f, transform.scale * 1.5f);
            //
            // switch (takeDamage.type)
            // {
            //     case DamageType.Casual:
            //     {
            //         blinkFx.Color = Constants.FX.CasualDamageColor;
            //         hitFx.Color = Constants.FX.CasualDamageColor;
            //         break;
            //     }
            //     case DamageType.Fire:
            //     {
            //         blinkFx.Color = Constants.FX.FireDamageColor;
            //         hitFx.Color = Constants.FX.FireDamageColor;
            //         break;
            //     }
            //     case DamageType.Explosion:
            //     {
            //         blinkFx.Color = Constants.FX.ExplosionDamageColor;
            //         hitFx.Color = Constants.FX.ExplosionDamageColor;
            //         break;
            //     }
            //     case DamageType.Poison:
            //     {
            //         blinkFx.SetDuration(0.3f);
            //         blinkFx.Color = Constants.FX.PoisonDamageColor;
            //         hitFx.Color = Constants.FX.PoisonDamageColor;
            //         break;
            //     }
            //     case DamageType.Cold:
            //     {
            //         blinkFx.Color = Constants.FX.ColdDamageColor;
            //         hitFx.Color = Constants.FX.ColdDamageColor;
            //         break;
            //     }
            //     case DamageType.Electro:
            //     {
            //         blinkFx.SetCount(2, 3);
            //         blinkFx.SetInterval(0.15f);
            //         blinkFx.Color = Constants.FX.ElectroDamageColor;
            //         hitFx.Color = Constants.FX.ElectroDamageColor;
            //         break;
            //     }
            //     default:
            //         break;
            // }
        }
    }
}