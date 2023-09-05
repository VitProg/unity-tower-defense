// // using System.Runtime.CompilerServices;
// using NaughtyAttributes;
// using td.features._common.interfaces;
// using td.features.eventBus;
// using td.features.inputEvents;
// using td.features.level;
// using td.features.tower.towerRadius.bus;
// using td.utils.di;
// using td.utils.ecs;
// using UnityEngine;
//
// namespace td.features.tower.mb
// {
//     [DisallowMultipleComponent]
//     [RequireComponent(typeof(EcsEntity))]
//     [SelectionBase]
//     public class TowerMonoBehaviour : MonoBehaviour, IInputEventsHandler
//     {
//         [Required] public EcsEntity ecsEntity;
//         [Required] public GameObject barrel;
//         [Required] public SpriteRenderer sprite;
//         [Required] public LineRenderer radiusRenderer;
//         
//         private EventBus Events =>  ServiceContainer.Get<EventBus>();
//         private LevelMap LevelMap =>  ServiceContainer.Get<LevelMap>();
//
//         [MethodImpl(MethodImplOptions.AggressiveInlining)]
//         public void OnPointerEnter(float x, float y)
//         {
//             // Debug.Log($"OnPointerEnter [{x}, {y}]");
//             sprite.color = new Color(1.0f, 0.9f, 0.9f, 1.0f);
//             if (ecsEntity.packedEntity.HasValue)
//             {
//                 //todo add radius preview if shard in hand
//                 Events.global.Add<Command_Tower_ShowRadius>().towerEntity = ecsEntity.packedEntity.Value;
//             }
//         }
//
//         [MethodImpl(MethodImplOptions.AggressiveInlining)]
//         public void OnPointerLeave(float x, float y)
//         {
//             // Debug.Log($"OnPointerLeave [{x}, {y}]");
//             // todo hide radius
//             sprite.color = Color.white;
//             if (ecsEntity.packedEntity.HasValue)
//             {
//                 //todo add radius preview if shard in hand
//                 Events.global.Add<Command_Tower_HideRadius>().towerEntity = ecsEntity.packedEntity.Value;
//             }
//         }
//
//         [MethodImpl(MethodImplOptions.AggressiveInlining)]
//         public void OnPointerDown(float x, float y)
//         {
//             // Debug.Log($"OnPointerDown [{x}, {y}]");
//             if (ecsEntity.packedEntity.HasValue)
//             {
//                 Events.global.Add<Command_Tower_ShowRadius>().towerEntity = ecsEntity.packedEntity.Value;
//             }
//         }
//
//         [MethodImpl(MethodImplOptions.AggressiveInlining)]
//         public void OnPointerUp(float x, float y, bool inside)
//         {
//             if (ecsEntity.packedEntity.HasValue)
//             {
//                 // todo show radius on next frame!
//                 Events.global.Add<Command_Tower_ShowRadius>().towerEntity = ecsEntity.packedEntity.Value;
//             }
//         }
//
//         [MethodImpl(MethodImplOptions.AggressiveInlining)]
//         public void OnPointerClick(float x, float y, bool isLong)
//         {
//         }
//
//         public bool IsHovered { get; set; }
//         public bool IsPressed { get; set; }
//         public float TimeFromDown { get; set; }
//     }
// }