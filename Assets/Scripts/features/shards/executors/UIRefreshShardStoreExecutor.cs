using System;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.common;
using td.components.flags;
using td.features.shards.commands;
using td.features.shards.flags;
using td.features.shards.mb;
using td.services;
using td.services.ecsConverter;
using td.utils.ecs;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace td.features.shards.executors
{
    public class UIRefreshShardStoreExecutor : IEcsRunSystem, IEcsInitSystem
    {
        [Inject] private LevelMap levelMap;
        [Inject] private EntityConverters converters;
        [InjectWorld] private EcsWorld world;
        [InjectShared] private SharedData shared;
        [InjectSystems] private IEcsSystems systems;

        private readonly EcsFilterInject<Inc<UIRefreshShardStoreOuterCommand>> eventEntities = Constants.Worlds.Outer;
        private readonly EcsFilterInject<Inc<Shard, ShardInStore>, Exc<IsDisabled, IsDestroyed>> shardEntities = default;

        private GameObject shardUiButtonPrefab;

        public void Init(IEcsSystems unused)
        {
            shardUiButtonPrefab = (GameObject)Resources.Load("Prefabs/ShardUIButton", typeof(GameObject));
        }
        
        public void Run(IEcsSystems systems)
        {
            foreach (var eventEntity in eventEntities.Value)
            {
                Refresh();
                break;
            }
            systems.CleanupOuter(eventEntities);
        }

        private void Refresh()
        {
            var ui = shared.shardStore;
            
            // todo Clear();
            Clear();

            foreach (var shardEntity in shardEntities.Value)
            {
                var cost = shardEntities.Pools.Inc2.Get(shardEntity).cost;
                
                var shardUiButtonGO = Object.Instantiate(shardUiButtonPrefab, ui.grid.gameObject.transform);
                
                var shardUiButton = shardUiButtonGO.GetComponent<ShardUIButton>();
                shardUiButton.druggable = false;
                shardUiButton.hasShard = true;
                shardUiButton.showPlus = false;
                shardUiButton.cost = cost;
                
                var shardMb = shardUiButtonGO.GetComponentInChildren<ShardMonoBehaviour>();
                if (!converters.Convert<Shard>(shardMb.gameObject, shardEntity))
                {
                    throw new NullReferenceException($"Failed to convert GameObject {shardMb.gameObject.name}");
                }

                var shardPackedEntity = world.PackEntity(shardEntity);
                
                var button = shardUiButtonGO.GetComponent<Button>();
                button.onClick.AddListener(delegate { OnShardButtonClick(shardPackedEntity, cost); });

                shardUiButton.Refresh();
            }

            var gridWidth = (ui.grid.cellSize.x + ui.grid.spacing.x * 2) * (shardEntities.Value.GetEntitiesCount() + 1);
            ((RectTransform)ui.gameObject.transform).SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, gridWidth);
            

        }
        
        private void OnShardButtonClick(EcsPackedEntity shardPackedEntity, int cost)
        {
            if (!shardPackedEntity.Unpack(world, out var shardEntity)) return;
            
            ref var buyCommand = ref systems.Outer<BuyShardCommand>();
            buyCommand.shardPackedEntity = world.PackEntity(shardEntity);
            buyCommand.cost = cost;
        }
        
        private void Clear()
        {
            var ui = shared.shardStore;
            
            //todo
            foreach (var shardUiButton in ui.transform.GetComponentsInChildren<ShardUIButton>())
            {
                // todo cleanup in ECS
                var button = shardUiButton.gameObject.GetComponent<Button>();
                button.onClick.RemoveAllListeners();
                Object.Destroy(shardUiButton.gameObject);
            }
        }
    }
}