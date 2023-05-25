using Cinemachine;
using Leopotam.EcsLite.Unity.Ugui;
using td.features.shards.ui;
using td.features.ui;
using td.monoBehaviours;
using UnityEngine;

namespace td.common
{
    public class SharedData
    {
        public EnemyConfig[] EnemyConfigs;

        public CinemachineVirtualCamera VirtualCamera;
        public EcsUguiEmitter UGUIEmitter;
        public Camera MainCamera;
        public HightlightGridByCursor HightlightGrid;

        public ShardsPanel shardsPanel;
        public BuyShardPopup buyShardPopup;
        public ShardInfoPanel shardInfoPanel;
        
        public bool IsPerspectiveCameraMode =>
            VirtualCamera && MainCamera && 
            !(VirtualCamera.m_Lens.Orthographic || MainCamera.orthographic);

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