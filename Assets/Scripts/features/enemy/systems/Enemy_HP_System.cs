using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using td.features.enemy.bus;
using td.features.eventBus;
using UnityEngine;

namespace td.features.enemy.systems
{
    public class Enemy_HP_System : IProtoInitSystem, IProtoDestroySystem
    {
        [DI] private EventBus events;
        [DI] private Enemy_Service enemyService;

        public void Init(IProtoSystems systems)
        {
            events.global.ListenTo<Event_Enemy_ChangeHealth>(OnEnemyHelthChanged);
        }

        public void Destroy()
        {
            events.global.RemoveListener<Event_Enemy_ChangeHealth>(OnEnemyHelthChanged);
        }

        // ------------------------------------------------------------------ //

        private void OnEnemyHelthChanged(ref Event_Enemy_ChangeHealth ev)
        {
            if (!enemyService.IsAlive(ev.Entity, out var enemyEntity)) return;

            ref var enemy = ref enemyService.GetEnemy(enemyEntity);
            var mb = enemyService.GetEnemyMB(enemyEntity);

            mb.hp.minValue = 0f;
            mb.hp.maxValue = enemy.startingHealth;
            mb.hp.value = enemy.health;

            if (enemy.health < enemy.startingHealth)
            {
                mb.hp.gameObject.SetActive(true);
            }

            var p = Mathf.Clamp01(enemy.health / enemy.startingHealth);
            var n = Mathf.FloorToInt(p * (Constants.Enemy.HpBarColors.Length - 1));

            mb.hpLine.color = Constants.Enemy.HpBarColors[n];
        }
    }
}