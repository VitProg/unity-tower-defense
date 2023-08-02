using Leopotam.EcsLite;
using NaughtyAttributes;
using td.features.inputEvents;
using td.features.level;
using td.features.towerRadius.bus;
using td.monoBehaviours;
using td.utils.di;
using Unity.Mathematics;
using UnityEngine;

namespace td.features.tower.mb
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(EcsEntity))]
    [SelectionBase]
    public class TowerMonoBehaviour : MonoInjectable, IInputEventsHandler
    {
        [Required] public EcsEntity ecsEntity;
        [Required] public GameObject barrel;
        [Required] public SpriteRenderer sprite;
        [Required] public LineRenderer radiusRenderer;
        
        public float2 size = new float2(1f, 1f);

        private readonly EcsInject<IEventBus> events;
        private readonly EcsInject<LevelMap> levelMap;
        
#if UNITY_EDITOR
        void OnDrawGizmosSelected()
        {
            // Display the explosion radius when selected
            Gizmos.color = new Color(1, 0, 0, 0.75F);

            var radius = size.x;
            var yScale = size.y / size.x;

            const byte triangelesCount = 32;
            const float fov = 360f;

            var position = transform.position;
            
            var vertices = new Vector3[triangelesCount + 1 + 1];
            var circleVerticesv = new Vector3[triangelesCount];
            
            var origin = Vector2.zero;
            var angle = 0f;
            var angleIncrease = fov / triangelesCount;

            var vertexIndex = 1;
            var circleIndex = 0;
            for (var i = 0; i <= triangelesCount; i++)
            {
                var angleRad = angle * (Mathf.PI / 180f);
                var vectorFromAngle = new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad) * yScale);
                Vector3 vertex = origin + vectorFromAngle * radius;
                vertex += position;
                vertices[vertexIndex] = vertex;
                if (i > 0 && i <= circleVerticesv.Length)
                {
                    circleVerticesv[circleIndex] = vertices[vertexIndex];
                    circleIndex++;
                }

                vertexIndex++;
                angle -= angleIncrease;
            }
            
            Gizmos.DrawLineStrip(circleVerticesv, true);
        }
#endif

        public void OnPointerEnter(float x, float y)
        {
            // Debug.Log($"OnPointerEnter [{x}, {y}]");
            sprite.color = new Color(1.0f, 0.9f, 0.9f, 1.0f);
            if (ecsEntity.packedEntity.HasValue)
            {
                //todo add radius preview if shard in hand
                events.Value.Global.Add<Command_Tower_ShowRadius>().towerEntity = ecsEntity.packedEntity.Value;
            }
        }

        public void OnPointerLeave(float x, float y)
        {
            // Debug.Log($"OnPointerLeave [{x}, {y}]");
            // todo hide radius
            sprite.color = Color.white;
            if (ecsEntity.packedEntity.HasValue)
            {
                //todo add radius preview if shard in hand
                events.Value.Global.Add<Command_Tower_HideRadius>().towerEntity = ecsEntity.packedEntity.Value;
            }
        }

        public void OnPointerDown(float x, float y)
        {
            // Debug.Log($"OnPointerDown [{x}, {y}]");
            if (ecsEntity.packedEntity.HasValue)
            {
                events.Value.Global.Add<Command_Tower_ShowRadius>().towerEntity = ecsEntity.packedEntity.Value;
            }
        }

        public void OnPointerUp(float x, float y, bool inRadius)
        {
            // Debug.Log($"OnPointerUp [{x}, {y}, {inRadius}]");
            if (ecsEntity.packedEntity.HasValue)
            {
                // Debug.Log("try to show radius");
                events.Value.Global.AddOnNextFrame(new Command_Tower_ShowRadius()
                {
                    towerEntity = ecsEntity.packedEntity.Value,
                });
            }
        }

        public void OnPointerClick(float x, float y)
        {
            // Debug.Log($"OnPointerClick [{x}, {y}]");
        }

        public bool IsHovered { get; set; } = false;
        public bool IsPressed { get; set; } = false;
    }
}