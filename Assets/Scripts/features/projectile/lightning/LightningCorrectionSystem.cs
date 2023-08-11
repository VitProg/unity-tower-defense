using System;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.features._common;
using td.features._common.components;
using td.features.enemy;
using td.utils.ecs;
using UnityEngine;

namespace td.features.projectile.lightning
{
    /**
     * Corrects trajectory - updates line renderer vertex coordinates according to the coordinates of target enemies
     */
    public class LightningCorrectionSystem : ProtoIntervalableRunSystem
    {
        private readonly EcsWorldInject world;
        private readonly EcsInject<Common_Service> common;
        private readonly EcsInject<Enemy_Service> enemyService;
        
        private readonly EcsFilterInject<Inc<Lightning, Ref<GameObject>>, ExcludeNotAlive> entities = default; 

        public override void IntervalRun(IEcsSystems systems, float deltaTime)
        {
            foreach (var entity in entities.Value)
            {
                ref var lightningLine = ref entities.Pools.Inc1.Get(entity);
                var lightningLineGO = entities.Pools.Inc2.Get(entity).reference!;

                var lineRenderer = lightningLineGO.GetComponent<LineRenderer>();

                lineRenderer.positionCount = lightningLine.length;

                var count = 0;
                for (var index = 0; index < lightningLine.chainEntities.Length && index < lightningLine.length; index++)
                {
                    var chainPackedEntity = lightningLine.chainEntities[index];

                    if (!chainPackedEntity.Unpack(world.Value, out var chainEntity) || !enemyService.Value.IsAlive(chainEntity)) continue;

                    var position = common.Value.GetGOPosition(chainEntity);
                    
                    lineRenderer.SetPosition(index, position);
                    count++;
                }


                lineRenderer.positionCount = count;
            }
        }

        public LightningCorrectionSystem(float interval, float timeShift, Func<float> getDeltaTime) : base(interval, timeShift, getDeltaTime)
        {
        }
    }
}