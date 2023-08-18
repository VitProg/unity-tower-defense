using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using td.features.destroy;
using td.features.ecsConverter;
using td.features.enemy.mb;
using td.features.impactEnemy;
using td.features.movement;
using UnityEngine;

namespace td.features.enemy
{
    public class Enemy_Converter : BaseEntity_Converter
    {
        [DI] private Enemy_Aspect aspect;
        [DI] private Enemy_Service enemyService;
        [DI] private Movement_Service movementService;
        [DI] private Destroy_Service destroyService;
        [DI] private ImpactEnemy_Service impactEnemy;

        public override ProtoWorld World() => aspect.World();
        
        public new void Convert(GameObject gameObject, int entity)
        {
            base.Convert(gameObject, entity);

            var mb = gameObject.GetComponent<EnemyMonoBehaviour>();

            enemyService.GetEnemy(entity);
            enemyService.GetEnemyMBRef(entity).reference = mb;
            
            movementService.SetCustomMovement(entity, true);
            
            movementService.GetRefTargetBody(entity).targetBody = mb.body;
            
            destroyService.SetIsOnlyOnLevel(entity, true);
            impactEnemy.RemoveAllDebuffs(entity);
        }
    }
}