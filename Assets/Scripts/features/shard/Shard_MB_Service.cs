using System;
using System.Collections.Generic;
using Leopotam.EcsProto.QoL;
using td.features.camera;
using td.features.shard.data;
using td.features.shard.mb;
using td.features.shard.shardCollection;
using td.features.state;
using td.utils;
using UnityEngine;

namespace td.features.shard
{
    public class Shard_MB_Service
    {
        [DI] private Shard_Calculator calc;
        [DI] private Shard_Service shardService;
        [DI] private Shards_Config_SO configSO;
        [DI] private State state;
        [DI] private ShardCollection_State collectionState;
        [DI] private Camera_Service cameraService;

        private readonly List<UI_Shard> list = new(20);

        private readonly UI_Shard draggableShard;

        public Shard_MB_Service()
        {
            var shardGO = GameObject.FindGameObjectWithTag(Constants.Tags.DraggableShard);
#if UNITY_EDITOR
            if (shardGO == null) throw new Exception($"На сцене не найден DruggableShard");
#endif
            var shardC = shardGO.GetComponent<UI_Shard>();
#if UNITY_EDITOR
            if (shardC == null) throw new Exception($"DruggableShard не содержит компонент ShardConrol");
#endif
            
            shardGO.gameObject.SetActive(false);
            
            draggableShard = shardC;
        }

        public UI_Shard GetDraggableShard() => draggableShard;
        
        public void Add(UI_Shard uiShard)
        {
            if (!list.Contains(uiShard))
            {
                list.Add(uiShard);
                uiShard.FullRefresh();
            }
        }

        public void Remove(UI_Shard shardMB)
        {
            list.Remove(shardMB);
        }

        public void Clear() {
            list.Clear();
        }
        
        public void Update(float deltaTime)
        {
            foreach (var uiShard in list) {
                if (!uiShard.isActiveAndEnabled) continue;
                
                if (uiShard.shard.level == 0) {
                    shardService.PrecalcAllData(ref uiShard.shard);
                }
                uiShard.PartialRefresh(deltaTime * state.GetGameSpeed());
                if (uiShard.ecsEntity && shardService.HasShard(uiShard.ecsEntity.packedEntity, out var shardEntity)) {
                    shardService.GetShard(shardEntity).currentColor = uiShard.shard.currentColor;
                }
                if (uiShard.collectionIndex >= 0 && collectionState.HasItem(uiShard.collectionIndex)) {
                    collectionState.GetItem(uiShard.collectionIndex).currentColor = uiShard.shard.currentColor;
                }
                //todo update color in current operation tower radius
            }
        }
    }
}