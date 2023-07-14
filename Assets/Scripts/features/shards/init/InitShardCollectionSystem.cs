using System;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.common;
using td.components.flags;
using td.components.refs;
using td.features.levels;
using td.features.shards.commands;
using td.features.shards.flags;
using td.features.shards.mb;
using td.services;
using td.services.ecsConverter;
using td.utils.ecs;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace td.features.shards.init
{
    public class InitShardCollectionSystem : IEcsInitSystem, IEcsRunSystem
    {
        [Inject] private LevelMap levelMap;
        [Inject] private EntityConverters converters;
        [Inject] private PrefabService prefabService;
        [InjectWorld] private EcsWorld world;
        [InjectShared] private SharedData shared;
        [InjectSystems] private IEcsSystems systems;
        
        private readonly EcsFilterInject<Inc<LevelLoadedOuterEvent>> loadedEventEntities = Constants.Worlds.Outer;
        private readonly EcsFilterInject<Inc<Shard, ShardInCollection, Ref<GameObject>>, Exc<IsDestroyed>> shardInCollectionEntities = default;

        public void Init(IEcsSystems systems)
        {
            InitShardCollection();
        }
        
        public void Run(IEcsSystems systems)
        {
            foreach (var entity in loadedEventEntities.Value)
            {
                PrepareForNewLevel();                
                systems.OuterSingle<UIRefreshShardCollectionOuterCommand>();
                break;
            }
        }

        private void PrepareForNewLevel()
        {
            var shardEntitiseInCollection = shardInCollectionEntities.Value.ToArray();
            var startedShards = levelMap.LevelConfig.HasValue ? levelMap.LevelConfig.Value.startedShards : new Shard[] { };
            
            // reset shards in collection
            var index = 0;
            foreach (var shardEntity in shardEntitiseInCollection)
            {
                var shardGO = shardInCollectionEntities.Pools.Inc3.Get(shardEntity);
                var shardUiButton = shardGO.reference.transform.GetComponentInParent<ShardUIButton>();

                var startedShard = index < startedShards.Length ? startedShards[index] : (Shard?)null;
                var hasShard = startedShard != null;
                
                if (hasShard)
                {
                    ref var shard = ref shardUiButton.shardUI.GetShard();
                    var startedShardValue = startedShard.Value;
                    ShardUtils.Copy(ref shard, ref startedShardValue);
                    world.DelComponent<IsDisabled>(shardEntity);
                    shardUiButton.shardUI.UpdateFromEntity();
                }
                else
                {
                    world.GetComponent<IsDisabled>(shardEntity);
                }
                
                index++;
            }
        }
        
        private void InitShardCollection()
        {
            var shardUiButtonPrefab = prefabService.GetPrefab(PrefabCategory.Shard, "ShardUIButton");
            var ui = shared.shardCollection;

            var draggableShardGO = shared.draggableShard.gameObject;
            draggableShardGO.gameObject.SetActive(false);
            converters.Convert<Shard>(draggableShardGO, out var draggableShardEntity);
            shared.draggableShardPackedEntity = world.PackEntity(draggableShardEntity);

            foreach (var button in ui.transform.GetComponentsInChildren<ShardUIButton>())
            {
                Object.Destroy(button.gameObject);
            }

            var plusShowed = false;
            for (var index = 0; index < Constants.UI.MaxShardsInCollection; index++)
            {
                // init shard GO
                var shardUiButtonGO = Object.Instantiate(shardUiButtonPrefab, ui.grid.gameObject.transform);
                var shardUiButton = shardUiButtonGO.GetComponent<ShardUIButton>();
                shardUiButton.druggable = true;
                shardUiButton.hasShard = false;
                shardUiButton.showPlus = !plusShowed;
                shardUiButton.cost = 0;

                if (shardUiButton.showPlus) plusShowed = true;
                
                var button = shardUiButtonGO.GetComponent<Button>();
                button.onClick.AddListener(delegate { OnShardButtonClick(shardUiButton); });

                var shardMb = shardUiButtonGO.GetComponentInChildren<ShardMonoBehaviour>();
                if (!converters.Convert<Shard>(shardMb.gameObject, out var shardEntity))
                {
                    throw new NullReferenceException($"Failed to convert GameObject {shardMb.gameObject.name}");
                }
                
                world.GetComponent<ShardInCollection>(shardEntity);
                world.GetComponent<IsDisabled>(shardEntity);
                
                shardUiButton.Refresh();
            }
        }
        
        private void OnShardButtonClick(ShardUIButton shardUiButton)
        {
            if (shardUiButton.showPlus)
            {
                var buttonPosition = shardUiButton.transform.position;
                systems.OuterSingle<UIShowShardStoreOuterCommand>().x = buttonPosition.x;
            }
            else
            {
                systems.OuterSingle<UIHideShardStoreOuterCommand>();
            }
        }
    }
}