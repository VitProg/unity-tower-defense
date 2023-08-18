using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using Leopotam.Types;
using td.features.eventBus;
using td.features.shard;
using td.features.tower.towerRadius.bus;
using Unity.Collections;
using UnityEngine;

namespace td.features.tower.towerRadius
{
    public class TowerRadius_System : IProtoInitSystem, IProtoDestroySystem
    {
        [DI] private Tower_Aspect aspect;
        [DI] private EventBus events;
        [DI] private Tower_Service towerService;
        [DI] private Shard_Service shardService;
        [DI] private Shard_MB_Service mbShardService;

        public void Init(IProtoSystems systems)
        {
            events.global.ListenTo<Command_Tower_ShowRadius>(ShowRadius);
            events.global.ListenTo<Command_Tower_HideRadius>(HideRadius);
        }

        public void Destroy()
        {
            events.global.RemoveListener<Command_Tower_ShowRadius>(ShowRadius);
            events.global.RemoveListener<Command_Tower_HideRadius>(HideRadius);
        }

        private void ShowRadius(ref Command_Tower_ShowRadius item)
        {
            if (!item.towerEntity.Unpack(out var world, out var towerEntity)) return;

            var radius = 0f;
            var color = Color.grey;
            var dndShard = mbShardService.GetDraggableShard();
            var isDragging = dndShard.gameObject.activeSelf;

            if (shardService.HasShardInTower(towerEntity, out var shardEntity))
            {
                ref var shard = ref shardService.GetShard(shardEntity);
                if (isDragging)
                {
                    ref var draggingShard = ref dndShard.GetShard();
                    var combinerShard = shard.MakeCopy();
                    combinerShard.CombineWith(ref draggingShard);
                    radius = shardService.GetRadius(ref combinerShard);
                }
                else
                {
                    radius = shardService.GetRadius(ref shard);   
                }

                color = shard.currentColor;
            }
            else if (isDragging)
            {
                ref var shard = ref dndShard.GetShard();
                radius = shardService.GetRadius(ref shard);
                color = shard.currentColor;
            }

            if (radius < 0.1f) return;
            
            HideAllRadiuses();

            DrawRadius(towerService.GetTowerMB(towerEntity).radiusRenderer, radius, color);
        }

        private void HideRadius(ref Command_Tower_HideRadius item)
        {
            if (!events.global.Has<Command_Tower_ShowRadius>())
            {
                HideAllRadiuses();
            }
        }
        
        private void HideAllRadiuses() // ALL
        {
            foreach (var entity in aspect.itTower)
            {
                var towerMB = towerService.GetTowerMB(entity);
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
                var angleRad = angle * (MathFast.Pi / 180f);
                var vectorFromAngle = new Vector2(MathFast.Cos(angleRad), MathFast.Sin(angleRad) * .85f);
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