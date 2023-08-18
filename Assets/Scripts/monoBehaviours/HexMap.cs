// using System;
// using td.common;
// using UnityEngine;
// using Random = UnityEngine.Random;
//
// namespace td.monoBehaviours
// {
//     public class HexMap : MonoBehaviour
//     {
//         public int minX = -2;
//         public int minY = -2;
//         public int maxX = 2;
//         public int maxY = 2;
//
//         private const float hexRadius = 0.5f;
//         private float hexOffsetX => hexRadius * 1.5f;
//         private float hexOffsetY => hexRadius * Mathf.Sqrt(3);
//
//         private void Start()
//         {
//             var go = GameObject.FindGameObjectWithTag("Enemy");
//
//             for (var y = minY; y <= maxY; y++)
//             {
//                 for (var x = minX; x <= maxX; x++)
//                 {
//                     var render = Random.Range(0, 100);
//                     if (render < 15) continue;
//
//                     var height = 0;//Random.Range(0, 0.5f);
//                     
//                     var position = cellToCoords(new Int2() { x = x, y = y });
//                     position.y += height;
//                     
//                     var instance = Instantiate(go, position, go.transform.rotation);
//                     var renderers = instance.GetComponents<SpriteRenderer>();
//                     foreach (var renderer in renderers)
//                     {
//                         renderer.sortingOrder = y;
//                     }
//                 }
//             }
//         }
//         
//         public Vector2 cellToCoords(Int2 cell) {
//             var x = hexOffsetX * (cell.x + 0.5f);
//             var y = hexOffsetY * (cell.y + (Math.Abs(cell.x) % 2) / 2f + 0.5f);
//             return new Vector2 {x = x, y = y};
//         }
//
//         public Int2 coordsToCell(Vector2 coord) {
//             var x = Mathf.FloorToInt(coord.x / hexOffsetX);
//             var y = Mathf.FloorToInt((coord.y - (x % 2) * hexOffsetY / 2) / hexOffsetY);
//             return new Int2() { x = x, y = y };
//         }
//         
//         
//         // private const float HexRadius = 2f;
//         // private const float HexWidth = HexRadius * 2;
//         // private float HexHeight => Mathf.Sqrt(3);
//         // private float HexHorizontalSpacing => HexWidth * 0.75f;
//         // private float HexVerticalSpacing => HexHeight;
//         
//         // private const float HexWidth = 1.0f * 2;
//         // private const float HexHeight = 0.8660254f * 2;
//         // private const float HexHorizontalSpacing = 0.75f;
//         // private const float HexVerticalSpacing = 0.4330127f;
//         //
//         // float xOffset = 0.4330127f; // расстояние между центрами соседних шестиугольников по оси X
//         // private float yOffset = 0.75f; //
//         //
//         //
//         // private void Start ()
//         // {
//         //     var go = GameObject.FindGameObjectWithTag("Enemy");
//         //
//         //     for (var y = minY; y <= maxY; y++)
//         //     {
//         //         for (var x = minX; x <= maxX; x++)
//         //         {
//         //             // var coord = new Vector2(
//         //             //     x * (HexWidth * HexVerticalSpacing),
//         //             //     (x % 2 ==  1 ? (y - 1) : y) * (HexHeight * HexHorizontalSpacing)
//         //             // );
//         //
//         //             var coord = new Vector2(
//         //                 // x * 2 * xOffset,
//         //                 // y * 2 * yOffset
//         //                 x * 2f * 0.74f,
//         //                 y * 2f * 0.862f
//         //             );
//         //
//         //             if (y % 2 == 1)
//         //             {
//         //                 // coord.x += 1.5f;
//         //             }
//         //
//         //             // if ((x % 2 == 1/* && y % 2 == 0*/)/* || (x % 2 == 0 && y % 2 == 1)*/)
//         //             if (Mathf.Abs(x) % 2 == 1)
//         //             {
//         //                 coord.y -= 0.85f;
//         //             }
//         //             
//         //
//         //             Instantiate(go, coord, go.transform.rotation);
//         //         }
//         //     }
//         // }
//     }
// }