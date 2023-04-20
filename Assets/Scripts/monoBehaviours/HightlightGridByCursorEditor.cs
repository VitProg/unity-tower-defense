// using UnityEditor;
// using UnityEngine;
//
// namespace td.monoBehaviours
// {
// #if UNITY_EDITOR
//     [CustomEditor(typeof(HightlightGridByCursor))]
//     public class HightlightGridByCursorEditor : Editor
//     {
//         public override void OnInspectorGUI()
//         {
//             base.OnInspectorGUI();
//             
//             GUILayout.Space(15);
//
//             if (GUILayout.Button("Update Shader Parameters"))
//             {
//                 ((HightlightGridByCursor)target).UpdateShaderParameters();
//             }
//             
//         }
//     }
// #endif
// }