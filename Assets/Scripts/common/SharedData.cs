using Cinemachine;
using Leopotam.EcsLite.Unity.Ugui;

namespace td.common
{
    public class SharedData
    {
        public EnemyConfig[] EnemyConfigs;

        public CinemachineVirtualCamera VirtualCamera;
        public EcsUguiEmitter UGUIEmitter;

        public EnemyConfig? GetEnemyConfig(string enemyName)
        {
            foreach (var enemyConfig in EnemyConfigs)
            {
                if (enemyConfig.name == enemyName)
                {
                    return enemyConfig;
                }
            }

            return null;
        }
    }
}