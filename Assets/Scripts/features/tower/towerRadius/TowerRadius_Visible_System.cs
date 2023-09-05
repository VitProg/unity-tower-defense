using System;
using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using Leopotam.Types;
using td.features.building;
using td.features.eventBus;
using td.features.level;
using td.features.level.cells;
using td.features.shard;
using td.features.shard.shardCollection;
using td.features.tower.bus;
using td.features.tower.towerRadius.bus;
using td.utils.ecs;
using Unity.Collections;
using UnityEngine;

namespace td.features.tower.towerRadius {
    public class TowerRadius_Visible_System : ProtoIntervalableRunSystem, IProtoInitSystem, IProtoDestroySystem {
        [DI] private Tower_Aspect aspect;
        [DI] private EventBus events;
        [DI] private Building_Service buildingService;
        [DI] private Tower_Service towerService;
        [DI] private Shard_Service shardService;
        [DI] private Shard_MB_Service mbShardService;
        [DI] private Level_State levelState;
        [DI] private ShardCollection_State collectionState;

        public void Init(IProtoSystems systems) {
            events.global.ListenTo<Event_Tower_Hovered>(OnHovered);
            events.global.ListenTo<Event_Tower_UnHovered>(OnLeave);
            events.global.ListenTo<Command_ShowTowerRadius>(OnCommandShowTowerRadius);
        }

        public void Destroy() {
            events.global.RemoveListener<Event_Tower_Hovered>(OnHovered);
            events.global.RemoveListener<Event_Tower_UnHovered>(OnLeave);
            events.global.RemoveListener<Command_ShowTowerRadius>(OnCommandShowTowerRadius);
        }

        // ----------------------------------------------------------------

        private void OnHovered(ref Event_Tower_Hovered ev) {
            if (!ev.Tower.Unpack(out _, out _)) return;
            events.global.Add<Command_ShowTowerRadius>().Tower = ev.Tower;
        }

        private void OnCommandShowTowerRadius(ref Command_ShowTowerRadius ev) {
            if (!ev.Tower.Unpack(out var world, out var towerEntity)) return;

            ref var building = ref buildingService.GetBuilding(towerEntity);
            ref var shardTower = ref towerService.GetShardTower(towerEntity);

            if (!levelState.HasCell(building.coords, CellTypes.CanBuild)) return;

            ref var cell = ref levelState.GetCell(building.coords, CellTypes.CanBuild);

            if (cell.IsEmpty) return;
            if (!cell.HasBuilding()) return;
            // if (!cell.HasShard()) return;

            Debug.Log(">>> OnCommandShowTowerRadius");

            //

            var radius = shardTower.radius;
            var color = Color.grey;
            // var dndShard = mbShardService.GetDraggableShard();

            Debug.Log(">>> collectionState.GetOperation = " + collectionState.GetOperationType());
            Debug.Log(">>> collectionState.IsDragging = " + collectionState.IsDragging());
            if (collectionState.IsInsertOperation() && collectionState.IsDragging()) {
                ref var shard = ref collectionState.GetDraggableShard();
                color = shard.currentColor;
                radius = shard.radius;
                // todo dinamic change color
            }

            if (collectionState.IsCombineOperation() && collectionState.GetOperationTargetEntity().EqualsTo(ev.Tower)) {
                ref var shard = ref collectionState.GetCombinedShard();
                color = shard.currentColor;
                radius = shard.radius;
                // todo dinamic change color
            }

            if (radius < 0.1f) return;

            HideAllRadiuses();

            DrawRadius(towerService.GetShardTowerMB(towerEntity).radiusRenderer, radius, color);
        }

        private void OnLeave(ref Event_Tower_UnHovered ev) {
            Debug.Log(">>> OnLeave");
            HideAllRadiuses();
        }

        /*private void ShowRadius(ref Command_Tower_ShowRadius item)
        {
            
        }
        
        private void HideRadius(ref Command_Tower_HideRadius item)
        {
            if (!events.global.Has<Command_Tower_ShowRadius>())
            {
                HideAllRadiuses();
            }
        }*/

        private void HideAllRadiuses() // ALL
        {
            Debug.Log(">>> HideAllRadiuses");
            foreach (var entity in aspect.itShardTower) {
                var towerMB = towerService.GetShardTowerMB(entity);
                towerMB.radiusRenderer.enabled = false;
            }
        }

        ///
        /// 
        private void DrawRadius(LineRenderer radiusRenderer, float radius, Color color) {
            Debug.Log(">>> DrawRadius " + radius + " " + color);

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
            for (var i = 0; i <= triangelesCount; i++) {
                var angleRad = angle * (MathFast.Pi / 180f);
                var vectorFromAngle = new Vector2(MathFast.Cos(angleRad), MathFast.Sin(angleRad) * .85f);
                Vector3 vertex = origin + vectorFromAngle * radius;
                vertices[vertexIndex] = vertex;
                if (i > 0 && i <= circleVerticesv.Length) {
                    circleVerticesv[circleIndex] = vertices[vertexIndex];
                    circleIndex++;
                }

                vertexIndex++;
                angle -= angleIncrease;
            }

            radiusRenderer.SetPositions(circleVerticesv);
            radiusRenderer.enabled = true;
        }

        public override void IntervalRun(float deltaTime) {
            foreach (var towerEntity in aspect.itShardTower) {
                var mb = towerService.GetShardTowerMB(towerEntity);
                if (!mb.radiusRenderer.enabled) continue;

                if (collectionState.IsInsertOperation() && collectionState.IsDragging()) {
                    ref var insertedShard = ref collectionState.GetDraggableShard();
                    mb.radiusRenderer.startColor = insertedShard.currentColor;
                    mb.radiusRenderer.endColor = insertedShard.currentColor;
                    Debug.Log(">>> Tower Radius Set Color " + insertedShard.currentColor);
                    continue;
                }

                if (collectionState.IsCombineOperation() &&
                    collectionState.GetOperationTargetEntity().Unpack(out _, out var opTowerEntity) &&
                    opTowerEntity == towerEntity
                ) {
                    ref var combinedShard = ref collectionState.GetCombinedShard();
                    mb.radiusRenderer.startColor = combinedShard.currentColor;
                    mb.radiusRenderer.endColor = combinedShard.currentColor;
                    Debug.Log(">>> Tower Radius Set Color " + combinedShard.currentColor);
                    continue;
                }

                ref var building = ref buildingService.GetBuilding(towerEntity);
                
                if (!levelState.HasCell(building.coords)) continue;
                
                ref var cell = ref levelState.GetCell(building.coords);
                if (!cell.HasBuilding() || !cell.HasShard()) continue;

                ref var shard = ref shardService.GetShard(cell.packedShardEntity, out var shardEntity);
                mb.radiusRenderer.startColor = shard.currentColor;
                mb.radiusRenderer.endColor = shard.currentColor;
                Debug.Log(">>> Tower Radius Set Color " + shard.currentColor);
            }
        }

        public TowerRadius_Visible_System(float interval, float timeShift, Func<float> getDeltaTime) : base(interval, timeShift, getDeltaTime) { }
    }
}
