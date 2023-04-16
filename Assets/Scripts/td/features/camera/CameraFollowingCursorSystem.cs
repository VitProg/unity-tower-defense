using System;
using Cinemachine;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.components;
using td.utils.ecs;
using UnityEngine;

namespace td.features.camera
{
    public class CameraFollowingCursorSystem: IEcsRunSystem
    {
        [EcsWorld] private EcsWorld world;
        
        private readonly EcsFilterInject<Inc<CursorFollowing, Ref<Transform>>> entities = default;

        // TODO ограничить перемещение за пределы экраны или резкое перемещение
        public void Run(IEcsSystems systems)
        {            
            Debug.Assert(Camera.main != null, "Camera.main != null");
            
            var cursorPosition = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var zoomDelta = Input.mouseScrollDelta;

            foreach (var entity in entities.Value)
            {
                ref var cursorFollowing = ref entities.Pools.Inc1.Get(entity);
                ref var refTransform = ref entities.Pools.Inc2.Get(entity);
                var transform = refTransform.reference;

                var currentPosition = transform.position;

                var movementVector = (cursorPosition - (Vector2)currentPosition);

                var distance = movementVector.magnitude;
                
                movementVector.Normalize();

                var speed = Mathf.Max(.1f, distance / 10f);

                var vector = movementVector * speed;

                // Camera.main.orthographicSize += -zoomDelta.y;
                // if (Mathf.Abs(zoomDelta.y) > 0.0001f)
                // {
                    // Debug.Log($"SCROLL: {zoomDelta.y}");
                // }
                if (cursorFollowing.virtualCamera != null)
                {
                    cursorFollowing.virtualCamera.m_Lens.OrthographicSize =
                        Mathf.Max(1f,
                            Mathf.Min(20f, cursorFollowing.virtualCamera.m_Lens.OrthographicSize + zoomDelta.y));
                }

                // transform.position += (Vector3)vector;
                transform.position = cursorPosition;
            }
        }
    }
}