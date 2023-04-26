// using NaughtyAttributes.Editor;
// using System.Reflection;
// using td.common;
// using td.utils;
// using UnityEditor;
// using UnityEngine;
//
// namespace td.monoBehaviours
// {
// #if UNITY_EDITOR
//     [CustomEditor(typeof(Cell))]
//     public class CellEditor : NaughtyInspector
//     {
//         private string id;
//         
//         public override void OnInspectorGUI()
//         {
//             base.OnInspectorGUI();
//
//             EditorGUI.BeginChangeCheck();
//
//             if (((Cell)target).Cell != null)
//             {
//                 EditorUtils.RenderAllPropertiesOfObject(ref id, ((Cell)target).Cell, "*");
//             }
//
//             // EditorGUI.EndChangeCheck();
//         }
//     }
// #endif
// }
