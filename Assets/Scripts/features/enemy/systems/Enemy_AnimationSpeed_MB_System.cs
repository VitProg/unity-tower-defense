using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using td.features.eventBus;
using td.features.state;

namespace td.features.enemy.systems
{
    public class Enemy_AnimationSpeed_MB_System : IProtoInitSystem, IProtoDestroySystem
    {
        [DI] private Enemy_Aspect aspect;
        [DI] private Enemy_Service enemyService;
        [DI] private EventBus events;
        [DI] private State state;

        public void Init(IProtoSystems systems)
        {
            events.unique.ListenTo<Event_StateChanged>(OnStateChanged);
        }

        public void Destroy()
        {
            events.unique.RemoveListener<Event_StateChanged>(OnStateChanged);
        }
        
        // ---------------------------------------------------------------- //

        private void OnStateChanged(ref Event_StateChanged item)
        {
            if (!item.gameSpeed) return;
            
            var gameSpeed = state.GetGameSpeed();

            foreach (var entity in aspect.itLivingEnemies)
            {
                var enemyMb = enemyService.GetEnemyMB(entity);
                if (enemyMb != null && enemyMb.animator != null && enemyMb.baseAnimationSpeed > 0f)
                {
                    enemyMb.animator.speed = enemyMb.baseAnimationSpeed * gameSpeed;
                }
            }
        }
    }
}