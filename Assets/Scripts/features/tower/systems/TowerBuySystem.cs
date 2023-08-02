// using Leopotam.EcsLite;
// using Leopotam.EcsLite.Di;
// using td.features._common;
// using td.features._common.components;
// using td.features.dragNDrop;
// using td.features.dragNDrop.evets;
// using td.features.dragNDrop.flags;
// using td.features.level;
// using td.features.state;
// using td.features.tower.components;
// using td.monoBehaviours;
// using td.utils;
// using UnityEngine;
//
// namespace td.features.tower.systems
// {
//     // todo recreate - need build cicle menu on cell pressed 
//     public class TowerBuySystem : IEcsRunSystem
//     {
//         private readonly EcsInject<IState> state;
//         private readonly EcsInject<LevelMap> levelMap;
//         private readonly EcsInject<Prefab_Service> prefabService;
//         
//         private readonly EcsInject<Common_Service> common;
//         private readonly EcsInject<DragNDrop_Service> dndService;
//
//         private readonly EcsWorldInject world;
//
//         private readonly EcsInject<SharedData> shared;
//
//         private readonly EcsFilterInject<Inc<Tower, DragEndEvent, Ref<GameObject>>, ExcludeNotAlive> dragEndEventEntities = default;
//         private readonly EcsFilterInject<Inc<Tower, IsDragging, Ref<GameObject>>, ExcludeNotAlive> draggableEntities = default;
//
//         private GameObject buildingsContainer;
//
//         public void Run(IEcsSystems _)
//         {
//             foreach (var draggableEntity in draggableEntities.Value)
//             {
//                 ref var refGameObject = ref draggableEntities.Pools.Inc3.Get(draggableEntity);
//
//                 var position = refGameObject.reference.transform.position;
//                 var coords = HexGridUtils.PositionToCell(position);
//                 
//                 // var cell = levelMap.Value.HasCell(coords.x, coords.y, CellTypes.CanBuild);
//                 // var canBuild = cell && !cell.HasBuilding(world.Value);
//                 
//                 // if (shared.Value.hightlightGrid)
//                 // {
//                     // shared.Value.hightlightGrid.state = canBuild ? GridHightlightState.Fine : GridHightlightState.Error;
//                 // }
//                 
//                 // dndService.Value.SetCanDrop(draggableEntity, canBuild);
//             }
//             
//             //////////////////
//
//             foreach (var dragEndEntity in dragEndEventEntities.Value)
//             {
//                 ref var tower = ref draggableEntities.Pools.Inc1.Get(dragEndEntity);
//                 ref var refGameObject = ref draggableEntities.Pools.Inc3.Get(dragEndEntity);
//
//                 var position = refGameObject.reference.transform.position;
//                 var cell = levelMap.Value.GetCell(position, CellTypes.CanBuild);
//  
//                 // todo надо бы проверять до того как начали "тянуть" башню
//                 if (cell && !cell.HasBuilding(world.Value) && state.Value.Energy - tower.cost != 0)
//                 {
//                     state.Value.Energy -= tower.cost;
//                     // todo
//                     cell.buildingPackedEntity = world.Value.PackEntity(dragEndEntity);
//                 }
//                 else
//                 {
//                     common.Value.SafeDelete(dragEndEntity);
//                 }
//
//                 dndService.Value.Clear(dragEndEntity);
//                 //..Value.dragEndEventPool.Value.SafeDel(dragEndEntity));
//             }
//         }
//     }
// }