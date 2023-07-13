using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.components.flags;
using td.components.refs;
using td.features.enemies.components;
using td.features.state;
using td.utils;
using td.utils.ecs;
using UnityEngine;

namespace td.features.impactsEnemy
{
    public class ShockingDebuffSystem: IEcsRunSystem
    {
        [Inject] private State state;
        [InjectWorld] private EcsWorld world;

        private readonly EcsFilterInject<Inc<ShockingDebuff, Enemy, Ref<GameObject>>, Exc<IsDestroyed>> debuffEntities = default;

        public void Run(IEcsSystems systems)
        {
            foreach (var debuffEntity in debuffEntities.Value)
            {
                ref var debuff = ref debuffEntities.Pools.Inc1.Get(debuffEntity);
                var transform = debuffEntities.Pools.Inc3.Get(debuffEntity).reference.transform;

                if (!debuff.started)
                {
                    world.GetComponent<IsFreezed>(debuffEntity);
                    debuff.originalPosition = transform.position;
                    debuff.shiftPositionTimeRemains = Constants.Debuff.ShockingShiftPositionTimeRemains;
                    debuff.started = true;
                }
                
                debuff.shiftPositionTimeRemains -= Time.deltaTime * state.GameSpeed;
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
                    world.DelComponent<IsFreezed>(debuffEntity);
                    world.DelComponent<ShockingDebuff>(debuffEntity);
                }
            }
        }
    }
}