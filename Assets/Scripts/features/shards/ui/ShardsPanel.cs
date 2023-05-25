using System;
using System.Collections.Generic;
using td.common;
using td.features.shards.mb;
using td.services;
using td.services.ecsConverter;
using td.utils.ecs;
using UnityEngine;
using UnityEngine.UI;

namespace td.features.shards.ui
{
    public class ShardsPanel : MonoBehaviour
    {
        [Inject] private LevelMap levelMap;
        [Inject] private EntityConverters converters;
        [InjectShared] private SharedData shared;
        
        public GridLayoutGroup grid;
        public int max = 8;

        private readonly List<ShardUIButton> shardUiButtonList = new(8);

        private void Start()
        {
            grid ??= GetComponent<GridLayoutGroup>();
        }

        public void Refresh()
        {
            if (levelMap == null)
            {
                DI.Resolve(this);
            }
            
            Clear();

            if (!levelMap!.LevelConfig.HasValue) return;
            
            var shardUiButtonPrefab = (GameObject)Resources.Load("Prefabs/ShardUIButton", typeof(GameObject));
            
            for (var index = 0; index < max; index++)
            {
                var shardUiButtonGO = Instantiate(shardUiButtonPrefab, grid.gameObject.transform);
                var shardMb = shardUiButtonGO.GetComponentInChildren<ShardMonoBehaviour>();
                    
                if (!converters.Convert<Shard>(shardMb.gameObject, out var entity))
                {
                    throw new NullReferenceException($"Failed to convert GameObject {shardMb.gameObject.name}");
                }

                var shardUiButton = shardUiButtonGO.GetComponent<ShardUIButton>();
                shardUiButton.druggable = true;
                shardUiButton.hasShard = false;
                shardUiButton.showPlus = index == 0;
                shardUiButton.cost = 0;

                var button = shardUiButtonGO.GetComponent<Button>();
                button.onClick.AddListener(delegate { OnShardButtonClick(shardUiButton); });
                
                shardUiButtonList.Add(shardUiButton);
            }

            if (levelMap.LevelConfig.Value.startedShards?.Length > 0)
            {
                var index = 0;
                foreach (var startedShard in levelMap.LevelConfig.Value.startedShards)
                {
                    var shardUiButton = shardUiButtonList[index];
                    var shardMb = shardUiButton.gameObject.GetComponentInChildren<ShardMonoBehaviour>();

                    if (shardMb.HasShard())
                    {
                        ref var shard = ref shardMb.GetShard();
                        var source = startedShard;
                        ShardUtils.Copy(ref shard, ref source);
                        shardMb.UpdateFromEntity();

                        shardUiButton.hasShard = true;
                        shardUiButton.showPlus = false;
                    }
                    
                    index++;
                    if (index >= max) break;
                }

                if (index < max - 1)
                {
                    shardUiButtonList[index].showPlus = true;
                }
            }
        }

        private void OnDestroy()
        {
            foreach (var shardUIButton in shardUiButtonList)
            {
                var button = shardUIButton.gameObject.GetComponent<Button>();
                if (button)
                {
                    button.onClick.RemoveAllListeners();
                }
            }
            //todo
            shardUiButtonList.Clear();
        }

        private void OnShardButtonClick(ShardUIButton shardUiButton)
        {
            if (shardUiButton.showPlus)
            {
                var buttonPosition = shardUiButton.transform.position;
                var popupTransform = shared.buyShardPopup.transform;
                var position = new Vector2(buttonPosition.x, popupTransform.position.y);
                popupTransform.position = position;
                shared.buyShardPopup.gameObject.SetActive(!shared.buyShardPopup.gameObject.activeSelf);
            }
            else
            {
                shared.buyShardPopup.gameObject.SetActive(false);
            }
        }

        private void Clear()
        {
            //todo
            foreach (var shardUiButton in transform.GetComponentsInChildren<ShardUIButton>())
            {
                // todo cleanup in ECS
                var button = shardUiButton.gameObject.GetComponent<Button>();
                button.onClick.RemoveAllListeners();
                Destroy(shardUiButton.gameObject);
            }
            shardUiButtonList.Clear();
            shared.buyShardPopup.gameObject.SetActive(false);
        }
    }
}