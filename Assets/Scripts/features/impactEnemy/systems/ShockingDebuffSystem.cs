using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.features._common;
using td.features._common.components;
using td.features.enemy.components;
using td.features.impactEnemy.components;
using td.features.state;
using td.utils;
using UnityEngine;

namespace td.features.impactEnemy.systems
{
    public class ShockingDebuffSystem: IEcsRunSystem
    {
        private readonly EcsInject<IState> state;
        private readonly EcsInject<Common_Service> common;
        private readonly EcsInject<IEventBus> events;
        private readonly EcsInject<ImpactEnemy_Service> impactEnemy;

        private readonly EcsFilterInject<Inc<ShockingDebuff, Enemy, Ref<GameObject>>, ExcludeNotAlive> filter = default;

        public void Run(IEcsSystems systems)
        {
            foreach (var debuffEntity in filter.Value)
            {
                ref var debuff = ref filter.Pools.Inc1.Get(debuffEntity);
                var transform = filter.Pools.Inc3.Get(debuffEntity).reference!.transform;

                if (!debuff.started)
                {
                    common.Value.SetIsFreezed(debuffEntity, true);
                    debuff.originalPosition = transform.position;
                    debuff.shiftPositionTimeRemains = Constants.Debuff.ShockingShiftPositionTimeRemains;
                    debuff.started = true;
                }
                
                debuff.shiftPositionTimeRemains -= Time.deltaTime * state.Value.GameSpeed;
                if (debuff.shiftPositionTimeRemains < 0f)
                {
                    var shift = new Vector3(
                        RandomUtils.Range(-Constants.Debuff.ShockingShiftRange, Constants.Debuff.ShockingShiftRange),
                        RandomUtils.Range(-Constants.Debuff.ShockingShiftRange, Constants.Debuff.ShockingShiftRange),
                        0f
                    );
                    transform.position = debuff.originalPosition + shift;
                    debuff.shiftPositionTimeRemains = Constants.Debuff.ShockingShiftPositionTimeRemains;
                }
                
                debuff.timeRemains -= Time.deltaTime;
                if (debuff.timeRemains < 0f)
                {
                    transform.position = debuff.originalPosition;
                    impactEnemy.Value.RemoveShockingDebuff(debuffEntity);
                }
            }
        }
    }
}