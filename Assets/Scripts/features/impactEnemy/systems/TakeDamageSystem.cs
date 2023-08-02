using Leopotam.EcsLite;
using td.features.enemy;
using td.features.impactEnemy.components;

namespace td.features.impactEnemy.systems
{
    public class TakeDamageSystem : IEcsInitSystem, IEcsDestroySystem
    {
        private readonly EcsInject<Enemy_Service> enemyService;
        private readonly EcsInject<IEventBus> events;

        public void Init(IEcsSystems systems)
        {
            events.Value.Entity.ListenTo<TakeDamage>(OnTakeDamage);
        }

        public void Destroy(IEcsSystems systems)
        {
            events.Value.Entity.RemoveListener<TakeDamage>(OnTakeDamage);
        }

        private void OnTakeDamage(EcsPackedEntityWithWorld packedEntity, ref TakeDamage takeDamage)
        {
            if (takeDamage.damage < 0.0001f) return;
            if (!enemyService.Value.IsAlive(packedEntity, out var enemyEntity)) return;
            enemyService.Value.ChangeHealthRelative(enemyEntity, -takeDamage.damage);
        }
    }
}