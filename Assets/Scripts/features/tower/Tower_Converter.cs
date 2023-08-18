using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using td.features.building;
using td.features.destroy;
using td.features.ecsConverter;
using td.features.inputEvents;
using td.features.tower.mb;
using td.monoBehaviours;
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

        public override ProtoWorld World()
        {
            return aspect.World();
        }

        public new void Convert(GameObject gameObject, int entity)
        {
            base.Convert(gameObject, entity);

            buildingService.Init(entity, "shard_tower");
            
            ref var tower = ref towerService.GetTower(entity);
            tower.coords = HexGridUtils.PositionToCell(gameObject.transform.position);

            destroyService.SetIsOnlyOnLevel(entity, true);

            var towerMB = gameObject.GetComponent<TowerMonoBehaviour>();
            towerService.GetTowerMBRef(entity).reference = towerMB;
            
            tower.barrel = towerMB.barrel ? (Vector2)towerMB.barrel.transform.localPosition : new Vector2(0, 0);

            // input.GetCicleCollider(entity).SetRadius(towerMB.size.x, towerMB.size.y / towerMB.size.x); // todo calc or move to contatnts
            // input.GetHexCellCollider(entity);
            input.AddHandler(entity, towerMB);
            
#if UNITY_EDITOR
            if (!gameObject.GetComponent<HexGridSnaping>()) gameObject.AddComponent<HexGridSnaping>();
#endif

            
            if (gameObject.TryGetComponent(out ShardTowerMonoBehaviour shardTowerMB))
            {
                towerService.GetShardTower(entity);
                towerService.GetShardTowerMBRef(entity).reference = shardTowerMB;
                input.AddHandler(entity, shardTowerMB);
            }
        }
    }
}