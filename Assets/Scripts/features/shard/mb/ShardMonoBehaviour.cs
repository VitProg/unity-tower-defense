using System;
using JetBrains.Annotations;
using Leopotam.Types;
using NaughtyAttributes;
using td.features._common;
using td.features.shard.components;
using td.features.shard.data;
using td.monoBehaviours;
using td.utils.di;
using UnityEngine;

namespace td.features.shard.mb
{
    [DisallowMultipleComponent]
    // [ExecuteAlways]
    public class ShardMonoBehaviour: MonoBehaviour
    {
        [OnValueChanged("Refresh")] public Shard shardData;
        
        [OnValueChanged("Refresh")] public MeshFilter meshFilter;
        [OnValueChanged("Refresh")] public UIMeshRenderer uiMeshRenderer;
        
        [OnValueChanged("Refresh")][MinValue(8)][MaxValue(64)] public int numVertices = 16; // Количество вершин по умолчанию
        [OnValueChanged("Refresh")] public float outerRadius = 1f;
        [OnValueChanged("Refresh")] public float innerRadius = .25f;
        
        [Required][OnValueChanged("Refresh")] public ShardLevelIndicatorMB levelIndicator;
        [Required][OnValueChanged("Refresh")] public ShardHoverMB hover;
        [Required][OnValueChanged("Refresh")] public ShardDenyMB deny;

        public CanvasRenderer canvasRenderer;

        private const float PI2 = MathFast.Pi * 2f;

        [SerializeField] private readonly Segment[] segments = new Segment[8];
        [SerializeField] private uint segmentsCount = 0;
        [SerializeField] private Vector3[] vertices;
        [SerializeField] private int[] triangles;
        [SerializeField] private Vector3[] normals;
        [SerializeField] private Color[] colors;

        public Color[] Colors => colors;

        public bool IsHovered => hover.gameObject.activeSelf;
        
        private void Start()
        {
            Refresh();
            shardData._id_ = shardData._id_ > 0 ? shardData._id_ : CommonUtils.ID("shard-mb");
        }

        private void CalculateColorSegments()
        {
            var all = shardData.Quantity;

            segmentsCount = 0;

            var shardsConfigSO = ServiceContainer.Get<Shards_Config_SO>();

            for (var i = 0; i < 8; i++)
            {
                var w = (float)shardData[i] / all;
                if (!(w > 0.01f)) continue;
                segments[segmentsCount].weight = w;
                segments[segmentsCount].color = shardsConfigSO[i];
                segmentsCount++;
            }

            if (segmentsCount == 1)
            {
                segments[0].angleBegin = 0f;
                segments[0].angleEnd = PI2;
            }
            else
            {
                var wAcc = 0f;
                for (var index = 0; index < segmentsCount; index++)
                {   
                    segments[index].angleBegin = wAcc * PI2;
                    wAcc += segments[index].weight;
                    segments[index].angleEnd = wAcc * PI2;
                }
            }
        }

        [CanBeNull]
        private ref Segment GetSegment(float angle)
        {
            for (var i = 0; i < segmentsCount; i++)
            {
                if (angle >= segments[i].angleBegin && angle <= segments[i].angleEnd)
                {
                    return ref segments[i];
                }
            }
            return ref segments[0];
        }

        [Button("Refresh")]
        public void Refresh()
        {
            if (levelIndicator == null)
            {
                levelIndicator ??= transform.GetComponent<ShardLevelIndicatorMB>();
                levelIndicator ??= transform.parent.GetComponentInChildren<ShardLevelIndicatorMB>();
                if (levelIndicator == null)
                {
                    for (var childIndex = transform.parent.childCount - 1; childIndex >= 0; childIndex--)
                    {
                        var child = transform.parent.GetChild(childIndex);
                        levelIndicator ??= child.GetComponent<ShardLevelIndicatorMB>();
                        if (levelIndicator != null) break;
                    }
                }
            }
            
            CalculateColorSegments();

            var nv = Math.Clamp(numVertices, 8, 64);
            
            // Создание массивов вершин, треугольников и нормалей
            vertices = new Vector3[nv * 2];
            triangles = new int[nv * 6];
            normals = new Vector3[nv * 2];
            colors = new Color[nv * 2];
            
            var normal = -Vector3.forward;

            var oRadius = outerRadius;
            var iRadius = innerRadius;

            // Центр круга
            vertices[0] = Vector3.zero;
            normals[0] = normal;

            // Вычисление позиций вершин на окружности
            var angleStep = PI2 / nv;
            for (var i = 0; i < nv; i++)
            {
                var angle = angleStep * i;
                ref var segment = ref GetSegment(angle);

                var cos = MathFast.Cos(angle);
                var sin = MathFast.Sin(angle);

                // outer
                var x = cos * oRadius;
                var y = sin * oRadius;
                var c = segment.color;
                vertices[i] = new Vector3(x, y, 0f);
                normals[i] = normal; // нормали указывают в сторону камеры
                colors[i] = c;
                
                // inner
                x = cos * iRadius;
                y = sin * iRadius;
                c = segment.color;
                vertices[nv + i] = new Vector3(x, y, 0f);
                normals[nv + i] = normal;
                colors[nv + i] = c;
            }
            
            // Создание треугольников для триангуляции
            for (var i = 0; i < nv; i++)
            {
                var ti = i * 6;

                var o2 = (i + 1) % nv;
                var i1 = i + nv;
                var i2 = (i + 1) % nv + nv;

                if (i % 2 == 0)
                {
                    triangles[ti++] = i;
                    triangles[ti++] = i1;
                    triangles[ti++] = i2;
                    
                    triangles[ti++] = i;
                    triangles[ti++] = o2;
                    triangles[ti  ] = i2;
                }
                else
                {
                    triangles[ti++] = o2;
                    triangles[ti++] = i1;
                    triangles[ti++] = i2;
                    
                    triangles[ti++] = o2;
                    triangles[ti++] = i;
                    triangles[ti  ] = i1;
                }
            }
            
            ApplyMeshGeometry();
        }
        
        private void ApplyMeshGeometry()
        {
            var mesh = new Mesh
            {
                vertices = vertices,
                triangles = triangles,
                normals = normals,
                colors = colors,
            };

            mesh.RecalculateNormals();
            
            if (uiMeshRenderer)
            {
                uiMeshRenderer.Mesh = mesh;
            }
            else
            {
                meshFilter.mesh = mesh;
            }
        }
        
        void OnEnable()
        {
            // Debug.Log("ShardMeshGenerator : OnEnable");
            // Refresh();
            if (canvasRenderer)
            {
                canvasRenderer.cull = false;
            }
        }

        void OnDisable()
        {
            if (canvasRenderer)
            {
                canvasRenderer.Clear();
                canvasRenderer.cull = true;
            }
        }

        public void SetShard(Shard shard)
        {
            ServiceContainer.Get<Shard_MB_Service>()?.Add(this);
            shardData = shard;
            // ShardUtils.Copy(ref shardData, ref shard);
        }

        public void SetShard(ref Shard shard)
        {
            ServiceContainer.Get<Shard_MB_Service>()?.Add(this);
            shardData = shard;
            // ShardUtils.Copy(ref shardData, ref shard);
        }

        private void OnDestroy()
        {
            ServiceContainer.Get<Shard_MB_Service>()?.Remove(this);
        }

        public void SetRotation(float r)
        {
            transform.rotation = Quaternion.AngleAxis(r, Vector3.forward);
        }
    }

    internal struct Segment
    {
        public float angleBegin;
        public float angleEnd;
        public Color color;
        public float weight;
    } 
}