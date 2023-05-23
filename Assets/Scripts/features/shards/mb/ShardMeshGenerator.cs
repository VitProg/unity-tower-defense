using System;
using JetBrains.Annotations;
using NaughtyAttributes;
using td.features.shards.config;
using UnityEngine;

namespace td.features.shards.mb
{
    [DisallowMultipleComponent]
    [ExecuteAlways]
    public class ShardMeshGenerator: MonoBehaviour
    {
        [OnValueChanged("Refresh")] public MeshFilter meshFilter;
        [OnValueChanged("Refresh")] public UIMeshRenderer uiMeshRenderer;
        
        [OnValueChanged("Refresh")][MinValue(3)][MaxValue(512)] public int numVertices = 16; // Количество вершин по умолчанию
        [OnValueChanged("Refresh")] public float outerRadius = 1f;
        [OnValueChanged("Refresh")] public float innerRadius = .25f;
        [OnValueChanged("Refresh")] public float shading = .1f;
        [OnValueChanged("Refresh")] public float padding = 0f;

        [OnValueChanged("Refresh")] public byte[] colorWeights = new byte[8];
        [OnValueChanged("Refresh")][Required] public ShardsConfig shardsConfig;

        public CanvasRenderer canvasRenderer;
        public MeshRenderer meshRenderer;

        private const float PI2 = Mathf.PI * 2f;

        private Segment[] segments = new Segment[8];
        private uint segmentsCount = 0;
        private Vector3[] vertices;
        private int[] triangles;
        private Vector3[] normals;
        [SerializeField] private Color[] colors;

        private void Start()
        {
            Refresh();
        }

        private void CalculateColorSegments()
        {
            var all = 0;
            foreach (var colorWeight in colorWeights)
            {
                all += colorWeight;
            }

            segmentsCount = 0;
            for (var i = 0; i < colorWeights.Length; i++)
            {
                var w = (float)colorWeights[i] / all;
                if (w > 0.01f)
                {
                    segments[segmentsCount].weight = w;
                    segments[segmentsCount].color = shardsConfig[i];
                    segmentsCount++;
                }
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

        [Button("Refresh Mesh", EButtonEnableMode.Editor)]
        public void Refresh()
        {
            CalculateColorSegments();
            
            var nv = Math.Clamp(numVertices, 3, 512);

            // Создание массивов вершин, треугольников и нормалей
            vertices = new Vector3[nv * 4];
            triangles = new int[nv * 18];
            normals = new Vector3[nv * 4];
            colors = new Color[nv * 4];
            
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

                var cos = Mathf.Cos(angle);
                var sin = Mathf.Sin(angle);

                // outer
                var x = cos * oRadius;
                var y = sin * oRadius;
                var c = segment.color;
                c.a = 0f;
                vertices[i] = new Vector3(x, y, 0f);
                normals[i] = normal; // нормали указывают в сторону камеры
                colors[i] = c;
                
                // outer - shading
                x = cos * (oRadius - shading);
                y = sin * (oRadius - shading);
                c = segment.color;
                vertices[nv + i] = new Vector3(x, y, 0f);
                normals[nv + i] = normal;
                colors[nv + i] = c;
                
                // inner + shading
                x = cos * (iRadius + shading);
                y = sin * (iRadius + shading);
                c = segment.color;
                vertices[2 * nv + i] = new Vector3(x, y, 0f);
                normals[2 * nv + i] = normal;
                colors[2 * nv + i] = c;

                // inner
                x = cos * iRadius;
                y = sin * iRadius;
                c = segment.color;
                c.a = 0f;
                vertices[3 * nv + i] = new Vector3(x, y, 0f);
                normals[3 * nv + i] = normal;
                colors[3 * nv + i] = c;
            }
            
            // Создание треугольников для триангуляции
            for (var i = 0; i < nv; i++)
            {
                var ti = i * 18;
                
                var outer0 = i;
                var outer = i % nv + nv;
                var inner = i % nv + (nv * 2);
                var inner0 = i % nv + (nv * 3);

                var outer0N = (outer0 + 1) % nv;
                var outerN = (outer + 1) % nv + nv;
                var innerN = (inner + 1) % nv + (nv * 2);
                var inner0N = (inner0 + 1) % nv + (nv * 3);

                if (i % 2 == 0)
                {
                    triangles[ti++] = outer0;
                    triangles[ti++] = outerN;
                    triangles[ti++] = outer0N;
                
                    triangles[ti++] = outer0;
                    triangles[ti++] = outer;
                    triangles[ti++] = outerN;

                    triangles[ti++] = outer;
                    triangles[ti++] = inner;
                    triangles[ti++] = outerN;

                    triangles[ti++] = outerN;
                    triangles[ti++] = inner;
                    triangles[ti++] = innerN;

                    triangles[ti++] = inner;
                    triangles[ti++] = inner0N;
                    triangles[ti++] = innerN;

                    triangles[ti++] = inner;
                    triangles[ti++] = inner0;
                    triangles[ti++] = inner0N;
                }
                else
                {
                    triangles[ti++] = outer0;
                    triangles[ti++] = outer;
                    triangles[ti++] = outer0N;

                    triangles[ti++] = outer0N;
                    triangles[ti++] = outer;
                    triangles[ti++] = outerN;                   

                    triangles[ti++] = outer;
                    triangles[ti++] = innerN;
                    triangles[ti++] = outerN;
                    
                    triangles[ti++] = outer;
                    triangles[ti++] = inner;
                    triangles[ti++] = innerN;

                    triangles[ti++] = inner;
                    triangles[ti++] = inner0;
                    triangles[ti++] = innerN;

                    triangles[ti++] = innerN;
                    triangles[ti++] = inner0;
                    triangles[ti++] = inner0N;
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
    }

    internal struct Segment
    {
        public float angleBegin;
        public float angleEnd;
        public Color color;
        public float weight;
    } 
}