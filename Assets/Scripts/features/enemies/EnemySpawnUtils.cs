using td.common;
using td.common.level;
using td.features.enemies.components;
using td.features.enemies.mb;
using UnityEngine;

namespace td.features.enemies
{
    public static class EnemySpawnUtils
    {
        public static bool PrepareSpawnCommand(EnemyConfig enemyConfig, ref WaveSpawnConfigEnemy spawnedEnemy, ref SpawnEnemyOuterCommand spawnCommand)
        {
            switch (spawnedEnemy.name.ToLower())
            {
                case "creep":
                {
                    var type = CreepEnemyMonoBehaviour.ParseType(spawnedEnemy.type);

                    //ToDo
                    if (enemyConfig.types.Length > 0)
                    {
                        var typedConfig = enemyConfig.types[(int)type - 1];
                        spawnCommand.speed *= typedConfig.baseSpeed;
                        spawnCommand.angularSpeed = typedConfig.angularSpeed;
                        spawnCommand.health *= typedConfig.baseHealth;
                        spawnCommand.damage *= typedConfig.baseDamage;
                    }

                    return true;
                }
            }
            return false;
        }

        public static bool PrepareGameObject(GameObject gameObject, ref SpawnEnemyOuterCommand spawnCommand)
        {
            switch (spawnCommand.enemyName.ToLower())
            {
                case "creep":
                {
                    var mb = gameObject.GetComponent<CreepEnemyMonoBehaviour>();

                    mb.type = CreepEnemyMonoBehaviour.ParseType(spawnCommand.enemyType);
                    mb.variant = CreepEnemyMonoBehaviour.ParseVariant(spawnCommand.enemyVariant);
                    mb.UpdateView();

                    return true;
                }
            }

            return false;
        }
    }
}