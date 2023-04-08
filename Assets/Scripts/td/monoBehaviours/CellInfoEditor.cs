using System.Reflection;
using td.common;
using td.utils;
using UnityEditor;
using UnityEngine;

namespace td.monoBehaviours
{
#if UNITY_EDITOR
    [CustomEditor(typeof(CellInfo))]
    public class CellInfoEditor : Editor
    {
        private string id;
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUI.BeginChangeCheck();
            
            EditorUtils.RenderAllPropertiesOfObject(ref id, ((CellInfo)target).CellData, "*");

            // EditorGUI.EndChangeCheck();
        }
    }
#endif
}
