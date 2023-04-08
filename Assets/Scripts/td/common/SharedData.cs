

using System.Linq;
using Leopotam.EcsLite;

namespace td.common
{
    public class SharedData
    {
        public EnemyConfig[] EnemyConfigs;

        public EnemyConfig GetEnemyConfig(string enemyName) =>
            EnemyConfigs.Single(e => e.name == enemyName);
    }
}