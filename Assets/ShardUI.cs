using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

namespace td
{
    [RequireComponent(typeof(CanvasRenderer))]
    // [ExecuteAlways]
    public class ShardUI : MonoBehaviour
    {
        [SerializeField] Material Material;
        [SerializeField] private Mesh mesh;

        [OnValueChanged("UpdateAll")][MaxValue(100)] public uint c1 = 1;
        [OnValueChanged("UpdateAll")][MaxValue(100)] public uint c2 = 1;
        [OnValueChanged("UpdateAll")][MaxValue(100)] public uint c3 = 1;
        [OnValueChanged("UpdateAll")][MaxValue(100)] public uint c4 = 1;
        [OnValueChanged("UpdateAll")][MaxValue(100)] public uint c5 = 1;
        [OnValueChanged("UpdateAll")][MaxValue(100)] public uint c6 = 1;
        [OnValueChanged("UpdateAll")][MaxValue(100)] public uint c7 = 1;
        [OnValueChanged("UpdateAll")][MaxValue(100)] public uint c8 = 1;
        
        CanvasRenderer canvasRenderer;

        Image[] childImage;

        Vector3[] baseVertices;

        RectTransform rect;
        float cachedHeight, cachedWidth;
        private Mesh meshInstance;
        [SerializeField][ReadOnly] private Vector2 v;
        [ShowNativeProperty] private int x => (int)(v.x * 1024);
        [ShowNativeProperty] private int y => (int)(v.y * 1024);
        [ShowNativeProperty] private int i => y * 1024 + x;

        void Start()
        {
            SetupMesh();
        }

        void Update()
        {
            // if rect changed, update
            if (cachedWidth != rect.rect.width || cachedHeight != rect.rect.height)
            {
                canvasRenderer.SetMesh(CreateNewMesh());
                cachedWidth = rect.rect.width;
                cachedHeight = rect.rect.height;
            }

            UpdateUVs();
        }

        private void UpdateAll()
        {
            canvasRenderer.SetMesh(CreateNewMesh());
            cachedWidth = rect.rect.width;
            cachedHeight = rect.rect.height;
        }

        private void UpdateUVs()
        {
            mesh.uv  = GetUVsNew();
            mesh.uv2  = GetUVsNew();
            // mesh.uv2  = GetUVs(c1, c2);
            // mesh.uv3 = GetUVs(c3, c4);
            // mesh.uv4 = GetUVs(c5, c6);
            // mesh.uv5 = GetUVs(c7, c8);
        }

        void SetupMesh()
        {
            // grab the canvasrenderer
            if (canvasRenderer == null)
            {
                canvasRenderer = GetComponent<CanvasRenderer>();
            }

            if (rect == null)
            {
                rect = GetComponent<RectTransform>();
            }

            // set the mesh and material
            canvasRenderer.SetMaterial(Material, null);
            // create new mesh with scale
            meshInstance = CreateNewMesh();
            canvasRenderer.SetMesh(meshInstance);
        }


        void OnEnable()
        {
            SetupMesh();
            canvasRenderer.cull = false;
        }

        void OnDisable()
        {
            canvasRenderer.Clear();
            canvasRenderer.cull = true;
        }

        private Mesh CreateNewMesh()
        {
            // create copy of the mesh
            Mesh newMesh = Instantiate(mesh);
            baseVertices = newMesh.vertices;
            var vertices = new Vector3[baseVertices.Length];
            // base size on mesh bounds
            Vector2 size = new Vector2(newMesh.bounds.extents.x, newMesh.bounds.extents.y);
            Rect r = rect.rect;

            // scale to rect transform size
            float scaleY = (r.height / newMesh.bounds.max.y) * 0.5f;
            float scaleX = (r.width / newMesh.bounds.max.x) * 0.5f;

            // use scale on all vertices
            for (var i = 0; i < vertices.Length; i++)
            {
                var vertex = baseVertices[i];
                vertex.x *= scaleX;
                vertex.y *= scaleY;
                vertex.z *= scaleX;
                
                vertices[i] = vertex;
            }
            
            // set new vertices
            newMesh.vertices = vertices;

            // newMesh.RecalculateNormals();
            // newMesh.RecalculateBounds();
            
            UpdateUVs();

            return newMesh;
        }

        private Vector2[] GetUVsNew()
        {
            const uint size = 1024;
            const uint variants = 5;

            var w1 = Mathf.CeilToInt(c1 / (100f / variants));
            var w2 = Mathf.CeilToInt(c2 / (100f / variants));
            var w3 = Mathf.CeilToInt(c3 / (100f / variants));
            var w4 = Mathf.CeilToInt(c4 / (100f / variants));
            var w5 = Mathf.CeilToInt(c5 / (100f / variants));
            var w6 = Mathf.CeilToInt(c6 / (100f / variants));
            var w7 = Mathf.CeilToInt(c7 / (100f / variants));
            var w8 = Mathf.CeilToInt(c8 / (100f / variants));
            
            long index = (w1 * 32768) +
                         (w2 * 16807) + 
                         (w3 * 7776) +
                         (w4 * 3125) + 
                         (w5 * 1024) +
                         (w6 * 243) +
                         (w7 * 32) +
                         (w8);
            index *= 2;
            
            var y = index % size;
            var x = index / size;

            v = new Vector2((float)x / size, (float)y / size);
            
            var uvs = new Vector2[mesh.vertices.Length];

            for (var i = 0; i < uvs.Length; i++)
            {
                uvs[i] = v;
            }

            return uvs;
        }
        
        private Vector2[] GetUVs(uint color1, uint color2)
        {
            var v = new Vector2(color1 / 100f, color2 / 100f);
            var uvs = new Vector2[mesh.vertices.Length];

            // Debug.Log(v);

            for (var i = 0; i < uvs.Length; i++)
            {
                uvs[i] = v;
            }

            return uvs;
        }

        void OnValidate()
        {
            if (!Application.isPlaying)
            {
                SetupMesh();
            }
        }
    }
}