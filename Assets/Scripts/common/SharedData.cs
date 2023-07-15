using Cinemachine;
using Leopotam.EcsLite;
using td.features.shards.mb;
using td.features.windows.common;
using td.monoBehaviours;
using UnityEngine;

namespace td.common
{
    public class SharedData
    {
        public EnemyConfig[] enemyConfigs;

        public CinemachineVirtualCamera virtualCamera;
        // public EcsUguiEmitter uguiEmitter;
        public Camera mainCamera;
        public Camera canvasCamera;
        public HightlightGridByCursor hightlightGrid;

        public ShardCollectionPanel shardCollection;
        public ShardStorePopup shardStore;
        public ShardInfoPanel shardInfo;
        public ShardMonoBehaviour draggableShard;
        public EcsPackedEntity draggableShardPackedEntity;
        public CombineShardCostPopup combineShardCostPopup;
        public Canvas canvas;
        public FadeInOut fade;

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