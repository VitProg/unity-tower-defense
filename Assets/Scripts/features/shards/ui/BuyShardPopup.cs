using System;
using System.Collections.Generic;
using Leopotam.EcsLite;
using td.common;
using td.features.shards.mb;
using td.services;
using td.services.ecsConverter;
using td.utils.ecs;
using UnityEngine;
using UnityEngine.UI;

namespace td.features.shards.ui
{
    public class BuyShardPopup : MonoBehaviour
    {
        [Inject] private LevelMap levelMap;
        [Inject] private EntityConverters converters;
        [InjectSystems] private EcsSystems ecsSystems;
        [InjectShared] private SharedData shared;
        [InjectWorld] private EcsWorld world;

        public Button closeButton;
        public GridLayoutGroup grid;

        private readonly List<ShardUIButton> shardUiButtonList = new(8);

        private void Start()
        {
            grid ??= GetComponent<GridLayoutGroup>();
            closeButton.onClick.AddListener(OnClose);
        }

        public void Refresh()
        {
            if (levelMap == null)
            {
                DI.Resolve(this);
            }

            Clear();

            if (!levelMap!.LevelConfig.HasValue) return;

            var availableShards = levelMap.LevelConfig.Value.availableShards;
            var shardsCost = levelMap.LevelConfig.Value.shardsCost;

            var shardUiButtonPrefab = (GameObject)Resources.Load("Prefabs/ShardUIButton", typeof(GameObject));

            for (var index = 0; index < availableShards.Length; index++)
            {
                var availableShard = availableShards[index];
                var cost = shardsCost[index];
                
                var shardUiButtonGO = Instantiate(shardUiButtonPrefab, grid.gameObject.transform);
                var shardMb = shardUiButtonGO.GetComponentInChildren<ShardMonoBehaviour>();

                if (!converters.Convert<Shard>(shardMb.gameObject, out var entity))
                {
                    throw new NullReferenceException($"Failed to convert GameObject {shardMb.gameObject.name}");
                }

                if (!shardMb.HasShard()) continue;

                var fieldName = availableShard.ToLower().Trim();

                ref var shard = ref shardMb.GetShard();
                ShardUtils.Clear(ref shard);
                ShardUtils.Set(ref shard, fieldName);
                shardMb.UpdateFromEntity();

                var shardUiButton = shardUiButtonGO.GetComponent<ShardUIButton>();
                shardUiButton.druggable = false;
                shardUiButton.hasShard = true;
                shardUiButton.showPlus = false;
                shardUiButton.cost = cost;
                if (shardUiButton.cost <= 0) shardUiButton.cost = 10;

                var button = shardUiButtonGO.GetComponent<Button>();
                button.onClick.AddListener(delegate { OnShardButtonClick(shardUiButton); });

                shardUiButtonList.Add(shardUiButton);
            }

            // calc width
            var gridWidth = (grid.cellSize.x + grid.spacing.x * 2) * shardUiButtonList.Count +
                            (grid.padding.left + grid.padding.right) * 2;
            
            ((RectTransform)gameObject.transform).SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, gridWidth);
        }

        private void OnShardButtonClick(ShardUIButton shardUiButton)
        {
            // todo check money and spend money
            
            var shardMb = shardUiButton.gameObject.GetComponentInChildren<ShardMonoBehaviour>();
            
            // if (!shardMb || !shardMb.HasShard()) return;
            if (!shardMb.ecsEntity.TryGetEntity(out var shardEntity)) return;
            
            // ref var shard = ref shardMb.GetShard();
            
            // ecsSystems.Outer<Ad>()
            //todo
            ecsSystems.Outer<AddShardToCollectionCommand>().shardPackedEntity = world.PackEntity(shardEntity);
            
            gameObject.SetActive(false);
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
        }

        private void OnDestroy()
        {
            closeButton.onClick.RemoveAllListeners();
        }

        private void OnClose()
        {
            gameObject.SetActive(false);
        }
    }
}