using Cinemachine;
using Leopotam.EcsLite.Unity.Ugui;
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