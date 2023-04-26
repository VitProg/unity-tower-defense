// using System;
// using Shapes2D;
// using td.features.towers;
// using UnityEngine;
//
// namespace td.monoBehaviours
// {
// #if UNITY_EDITOR
//     [ExecuteAlways] // Код ниже должен исполняться всегда
//     [SelectionBase] // Если вы кликнете на внутреннюю запчасть префаба, то выделится именно этот объект
// #endif
//     public class TowerRadius : MonoBehaviour
//     {
//         private TowerProvider Tower;
//         private float lastRadius = -1f;
//         private Shape shape;
//         private Transform radiusTransform;
//
// #if UNITY_EDITOR
//         private void Start()
//         {
//             try
//             {
//                 Tower = GetComponent<TowerProvider>();
//                 radiusTransform = transform.Find("radius");
//                 shape = radiusTransform.GetComponent<Shape>();
//             }
//             catch
//             {
//                 // ignored
//             }
//         }
//         
//         private void Update()
//         {
//             try
//             {
//                 if (!Tower)
//                 {
//                     Tower = GetComponent<TowerProvider>();
//                 }
//         
//                 if (!radiusTransform)
//                 {
//                     radiusTransform = transform.Find("radius");
//                 }
//         
//                 if (!shape)
//                 {
//                     shape = radiusTransform.GetComponent<Shape>();
//                 }
//             
//                 var radius = Tower.component.radius;
//                 if (Math.Abs(lastRadius - radius) > Constants.ZeroFloat)
//                 {
//                     lastRadius = radius;
//                     radiusTransform.localScale = new Vector3(radius, radius, radius) * 1.3f;
//                 }
//             }
//             catch
//             {
//                 // ignored
//             }
//         }
// #endif   
//     }
// }