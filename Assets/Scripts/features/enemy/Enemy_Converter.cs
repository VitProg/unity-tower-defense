using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.features._common;
using td.features._common.flags;
using td.features.ecsConverter;
using td.features.enemy.mb;
using td.features.impactEnemy;
using UnityEngine;

namespace td.features.enemy
{
    public class Enemy_Converter : BaseEntity_Converter
    {
        private readonly EcsInject<Enemy_Service> enemyService;
        private readonly EcsInject<Common_Service> common;
        private readonly EcsInject<ImpactEnemy_Service> impactEnemy;
        
        public new void Convert(GameObject gameObject, int entity)
        {
            base.Convert(gameObject, entity);

            var mb = gameObject.GetComponent<EnemyMonoBehaviour>();

            enemyService.Value.GetEnemy(entity);
            enemyService.Value.GetEnemyMBRef(entity).reference = mb;
            
            common.Value.SetCustomMovement(entity, true);
            
            common.Value.GetRefTargetBody(entity).targetBody = mb.body;
            
            common.Value.SetIsOnlyOnLevel(entity, true);
            impactEnemy.Value.RemoveAllDebuffs(entity);
        }
    }
}