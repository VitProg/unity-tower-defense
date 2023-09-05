using System;
using Leopotam.EcsProto;
using td.features.enemy.bus;
using td.features.enemy.data;
using td.features.enemy.enemyPath;
using td.features.enemy.systems;
using td.features.eventBus;
using td.utils.ecs;
using UnityEngine;

namespace td.features.enemy {
    public class Enemy_Module : IProtoModuleWithEvents {
        private readonly Func<float> getDeltaTime;
        private readonly Enemies_Config_SO enemiesConfigSO;

        public Enemy_Module(Func<float> getDeltaTime) {
            this.getDeltaTime = getDeltaTime;

            // Attension! also used in CreepEnemyMonoBehaviour, the values must match!
            enemiesConfigSO = Resources.Load<Enemies_Config_SO>("Configs/Enemies Config");
        }

        public void Init(IProtoSystems systems) {
            systems
                .AddSystem(new Enemy_InitData_System())
                .AddSystem(new Enemy_AnimationSpeed_MB_System())
                .AddSystem(new Enemy_Movement_System(1 / 30f, 0f, getDeltaTime))
                .AddSystem(new Enemy_ReachCell_System())
                .AddSystem(new Enemy_ReachKernel_System())
                .AddSystem(new Enemy_HP_System())
                .AddSystem(new Enemy_Died_System())
                //
                .AddService(enemiesConfigSO, true)
                .AddService(new Enemy_Service(), true)
                .AddService(new Enemy_Converter(), true)
                ;
        }

        public IProtoAspect[] Aspects() {
            return new IProtoAspect[] { new Enemy_Aspect(), };
        }

        public IProtoModule[] Modules() {
            return new IProtoModule[] {
                new EnemyPath_Module(getDeltaTime),
            };
        }

        public Type[] Events() =>
            Ev.E<
                Event_Enemy_ChangeHealth,
                Event_Enemy_ReachKernel,
                Event_Enemy_Spawned,
                Event_Enemy_Died
            >();
    }
}
