using System;
using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using td.features._common;
using td.features.building;
using td.features.destroy;
using td.features.ecsConverter;
using td.features.inputEvents;
using td.features.level;
using td.features.level.cells;
using td.features.movement;
using td.features.tower.mb;
using td.utils;
using UnityEngine;

namespace td.features.tower
{
    public class Tower_Converter : BaseEntity_Converter
    {
        [DI] private Tower_Aspect aspect;
        [DI] private Tower_Service towerService;
        [DI] private Destroy_Service destroyService;
        [DI] private InputEvents_Service input;
        [DI] private Building_Service buildingService;
        [DI] private Common_Service common;
        [DI] private Movement_Service movementService;

        public override ProtoWorld World()
        {
            return aspect.World();
        }

        public new void Convert(GameObject gameObject, int entity)
        {
            ref var shardTower = ref towerService.GetShardTower(entity);
            var shardTowerMB = gameObject.GetComponent<ShardTowerMonoBehaviour>();
            
            var transform = gameObject.transform;
            var coords = HexGridUtils.PositionToCell(transform.position);
            
            base.Convert(gameObject, entity);
            
            buildingService.Init(entity, Constants.Buildings.ShardTower, coords, shardTowerMB);
            towerService.GetShardTowerMBRef(entity).reference = shardTowerMB;
            input.AddHandler(entity, shardTowerMB); // todo ???
            destroyService.SetIsOnlyOnLevel(entity, true);

            var targetPoint = (Vector2)shardTowerMB.barrel.transform.localPosition;
            movementService.GetTargetPointPool(entity).Point = targetPoint;
            // shardTower.barrel = shardTowerMB.barrel ? targetPoint : new Vector2(0, 0);
            
#if UNITY_EDITOR
            if (!gameObject.GetComponent<HexGridSnaping>()) gameObject.AddComponent<HexGridSnaping>();
#endif

        }
    }
}