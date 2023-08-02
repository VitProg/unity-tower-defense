using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.features._common;
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
        private readonly EcsWorldInject world;
        private readonly EcsInject<Tower_Service> towerService;
        private readonly EcsInject<Common_Service> common;
        private readonly EcsInject<InputEvents_Service> input;

        public new void Convert(GameObject gameObject, int entity)
        {
            base.Convert(gameObject, entity);
            
            ref var tower = ref towerService.Value.GetTower(entity);
            tower.coords = HexGridUtils.PositionToCell(gameObject.transform.position);

            common.Value.SetIsOnlyOnLevel(entity, true);

            var towerMB = gameObject.GetComponent<TowerMonoBehaviour>();
            towerService.Value.GetTowerMBRef(entity).reference = towerMB;
            
            tower.barrel = towerMB.barrel ? (Vector2)towerMB.barrel.transform.localPosition : new Vector2(0, 0);

            input.Value.GetCicleCollider(entity).SetRadius(towerMB.size.x, towerMB.size.y / towerMB.size.x); // todo calc or move to contatnts
            input.Value.AddHandler(entity, towerMB);
            
#if UNITY_EDITOR
            if (!gameObject.GetComponent<HexGridSnaping>()) gameObject.AddComponent<HexGridSnaping>();
#endif

            
            if (gameObject.TryGetComponent(out ShardTowerMonoBehaviour shardTowerMB))
            {
                towerService.Value.GetShardTower(entity);
                towerService.Value.GetShardTowerMBRef(entity).reference = shardTowerMB;
                input.Value.AddHandler(entity, shardTowerMB);
            }
        }
    }
}