using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.components.flags;
using td.monoBehaviours;
using td.services;
using td.utils;
using td.utils.ecs;
using UnityEngine;

namespace td.features.towers
{
    public class TowerShowRadiusSystem : IEcsRunSystem
    {
        [Inject] private LevelMap levelMap;
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
                var tower = towerEntities.Pools.Inc1.Get(towerEntity);
                // todo плавное исчезновение радисуса
                world.DelComponent<IsRadiusShown>(towerEntity);

                if (tower.radiusGameObject)
                {
                    tower.radiusGameObject.SetActive(false);
                }
            }
        }

        private void ShowRadiusForTowerUnderCursor()
        {
            var cursorPosition = CameraUtils.ToWorldPoint(Input.mousePosition);
            var cell = levelMap.GetCell(cursorPosition, CellTypes.CanWalk);

            // if (cell is { BuildingPackedEntity: not null } and { HasBuilding: true } &&
            // TODo!!!!
            // if (cell.HasBuilding) {}
            // //ToDo
            //     cell.Buildings[0].Value.Unpack(world, out var towerEntity) &&
            //     !world.HasComponent<IsDisabled>(towerEntity) &&
            //     world.TryGetComponent<Tower>(towerEntity, out var tower) &&
            //     tower.radiusGameObject
            //    )
            // {
            //     world.AddComponent<IsRadiusShown>(towerEntity);
            //     
            //     if (tower.radiusGameObject)
            //     {
            //         tower.radiusGameObject.SetActive(true);
            //     }
            // }
        }
    }
}