using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.common;
using td.components.flags;
using td.components.refs;
using td.features.shards;
using td.features.shards.config;
using td.features.shards.mb;
using td.features.towers.mb;
using td.monoBehaviours;
using td.services;
using td.utils;
using td.utils.ecs;
using UnityEngine;
using UnityEngine.TextCore.Text;

namespace td.features.towers
{
    public class TowerShowRadiusSystem : IEcsRunSystem
    {
        [Inject] private LevelMap levelMap;
        [Inject] private ShardCalculator shardCalculator;
        [Inject] private ShardsConfig config;
        [InjectShared] private SharedData shared;
        [InjectWorld] private EcsWorld world;

        private readonly EcsFilterInject<Inc<Tower, IsRadiusShown>, Exc<IsDestroyed>> towerEntities = default;

        public void Run(IEcsSystems systems)
        {
            HideAllRadius();
            ShowRadiusForTowerUnderCursor();
        }

        private void HideAllRadius()
        {
            foreach (var towerEntity in towerEntities.Value)
            {
                // todo плавное исчезновение радисуса
                world.DelComponent<IsRadiusShown>(towerEntity);

                ref var gameObjectRef = ref world.GetComponent<Ref<GameObject>>(towerEntity);
                var shardTowerMb = gameObjectRef.reference.transform.GetComponent<ShardTowerMonoBehaviour>();

                if (shardTowerMb)
                {
                    shardTowerMb.lineRenderer.enabled = false;
                }
            }
        }

        private void ShowRadiusForTowerUnderCursor()
        {
            var cursorPosition = CameraUtils.ToWorldPoint(shared.mainCamera, Input.mousePosition);
            var cell = levelMap.GetCell(cursorPosition, CellTypes.CanBuild);

            if (
                !cell ||
                !cell.HasBuilding<ShardTower>(world) ||
                !cell.HasBuilding(world, out var towerEntity) ||
                !ShardTowerUtils.HasShard(world, towerEntity)
            ) return;
            
            ref var shardTower = ref world.GetComponent<ShardTower>(towerEntity);
            ref var gameObjectRef = ref world.GetComponent<Ref<GameObject>>(towerEntity);
            ref var shard = ref ShardTowerUtils.GetShard(world, ref shardTower, out var shardEntity);

            var shardTowerMb = gameObjectRef.reference.transform.GetComponent<ShardTowerMonoBehaviour>();

            if (shardTowerMb)
            {
                // ref var shardPackedEntity = ref world.GetComponent<Shard>(shardEntity);
                var radius = shardCalculator.GetTowerRadius(ref shard);

                var color = ShardUtils.GetMixedColor(ref shard, config);;

                if (world.HasComponent<ShardColor>(shardEntity))
                {
                    ref var sc = ref world.GetComponent<ShardColor>(shardEntity);
                    color = sc.resultColor;
                }

                shardTowerMb.lineRenderer.enabled = true;

                DrawRadius(shardTowerMb.lineRenderer, radius, color);

                world.GetComponent<IsRadiusShown>(towerEntity);
            }
        }

        private void DrawRadius(LineRenderer lineRenderer, float radius, Color color)
        {
            var fov = 360f;
            var origin = Vector2.zero;
            var triangelesCount = 64;
            var angle = 0f;
            var angleIncrease = fov / triangelesCount;

            var vertices = new Vector3[triangelesCount + 1 + 1];
            var circleVerticesv = new Vector3[triangelesCount];
            lineRenderer.positionCount = triangelesCount;
            lineRenderer.startColor = color;
            lineRenderer.endColor = color;

            vertices[0] = origin;

            var vertexIndex = 1;
            var circleIndex = 0;
            for (var i = 0; i <= triangelesCount; i++)
            {
                var angleRad = angle * (Mathf.PI / 180f);
                var vectorFromAngle = new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad) * .85f);

                Vector3 vertex = origin + vectorFromAngle * radius;
                vertices[vertexIndex] = vertex;

                if (i > 0 && i <= circleVerticesv.Length)
                {
                    circleVerticesv[circleIndex] = vertices[vertexIndex];
                    circleIndex++;
                }

                vertexIndex++;
                angle -= angleIncrease;
            }

            lineRenderer.SetPositions(circleVerticesv);
        }
    }
}