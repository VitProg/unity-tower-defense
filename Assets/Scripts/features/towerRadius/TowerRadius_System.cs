using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.features._common;
using td.features.shard;
using td.features.tower;
using td.features.tower.components;
using td.features.towerRadius.bus;
using Unity.Collections;
using UnityEngine;

namespace td.features.towerRadius
{
    public class TowerRadius_System : IEcsInitSystem, IEcsDestroySystem
    {
        private readonly EcsInject<IEventBus> events;
        private readonly EcsInject<Common_Service> common;
        private readonly EcsInject<Tower_Service> towerService;
        private readonly EcsInject<Shard_Service> shardService;
        private readonly EcsInject<SharedData> shared;

        private readonly EcsFilterInject<Inc<Tower>, ExcludeNotAlive> towerFilter = default;

        public void Init(IEcsSystems systems)
        {
            events.Value.Global.ListenTo<Command_Tower_ShowRadius>(ShowRadius);
            events.Value.Global.ListenTo<Command_Tower_HideRadius>(HideRadius);
        }

        public void Destroy(IEcsSystems systems)
        {
            events.Value.Global.RemoveListener<Command_Tower_ShowRadius>(ShowRadius);
            events.Value.Global.RemoveListener<Command_Tower_HideRadius>(HideRadius);
        }

        private void ShowRadius(ref Command_Tower_ShowRadius item)
        {
            if (!item.towerEntity.Unpack(out var world, out var towerEntity)) return;

            var radius = 0f;
            var color = Color.grey;
            var isDragging = shared.Value.draggableShard.gameObject.activeSelf;

            if (shardService.Value.HasShardInTower(towerEntity, out var shardEntity))
            {
                ref var shard = ref shardService.Value.GetShard(shardEntity);
                if (isDragging)
                {
                    ref var draggingShard = ref shared.Value.draggableShard.GetShard();
                    var combinerShard = shard.MakeCopy();
                    combinerShard.CombineWith(ref draggingShard);
                    radius = shardService.Value.GetRadius(ref combinerShard);
                }
                else
                {
                    radius = shardService.Value.GetRadius(ref shard);   
                }

                color = shard.currentColor;
            }
            else if (isDragging)
            {
                ref var shard = ref shared.Value.draggableShard.GetShard();
                radius = shardService.Value.GetRadius(ref shard);
                color = shard.currentColor;
            }

            if (radius < 0.1f) return;
            
            HideAllRadiuses();

            DrawRadius(towerService.Value.GetTowerMB(towerEntity).radiusRenderer, radius, color);
        }

        private void HideRadius(ref Command_Tower_HideRadius item)
        {
            if (!events.Value.Global.Has<Command_Tower_ShowRadius>())
            {
                HideAllRadiuses();
            }
        }
        
        private void HideAllRadiuses() // ALL
        {
            //todo hide radius
            var count = towerFilter.Value.GetEntitiesCount();
            var arr = towerFilter.Value.GetRawEntities();
            for (var i = 0; i < count; i++)
            {
                var entity = arr[i];
                var towerMB = towerService.Value.GetTowerMB(entity);
                towerMB.radiusRenderer.enabled = false;
            }
        }

        ///
        /// 
        private void DrawRadius(LineRenderer radiusRenderer, float radius, Color color)
        {
            var fov = 360f;
            var origin = Vector2.zero;
            var triangelesCount = (int)(16 * (radius * 0.5f + 1f));
            var angle = 0f;
            var angleIncrease = fov / triangelesCount;
            var vertices = new Vector3[triangelesCount + 1 + 1];
            var circleVerticesv = new NativeArray<Vector3>(triangelesCount, Allocator.Temp);
            radiusRenderer.positionCount = triangelesCount;
            radiusRenderer.startColor = color;
            radiusRenderer.endColor = color;
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

            radiusRenderer.SetPositions(circleVerticesv);
            radiusRenderer.enabled = true;
        }
    }
}