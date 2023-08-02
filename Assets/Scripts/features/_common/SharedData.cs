using Cinemachine;
using Leopotam.EcsLite;
using td.features._common.costPopup;
using td.features.enemy.data;
using td.features.infoPanel;
using td.features.shard.mb;
using td.features.shardCollection;
using td.features.shardStore;
using td.features.ui;
using td.features.window.common;
using td.monoBehaviours;
using UnityEngine;

namespace td.features._common
{
    public class SharedData
    {
        public EnemyConfig[] enemyConfigs;

        public CinemachineVirtualCamera virtualCamera;
        // public EcsUguiEmitter uguiEmitter;
        public Camera mainCamera;
        public Camera canvasCamera;
        public HightlightGridByCursor hightlightGrid;

        public UI_ShardCollection uiShardCollection;
        public UI_ShardStore uiShardStore;
        public UI_InfoPanel uiInfo;
        public ShardConrol draggableShard;
        public EcsPackedEntity draggableShardPackedEntity;
        public CostPopup costPopup;
        public Canvas canvas;
        public FadeInOut fade;
        public GameObject EnemiesContainer;

        public bool IsPerspectiveCameraMode =>
            virtualCamera && mainCamera && 
            !(virtualCamera.m_Lens.Orthographic || mainCamera.orthographic);

        public EnemyConfig? GetEnemyConfig(string enemyName)
        {
            var enemyNameLowerCase = enemyName.ToLower();
            foreach (var enemyConfig in enemyConfigs)
            {
                if (enemyConfig.name == enemyName || enemyConfig.name == enemyNameLowerCase)
                {
                    return enemyConfig;
                }
            }

            return null;
        }
    }
}