using System;
using Leopotam.EcsProto.QoL;
using td.features.destroy;
using td.features.enemy;
using td.features.movement;
using td.utils.ecs;
using UnityEngine;

namespace td.features.projectile.lightning
{
    /**
     * Corrects trajectory - updates line renderer vertex coordinates according to the coordinates of target enemies
     */
    public class LightningCorrectionSystem : ProtoIntervalableRunSystem
    {
        [DI] private Lightning_Aspect lightningAspect;
        [DI] private Projectile_Aspect projectileAspect;
        [DI] private Enemy_Service enemyService; 
        [DI] private Movement_Service movementService;
        [DI] private Destroy_Service destroyService; 
        
        public override void IntervalRun(float deltaTime)
        {
            foreach (var entity in lightningAspect.it)
            {
                ref var lightning = ref lightningAspect.lightningPool.Get(entity);
                var lightningLineGO = lightningAspect.refLineRendererPool.Get(entity).reference;

                if (lightningLineGO == null)
                {
                    destroyService.MarkAsRemoved(entity, projectileAspect.World());
                    continue;
                }
                
                var lineRenderer = lightningLineGO.GetComponent<LineRenderer>();

                lineRenderer.positionCount = lightning.length;

                var count = 0;
                for (var index = 0; index < lightning.chainEntities.Length && index < lightning.length; index++)
                {
                    var chainPackedEntity = lightning.chainEntities[index];

                    if (!enemyService.IsAlive(chainPackedEntity, out _)) continue;

                    var position = movementService.GetTransform(chainPackedEntity).position;
                    
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