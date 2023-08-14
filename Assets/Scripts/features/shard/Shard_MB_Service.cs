using System;
using System.Collections.Generic;
using Leopotam.EcsProto.QoL;
using td.features.camera;
using td.features.level;
using td.features.level.cells;
using td.features.shard.mb;
using td.features.state;
using td.features.tower;
using td.utils;
using UnityEngine;

namespace td.features.shard
{
    public class Shard_MB_Service
    {
        [DI] private Shard_Calculator calc;
        [DI] private ShardsConfig config;
        [DI] private LevelMap levelMap;
        [DI] private State state;
        [DI] private Tower_Service towerService;
        [DI] private Camera_Service cameraService;

        private readonly List<ShardMonoBehaviour> list = new(20);

        private readonly ShardConrol draggableShard;

        public Shard_MB_Service()
        {
            var shardGO = GameObject.FindGameObjectWithTag(Constants.Tags.DraggableShard);
            if (shardGO == null) throw new Exception($"На сцене не найден DruggableShard");
            var shardC = shardGO.GetComponent<ShardConrol>();
            if (shardC == null) throw new Exception($"DruggableShard не содержит компонент ShardConrol");
            draggableShard = shardC;
        }

        public ShardConrol GetDraggableShard() => draggableShard;
        
        public void Add(ShardMonoBehaviour shardMB)
        {
            if (!list.Contains(shardMB))
            {
                list.Add(shardMB);
                shardMB.levelIndicator.colorTime = RandomUtils.Range(0f, 10f);
                shardMB.levelIndicator.rotation = RandomUtils.Range(0f, 180f);
            }
        }

        public void Remove(ShardMonoBehaviour shardMB)
        {
            list.Remove(shardMB);
        }

        public void Init()
        {
            Add(draggableShard.shardMB);
        }

        public void Update(float deltaTime)
        {
            //todo shard animations

            foreach (var mb in list)
            {
                if (!mb.gameObject.activeSelf || !mb.gameObject.activeInHierarchy) continue;

                var li = mb.levelIndicator;
                var h = mb.hover;

                if (li == null || h == null)
                {
                    Debug.Log("LevelIndicator or Hover is null", mb.gameObject);
                    continue;
                }
                
                var level = calc.GetShardLevel(ref mb.shardData);

                var cDivider = mb.numVertices / Constants.UI.Shard.ColorAnimNumVerticesDivider;
                var cMax = mb.Colors.Length / cDivider;
                
                li.colorTime += deltaTime * state.GetGameSpeed() * Constants.UI.Shard.ColorAnimSpeed;
                if (li.colorTime > cMax) li.colorTime -= cMax;

                var colorFloat = li.colorTime; //((li.time) % (cm));
                var colorMinIndex = Mathf.FloorToInt(colorFloat) % cMax;
                var colorMaxIndex = Mathf.CeilToInt(colorFloat) % cMax;
                
                var colorMin = mb.Colors[colorMinIndex * cDivider];
                var colorMax = mb.Colors[colorMaxIndex * cDivider];
                
                var color = colorMin != colorMax ? Color.Lerp(colorMin, colorMax, colorFloat - colorMinIndex) : colorMin;
                
                var rotationSpeed = Constants.UI.Shard.RotationSpeed + (level - 1) * Constants.UI.Shard.RotationSpeedLevelImpact;
                var rotation = li.rotation + deltaTime * state.GetGameSpeed() * rotationSpeed;
                if (rotation > 360f) rotation -= 360f;
                // Debug.Log("___________________________________");
                // Debug.Log("colorFloat = " + colorFloat);
                // Debug.Log("colorMinIndex = " + colorMinIndex);
                // Debug.Log("colorMaxIndex = " + colorMaxIndex);
                // Debug.Log("colorDeltaIndex = " + colorDeltaIndex);
                // Debug.Log("colorMin = " + colorMin);
                // Debug.Log("colorMax = " + colorMax);
                // Debug.Log("color = " + color);
                // Debug.Log("rotationSpeed = " + rotationSpeed);
                // Debug.Log("rotation = " + rotation);
                // Debug.Log("___________________________________");
                
                
                li.SetLevel(level, config);
                li.SetColor(color);
                li.SetRotation(rotation);

                mb.shardData.currentColor = color;
                
                // hack - change color in tower radius
                if (levelMap.HasCell(mb.transform.position, CellTypes.CanBuild))
                {
                    ref var cell = ref levelMap.GetCell(mb.transform.position, CellTypes.CanBuild);
                    if (
                        cell.packedBuildingEntity.HasValue &&
                        cell.packedBuildingEntity.Value.Unpack(out _, out var towerEntity)
                    )
                    {
                        var towerMB = towerService.GetTowerMB(towerEntity);
                        towerMB.radiusRenderer.startColor = color;
                        towerMB.radiusRenderer.endColor = color;
                    }
                }
                // ----

                if (h.IsVisible)
                {
                    h.SetColor(color);
                }
                
                mb.SetRotation(rotation);
            }
        }

        public void InitializeDndShard(ShardUIButton shardButton, Vector2 screenPoint)
        {
            ref var shard = ref shardButton.GetShard();

            draggableShard.SetShard(ref shard);
            /*
             * Note!
             * Copy the ID of the shard we want to move to the global shard to be moved.
             * For correct operation of the shard equality check 
             */
            draggableShard.GetShard()._id_ = shard._id_;
            draggableShard.shardMB.levelIndicator.level = shardButton.shardConrol.shardMB.levelIndicator.level;
            draggableShard.shardMB.levelIndicator.rotation = shardButton.shardConrol.shardMB.levelIndicator.rotation;
            draggableShard.shardMB.levelIndicator.colorTime = shardButton.shardConrol.shardMB.levelIndicator.colorTime;
            draggableShard.Refresh();
            draggableShard.gameObject.SetActive(true);
            draggableShard.transform.position = CameraUtils.ToWorldPoint(cameraService.GetCanvasCamera(), screenPoint);
            draggableShard.transform.FixAnchoeredPosition();
        }

        public void RevertDndShard(ShardUIButton shardButton)
        {
            draggableShard.GetShard()._id_ = 0;
            shardButton.shardConrol.shardMB.levelIndicator.level = draggableShard.shardMB.levelIndicator.level;
            shardButton.shardConrol.shardMB.levelIndicator.rotation = draggableShard.shardMB.levelIndicator.rotation;
            shardButton.shardConrol.shardMB.levelIndicator.colorTime = draggableShard.shardMB.levelIndicator.colorTime;
            draggableShard.gameObject.SetActive(false);
        }
    }
}