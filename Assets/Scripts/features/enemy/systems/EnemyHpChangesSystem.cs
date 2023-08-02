using Leopotam.EcsLite;
using td.features.enemy.bus;
using UnityEngine;

namespace td.features.enemy.systems
{
    public class EnemyHpChangesSystem: IEcsInitSystem, IEcsDestroySystem
    {
        private readonly EcsInject<IEventBus> events;
        private readonly EcsInject<Enemy_Service> enemyService;

        public void Init(IEcsSystems systems)
        {
            events.Value.Entity.ListenTo<Event_Enemy_ChangeHealth>(OnEnemyHelthChanged);
        }

        public void Destroy(IEcsSystems systems)
        {
            events.Value.Entity.RemoveListener<Event_Enemy_ChangeHealth>(OnEnemyHelthChanged);
        }
        
        // ------------------------------------------------------------------ //

        private void OnEnemyHelthChanged(EcsPackedEntityWithWorld packedEntity, ref Event_Enemy_ChangeHealth @event)
        {
            if (!enemyService.Value.IsAlive(packedEntity, out var enemyEntity)) return;

            ref var enemy = ref enemyService.Value.GetEnemy(enemyEntity);
            var mb = enemyService.Value.GetEnemyMB(enemyEntity);
            
            mb.hp.minValue = 0f;
            mb.hp.maxValue = enemy.startingHealth;
            mb.hp.value = enemy.health;

            var p = Mathf.Clamp01(enemy.health / enemy.startingHealth);
            var n = Mathf.FloorToInt(p * (Constants.Enemy.HpBarColors.Length - 1));
            
            mb.hpLine.color = Constants.Enemy.HpBarColors[n];
        }
    }
}