using Leopotam.EcsLite.Di;
using td.features._common;
using td.features._common.components;
using td.features.enemy.components;
using td.features.enemy.mb;
using UnityEngine;

namespace td.features.enemy
{
    public class Enemy_Pools
    {
        public readonly EcsPoolInject<Enemy> enemyPool = default;
        public readonly EcsPoolInject<IsEnemyDead> isEnemyDeadPool = default;
        public readonly EcsPoolInject<EnemyPath> enemyPathPool = default;
        public readonly EcsPoolInject<Ref<EnemyMonoBehaviour>> enemyRefMBPool = default;
        public readonly EcsFilterInject<Inc<Enemy, ObjectTransform>, ExcludeNotAliveEnemies> livingEnemiesFilter = default;
    }
}