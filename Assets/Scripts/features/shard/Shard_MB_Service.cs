using System.Collections.Generic;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.features._common;
using td.features.level;
using td.features.shard.mb;
using td.features.state;
using td.features.tower;
using td.monoBehaviours;
using td.utils;
using UnityEditor;
using UnityEngine;

namespace td.features.shard
{
    public class Shard_MBService
    {
        private readonly EcsInject<Shard_Calculator> calc;
        private readonly EcsInject<ShardsConfig> config;
        private readonly EcsInject<SharedData> shared;
        private readonly EcsInject<LevelMap> levelMap;
        private readonly EcsInject<State> state;
        private readonly EcsInject<Tower_Service> towerService;
        private readonly EcsWorldInject world;

        private readonly List<ShardMonoBehaviour> list = new(20);

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
            Add(shared.Value.draggableShard.shardMB);
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
                
                var level = calc.Value.GetShardLevel(ref mb.shardData);

                var cDivider = mb.numVertices / Constants.UI.Shard.ColorAnimNumVerticesDivider;
                var cMax = mb.Colors.Length / cDivider;
                
                li.colorTime += deltaTime * state.Value.GameSpeed * Constants.UI.Shard.ColorAnimSpeed;
                if (li.colorTime > cMax) li.colorTime -= cMax;

                var colorFloat = li.colorTime; //((li.time) % (cm));
                var colorMinIndex = Mathf.FloorToInt(colorFloat) % cMax;
                var colorMaxIndex = Mathf.CeilToInt(colorFloat) % cMax;
                
                var colorMin = mb.Colors[colorMinIndex * cDivider];
                var colorMax = mb.Colors[colorMaxIndex * cDivider];
                
                var color = colorMin != colorMax ? Color.Lerp(colorMin, colorMax, colorFloat - colorMinIndex) : colorMin;
                
                var rotationSpeed = Constants.UI.Shard.RotationSpeed + (level - 1) * Constants.UI.Shard.RotationSpeedLevelImpact;
                var rotation = li.rotation + deltaTime * state.Value.GameSpeed * rotationSpeed;
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
                
                
                li.SetLevel(level, config.Value);
                li.SetColor(color);
                li.SetRotation(rotation);

                mb.shardData.currentColor = color;
                
                // hack - change color in tower radius
                if (levelMap.Value.HasCell(mb.transform.position, CellTypes.CanBuild))
                {
                    ref var cell = ref levelMap.Value.GetCell(mb.transform.position, CellTypes.CanBuild);
                    if (
                        cell.packedBuildingEntity.HasValue &&
                        cell.packedBuildingEntity.Value.Unpack(world.Value, out var towerEntity)
                    )
                    {
                        var towerMB = towerService.Value.GetTowerMB(towerEntity);
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
            
            var dndShard = shared.Value.draggableShard;
            
            dndShard.SetShard(ref shard);
            /*
             * Note!
             * Copy the ID of the shard we want to move to the global shard to be moved.
             * For correct operation of the shard equality check 
             */
            dndShard.GetShard()._id_ = shard._id_;
            dndShard.shardMB.levelIndicator.level = shardButton.shardConrol.shardMB.levelIndicator.level;
            dndShard.shardMB.levelIndicator.rotation = shardButton.shardConrol.shardMB.levelIndicator.rotation;
            dndShard.shardMB.levelIndicator.colorTime = shardButton.shardConrol.shardMB.levelIndicator.colorTime;
            dndShard.Refresh();
            dndShard.gameObject.SetActive(true);
            dndShard.transform.position = CameraUtils.ToWorldPoint(shared.Value.canvasCamera, screenPoint);
            dndShard.transform.FixAnchoeredPosition();
        }

        public void RevertDndShard(ShardUIButton shardButton)
        {
            var dndShard = shared.Value.draggableShard;
            dndShard.GetShard()._id_ = 0;
            shardButton.shardConrol.shardMB.levelIndicator.level = dndShard.shardMB.levelIndicator.level;
            shardButton.shardConrol.shardMB.levelIndicator.rotation = dndShard.shardMB.levelIndicator.rotation;
            shardButton.shardConrol.shardMB.levelIndicator.colorTime = dndShard.shardMB.levelIndicator.colorTime;
            dndShard.gameObject.SetActive(false);
        }
    }
}