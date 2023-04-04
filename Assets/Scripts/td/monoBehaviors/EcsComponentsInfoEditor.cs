using System.Reflection;
using Leopotam.EcsLite;
using td.common;
using UnityEditor;
using UnityEngine;

namespace td.monoBehaviors
{
    [CustomEditor(typeof(EcsComponentsInfo))]
    public class EcsComponentsInfoEditor : Editor
    {
#if UNITY_EDITOR
        private GUIStyle style;
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            InitStyles();
            
            EditorGUI.BeginChangeCheck();
            
            var info = (EcsComponentsInfo)target;
            
            /**/
            if (info.World != null && info.PackedEntity.Unpack(info.World, out var entity))
            {
                var components = new object[] { };
                info.World.GetComponents(entity, ref components);


                EditorGUILayout.Space();

                foreach (var component in components)
                {
                    if (component == null) continue;
                    
                    var componentName = component.GetType().Name;
                    var componentType = component.GetType();
                    var fields = componentType.GetFields(BindingFlags.Public | BindingFlags.Instance);

                    EditorGUILayout.Separator();
                    EditorGUILayout.LabelField(componentName, EditorStyles.boldLabel);

                    // Iterate over the fields and display their values in the inspector
                    foreach (var field in fields)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.PrefixLabel(field.Name);

                        RenderValue(field, field.GetValue(component));

                        EditorGUILayout.EndHorizontal();
                    }
                }
            }
        }

        private void RenderValue(FieldInfo field, object fieldValue)
        {
            EditorGUI.BeginDisabledGroup(true);

            if (field.FieldType == typeof(string))
            {
                EditorGUILayout.TextField((string)fieldValue);
            }
            else if (field.FieldType == typeof(bool))
            {
                EditorGUILayout.Toggle((bool)fieldValue);
            }
            else if (field.FieldType == typeof(int))
            {
                EditorGUILayout.IntField((int)fieldValue);
            }
            else if (field.FieldType == typeof(float))
            {
                EditorGUILayout.FloatField((float)fieldValue);
            }
            else if (field.FieldType == typeof(Vector2))
            {
                EditorGUILayout.Vector2Field("", (Vector2)fieldValue);
            }
            else if (field.FieldType == typeof(Int2))
            {
                var f = (Int2)fieldValue;
                EditorGUILayout.Vector2IntField("", new Vector2Int(f.x, f.y));
            }
            else if (field.FieldType == typeof(Quaternion))
            {
                EditorGUILayout.Vector3Field("", ((Quaternion)fieldValue).eulerAngles);
            }
            else if (field.FieldType == typeof(UnityEngine.Object) ||
                     field.FieldType == typeof(UnityEngine.GameObject) ||
                     field.FieldType == typeof(UnityEngine.ScriptableObject))
            {
                EditorGUILayout.ObjectField((UnityEngine.Object)fieldValue, fieldValue.GetType(), true);
            }
            else
            {
                // EditorGUILayout.LabelField($"Unsupported type: {field.FieldType.Name}");
                EditorGUILayout.LabelField(fieldValue.ToString());
            }

            EditorGUI.EndDisabledGroup();
        }

        private void InitStyles()
        {
            if (style == null)
            {
                style = new GUIStyle(EditorStyles.textArea)
                {
                    font = EditorStyles.label.font,
                    fontSize = 9,
                    wordWrap = true
                };
                style.normal.textColor = new Color(0.67f, 0.67f, 0.67f, 1.0f); // #aaa in RGB
            }
        }
#endif
    }
}
