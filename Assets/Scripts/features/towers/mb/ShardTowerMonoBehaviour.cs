using Leopotam.EcsLite;
using td.features.shards;
using td.features.shards.ui;
using td.features.ui;
using td.monoBehaviours;
using td.utils;
using td.utils.ecs;
using UnityEngine;

namespace td.features.towers.mb
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(EcsEntity))]
    public class ShardTowerMonoBehaviour : MonoBehaviour
    {
        private EcsEntity ecsEntity;
        private ShardInfoPanel infoPanel;
        public LineRenderer lineRenderer;

        private void Start()
        {
            ecsEntity ??= GetComponent<EcsEntity>();
            lineRenderer ??= GetComponent<LineRenderer>();
            infoPanel ??= FindObjectOfType<ShardInfoPanel>();
        }
        
        public void UpdateEntity(EcsWorld world, int entity)
        {
            world.GetComponent<ShardTower>(entity);
        }

        public void Update()
        {
            // Todo optimize
            var cursorPosition = CameraUtils.ToWorldPoint(Input.mousePosition);
            var cursorCell = HexGridUtils.PositionToCell(cursorPosition);

            var cell = HexGridUtils.PositionToCell(transform.position);

            if (cell == cursorCell && ecsEntity.TryGetEntity(out var towerEntity))
            {
                ref var tower = ref DI.GetWorld().GetComponent<ShardTower>(towerEntity);

                if (tower.shard.Unpack(DI.GetWorld(), out var shardEntity))
                {
                    ref var shard = ref DI.GetWorld().GetComponent<Shard>(shardEntity);
                    infoPanel.ShowInfo(ref shard);
                }
            }
        }
    }
}