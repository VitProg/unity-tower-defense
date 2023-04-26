// using Leopotam.EcsLite;
// using Leopotam.EcsLite.Di;
// using td.components;
// using td.components.behaviors;
// using td.utils;
// using td.utils.ecs;
// using UnityEngine;
//
// namespace td.systems.init
// {
//     public class EnemiesTestInitSystem : IEcsInitSystem
//     {
//         [InjectPool] private EcsPool<LinearMovementToTarget> moveToTargetPointPool;
//
//         public void Init(IEcsSystems systems)
//         {
//             var world = systems.GetWorld();
//
//             var go = (GameObject)Resources.Load("Prefabs/enemies/Enemy", typeof(GameObject));
//
//             var count =  0;
//
//             var parent = GameObject.FindGameObjectWithTag(Constants.Tags.EnemiesContainer);
//
//             for (var index = 0; index < Constants.Enemy.MaxEnemies; index++)
//             {
//                 CreateEnemy(world, go, parent.transform);
//                 count++;
//             }
//
//             // Debug.Log($"Final number of enemies {count}");
//         }
//
//         private void CreateEnemy(EcsWorld world, GameObject baseGameObject, Transform parent)
//         {
//             var x = Random.Range(-1 * 100, 41 * 100) / 100.0f;
//             var y = Random.Range(1 * 100, 25 * 100) / 100.0f;
//             
//             var gameObject = Object.Instantiate(baseGameObject, parent, true);
//             var entity = world.ConvertToEntity(gameObject);
//             
//             gameObject.transform.position = new Vector2(x, y);
//             
//             // var entity = world.NewEntity();
//             //
//             // Debug.Assert(Camera.main != null, "Camera.main != null");
//             //
//             //
//             // var x = Random.Range(
//             //     -1 * 100, 41 * 100
//             // ) / 100.0f;
//             // var y = Random.Range(
//             //     1 * 100, 25 * 100
//             // ) / 100.0f;
//             //
//             // var gameObject = Object.Instantiate(baseGameObject, parent, true);
//             // gameObject.transform.position = new Vector2(x, y);
//
//             PrepareEnemy(entity, gameObject);
//         }
//
//         private void PrepareEnemy(int entity, GameObject gameObject)
//         {
//             // gameObjectLinkPool.Value.Add(entity).gameObject = gameObject;
//             // transformLinkPool.Value.Add(entity).transform = gameObject.transform;
//             // positionPool.Value.Add(entity).position = gameObject.transform.position;
//             // lookForwardPool.Value.Add(entity);
//
//             // ref var movable = ref moveToTargetPointPool.Value.Add(entity);
//             ref var movable = ref moveToTargetPointPool.Get(entity);
//             movable.speed = Random.Range(Constants.Enemy.MinSpeed, Constants.Enemy.MaxSpeed);
//
//             var scale = Random.Range(Constants.Enemy.MinSize, Constants.Enemy.MaxSize);
//             gameObject.transform.localScale = new Vector3(scale, scale, scale);
//         }
//     }
// }