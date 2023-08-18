// using NaughtyAttributes;
// using td.utils;
// using UnityEngine;
// using UnityEngine.UI;
//
// namespace td.monoBehaviours
// {
//     [DisallowMultipleComponent]
//     // [RequireComponent(typeof(LineRenderer))]
//     public class CircleDrawer : MonoBehaviour
//     {
//         [OnValueChanged("Refresh")] public ushort segments = 32;
//         [OnValueChanged("Refresh")] public float radius = 1f;
//         [OnValueChanged("Refresh")] public float lineWidth = 0.1f;
//         [OnValueChanged("Refresh")] public Color color = Color.red;
//         
//         [SerializeField] private LineRenderer lineRenderer;
//         [SerializeField] private Image imageRenderer;
//         private float _lastRadius = -1f;
//
//         void Start()
//         {
//             Refresh();
//         }
//
//         void Update()
//         {
//             if (lineRenderer && (
//                     lineRenderer.positionCount != segments + 1 ||
//                     lineRenderer.startColor != color ||
//                     !FloatUtils.IsEquals(_lastRadius, radius) ||
//                     !FloatUtils.IsEquals(lineRenderer.startWidth, lineWidth)
//                 ))
//             {
//                 _lastRadius = radius;
//                 Refresh();
//             }
//         }
//
//         private void Refresh()
//         {
//             if (lineRenderer)
//             {
//                 DrawInWorld();
//             }
//
//             if (imageRenderer)
//             {
//                 DrawInCanvas();
//             }
//         }
//
//         private void DrawInWorld()
//         {
//             lineRenderer.positionCount = segments + 1;
//             lineRenderer.startColor = lineRenderer.endColor = color;
//             lineRenderer.startWidth = lineRenderer.endWidth = lineWidth;
//
//             var deltaTheta = (2.0f * Mathf.PI) / segments;
//             var theta = 0.0f;
//
//             for (var i = 0; i < segments + 1; i++)
//             {
//                 var x = radius * Mathf.Cos(theta);
//                 var y = radius * Mathf.Sin(theta);
//                 var pos = new Vector3(x, y, 0.0f);
//                 lineRenderer.SetPosition(i, pos);
//                 theta += deltaTheta;
//             }
//         }
//
//         private void DrawInCanvas()
//         {
//             Texture2D texture = new Texture2D(Mathf.FloorToInt(radius * 2f), Mathf.FloorToInt(radius * 2f));
//             Color[] pixels = new Color[texture.width * texture.height];
//
//             for (int i = 0; i < pixels.Length; i++)
//             {
//                 int x = i % texture.width;
//                 int y = i / texture.width;
//                 Vector2 point = new Vector2(x - radius, y - radius);
//                 if (point.magnitude <= radius && point.magnitude >= radius - lineWidth)
//                 {
//                     pixels[i] = color;
//                 }
//                 else
//                 {
//                     pixels[i] = Color.clear;
//                 }
//             }
//
//             texture.SetPixels(pixels);
//             texture.Apply();
//
//             imageRenderer.sprite = Sprite.Create(
//                 texture,
//                 new Rect(0, 0, texture.width, texture.height),
//                 new Vector2(0.5f, 0.5f),
//                 pixelsPerUnit: 1f,
//                 extrude: 0,
//                 SpriteMeshType.FullRect
//             );
//
//             imageRenderer.rectTransform.sizeDelta = new Vector2(radius * 2, radius * 2);
//         }
//     }
// }