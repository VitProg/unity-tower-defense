using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Leopotam.EcsLite;
using Leopotam.EcsLite.UnityEditor;
using td.common;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace td.utils
{
#if UNITY_EDITOR
    public static class EditorUtils
    {
        private static uint _lastID;

        private static readonly Dictionary<string, bool> _isShoweds = new();

        public static void RenderAllPropertiesOfObject(
            ref string id,
            object obj,
            [CanBeNull] string title = null,
            uint deep = 0,
            BindingFlags bindingAttr = BindingFlags.Public | BindingFlags.Instance
        ) {
            if (obj == null || deep > 3) return;

            if (string.IsNullOrWhiteSpace(id))
            {
                _lastID++;
                id = _lastID.ToString();
            }

            var type = obj.GetType();
            var typeName = type.Name;
            var fields = type.GetFields(bindingAttr);
            var properties = type.GetProperties(bindingAttr);

            var key = $"{id}#{typeName}";

            if (title != null)
            {
                EditorGUILayout.Space();
                EditorGUILayout.Separator();
                EditorGUILayout.LabelField(title == "*" ? typeName : title, EditorStyles.boldLabel);
            }

            if (properties.Length > 0)
            {
                // EditorGUILayout.LabelField("PROPERTIES:", EditorStyles.miniBoldLabel);

                // Iterate over the fields and display their values in the inspector
                foreach (var property in properties)
                {
                    try
                    {
                        RenderProperty(key, property.PropertyType, property.Name, property.GetValue(obj), false, deep);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                }
            }

            if (fields.Length > 0)
            {
                // EditorGUILayout.LabelField("PROPERTIES:", EditorStyles.miniBoldLabel);

                // Iterate over the fields and display their values in the inspector
                foreach (var field in fields)
                {
                    RenderProperty(key, field.FieldType, field.Name, field.GetValue(obj), false, deep);
                }
            }
        }

        private static void RenderProperty(
            string id,
            Type type,
            [CanBeNull] string name,
            object value,
            bool onlyValue = false, 
            uint deep = 0
        )
        {
            var style = new GUIStyle()
            {
                margin =
                {
                    left = (int)deep * 12
                    
                },
                border = new RectOffset((int)deep, 0, 0, 0),
            };
            
            var foldStyle = EditorStyles.foldoutHeader;
            foldStyle.normal.background = Texture2D.linearGrayTexture;
            
            if (!onlyValue) EditorGUILayout.BeginHorizontal(style);

            var key = $"{id}.{type.Name}:{name}";

            if (type == typeof(string))
            {
                if (!onlyValue && name != null) EditorGUILayout.PrefixLabel(name);
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.TextField((string)value);
                EditorGUI.EndDisabledGroup();
            }
            if (type == typeof(EcsPackedEntity))
            {
                if (!onlyValue && name != null) EditorGUILayout.PrefixLabel(name);
                EditorGUI.BeginDisabledGroup(true);

                // try
                // {
                //     var packed = (EcsPackedEntity)value;
                //     var view = Object
                //         .FindObjectsOfType<EcsEntityDebugView>(true)
                //         .First(view =>
                //             packed.Unpack(view.World, out var entity) && view.name.StartsWith($"{entity:X8}:"));
                //
                //     if (view == null)
                //     {
                //         EditorGUILayout.TextField($"Entity#{value}");
                //     }
                //     else
                //     {
                //         EditorGUILayout.ObjectField(view, view.GetType(), true);
                //     }
                // }
                // catch
                // {
                    EditorGUILayout.TextField($"Entity#{value}");
                // }

                EditorGUI.EndDisabledGroup();
            }
            else if (type == typeof(bool))
            {
                if (!onlyValue && name != null) EditorGUILayout.PrefixLabel(name);
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.Toggle((bool)value);
                EditorGUI.EndDisabledGroup();
            }
            else if (type == typeof(int) || type == typeof(uint))
            {
                if (!onlyValue && name != null) EditorGUILayout.PrefixLabel(name);
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.IntField(type == typeof(uint) ? ((int)(uint)value) : (int)value);
                EditorGUI.EndDisabledGroup();
            }
            else if (type == typeof(float))
            {
                if (!onlyValue && name != null) EditorGUILayout.PrefixLabel(name);
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.FloatField((float)value);
                EditorGUI.EndDisabledGroup();
            }
            else if (type == typeof(Vector2))
            {
                if (!onlyValue && name != null) EditorGUILayout.PrefixLabel(name);
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.Vector2Field("", (Vector2)value);
                EditorGUI.EndDisabledGroup();
            }
            else if (type == typeof(Int2))
            {
                if (!onlyValue && name != null) EditorGUILayout.PrefixLabel(name);
                var f = (Int2)value;
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.Vector2IntField("", new Vector2Int(f.x, f.y));
                EditorGUI.EndDisabledGroup();
            }
            else if (type == typeof(Quaternion))
            {
                if (!onlyValue && name != null) EditorGUILayout.PrefixLabel(name);
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.Vector3Field("", ((Quaternion)value).eulerAngles);
                EditorGUI.EndDisabledGroup();
            }
            else if (type == typeof(Object) ||
                     type == typeof(GameObject) ||
                     type == typeof(ScriptableObject))
            {
                if (!onlyValue && name != null) EditorGUILayout.PrefixLabel(name);
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.ObjectField((Object)value, value.GetType(), true);
                EditorGUI.EndDisabledGroup();
            }
            else
            {
                if (value == null)
                {
                    EditorGUILayout.PrefixLabel(name);
                    EditorGUILayout.LabelField("<null>");
                }
                else if (type.IsArray)
                {
                    EditorGUILayout.BeginVertical();

                    var isShowed = false;
                    if (_isShoweds.TryGetValue(key, out var showed)) isShowed = showed;

                    isShowed = EditorGUILayout.Foldout(isShowed, $"{name} [{((Array)value).Length}]", foldStyle);

                    if (isShowed)
                    {
                        foreach (var item in (Array)value)
                        {
                            RenderProperty($"{key}[]", item.GetType(), null, item, true, deep);
                        }
                    }

                    _isShoweds.Remove(key);
                    _isShoweds.Add(key, isShowed);

                    EditorGUILayout.EndVertical();
                }
                else if ((type.IsClass || type.IsInterface || type.IsAnsiClass))
                {
                    if (onlyValue)
                    {
                        RenderAllPropertiesOfObject(ref key, value, "*", deep + 1);
                    }
                    else
                    {
                        // EditorGUILayout.EndHorizontal();
                        EditorGUILayout.BeginVertical();

                        var subKey = key + "__";
                        
                        var isShowed = false;
                        if (_isShoweds.TryGetValue(subKey, out var showed)) isShowed = showed;

                        isShowed = EditorGUILayout.Foldout(isShowed, name, foldStyle);
                        
                        if (isShowed)
                        {
                            RenderAllPropertiesOfObject(ref key, value, null, deep + 1);
                        }

                        _isShoweds.Remove(subKey);
                        _isShoweds.Add(subKey, isShowed);

                        
                        EditorGUILayout.EndVertical();
                        // EditorGUILayout.BeginHorizontal();
                    }
                }
                else
                {
                    EditorGUILayout.PrefixLabel(name);
                    EditorGUILayout.LabelField(value.ToString());
                }
            }


            if (!onlyValue) EditorGUILayout.EndHorizontal();
        }

        public static GUIStyle GetTExtAreaStyles()
        {
            var style = new GUIStyle(EditorStyles.textArea)
            {
                font = EditorStyles.label.font,
                fontSize = 9,
                wordWrap = false,
                // normal =
                // {
                // textColor = new Color(0.67f, 0.67f, 0.67f, 1.0f) // #aaa in RGB
                // }
                stretchHeight = true
            };

            return style;
        }
        
        public static void HorizontalLine ( Color color ) {
            var horizontalLine = new GUIStyle
            {
                normal =
                {
                    background = EditorGUIUtility.whiteTexture
                },
                margin = new RectOffset( 0, 0, 4, 4 ),
                fixedHeight = 1
            };

            var c = GUI.color;
            GUI.color = color;
            GUILayout.Box( GUIContent.none, horizontalLine );
            GUI.color = c;
        }
    }
#endif
}