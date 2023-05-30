using Cinemachine;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Unity.Ugui;
using td.features.shards.mb;
using td.features.ui;
using td.monoBehaviours;
using UnityEngine;

namespace td.common
{
    public class SharedData
    {
        public EnemyConfig[] enemyConfigs;

        public CinemachineVirtualCamera virtualCamera;
        public EcsUguiEmitter uguiEmitter;
        public Camera mainCamera;
        public HightlightGridByCursor hightlightGrid;

        public ShardCollectionPanel shardCollection;
        public ShardStorePopup shardStore;
        public ShardInfoPanel shardInfo;
        public ShardMonoBehaviour draggableShard;
        public EcsPackedEntity draggableShardPackedEntity;

        public bool IsPerspectiveCameraMode =>
            virtualCamera && mainCamera && 
            !(virtualCamera.m_Lens.Orthographic || mainCamera.orthographic);

        public EnemyConfig? GetEnemyConfig(string enemyName)
        {
            foreach (var enemyConfig in enemyConfigs)
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