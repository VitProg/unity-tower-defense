// #if UNITY_EDITOR
// using System;
// using System.Collections.Generic;
// using System.Reflection;
// using Leopotam.EcsLite;
// using Leopotam.EcsLite.UnityEditor;
// using td.common;
// using Unity.Mathematics;
// using UnityEditor;
// using UnityEngine;
// using Object = System.Object;
//
// namespace td.monoBehaviours
// {
//     [CustomEditor(typeof(EcsComponentsInfo))]
//     public class EcsComponentsInfoEditor : Editor
//     {
//         private string id;
//
//         private static Dictionary<Type, PropertyInfo[]> cacheProperties = new();
//         private static Dictionary<Type, FieldInfo[]> cacheFields = new();
//         
//         private static readonly Type EcsPackedEntityType = typeof(EcsPackedEntity);
//         private static readonly Type EcsPackedEntityWithWorldType = typeof(EcsPackedEntityWithWorld);
//         private static readonly Type BooleanType = typeof(bool);
//         private static readonly Type IntType = typeof(int);
//         private static readonly Type StringType = typeof(string);
//         private static readonly Type UintType = typeof(uint);
//         private static readonly Type ShortType = typeof(short);
//         private static readonly Type UshortType = typeof(ushort);
//         private static readonly Type ByteType = typeof(byte);
//         private static readonly Type SbyteType = typeof(sbyte);
//         private static readonly Type LongType = typeof(long);
//         private static readonly Type ULongType = typeof(ulong);
//         private static readonly Type FloatType = typeof(float);
//         private static readonly Type DoubleType = typeof(double);
//         private static readonly Type Vector2Type = typeof(Vector2);
//         private static readonly Type Vector3Type = typeof(Vector3);
//         private static readonly Type Vector4Type = typeof(Vector4);
//         private static readonly Type Float2Type = typeof(float2);
//         private static readonly Type Float3Type = typeof(float3);
//         private static readonly Type Float4Type = typeof(float4);
//         private static readonly Type Int2Type = typeof(int2);
//         private static readonly Type Int3Type = typeof(int3);
//         private static readonly Type Int4Type = typeof(int4);
//         private static readonly Type Int2CustomType = typeof(Int2);
//         private static readonly Type QuaternionType = typeof(Quaternion);
//         private static readonly Type UnityObjectType = typeof(Object);
//         private static readonly Type GameObjectType = typeof(GameObject);
//         private static readonly Type MonoBehaviourType = typeof(MonoBehaviour);
//         private static readonly Type ScriptableObjectType = typeof(ScriptableObject);
//         private static readonly Type ColorType = typeof(Color);
//
//         public override void OnInspectorGUI()
//         {
//             base.OnInspectorGUI();
//
//             EditorGUI.BeginChangeCheck();
//             
//             var info = (EcsComponentsInfo)target;
//
//             EditorGUILayout.BeginVertical();
//             if (EditorGUILayout.LinkButton("Refresh"))
//             {
//                 info.UpdateComponents();
//             }
//             EditorGUILayout.Space();
//             EditorGUILayout.Separator();
//             EditorGUILayout.EndVertical();
//             
//             // var count = info.components.Count;
//             // for (var index = 0; index < count; index++)
//             // {
//             // var component = info.components[index];
//             foreach (var (type, component) in info.components)
//             {
//                 if (component == null) continue;
//                 var componentType = component.GetType();
//                 var componentName = EditorExtensions.GetCleanGenericTypeName(componentType);
//                 
//                 if (
//                     !cacheProperties.TryGetValue(componentType, out var properties) ||
//                     !cacheFields.TryGetValue(componentType, out var fields))
//                 {
//                     var bindingAttr = BindingFlags.Public | BindingFlags.Instance;
//                     
//                     properties = componentType.GetProperties(bindingAttr);
//                     fields = componentType.GetFields(bindingAttr);
//
//                     cacheProperties.Remove(componentType);
//                     cacheFields.Remove(componentType);
//                     
//                     cacheProperties.Add(componentType, properties);
//                     cacheFields.Add(componentType, fields);
//                 }
//                 
//                 GUILayout.BeginVertical(GUI.skin.box);
//
//                 DrawTitle(componentName);
//                 EditorGUI.indentLevel++;
//
//                 foreach (var property in properties)
//                 {
//                     if (property.GetMethod.GetParameters().Length > 0) continue;
//                     DrawProperty(property.Name, property.PropertyType, property.GetValue(component));
//                 }
//
//                 foreach (var field in fields)
//                 {
//                     DrawProperty(field.Name, field.FieldType, field.GetValue(component));
//                 }
//                 EditorGUI.indentLevel--;
//                 
//                 GUILayout.EndVertical();
//                 EditorGUILayout.Space();
//             }
//             
//             EditorGUI.EndChangeCheck();
//         }
//
//         private void DrawProperty(string name, Type type, object value)
//         {
//             if (type == StringType) EditorGUILayout.TextField(name, (string)value);
//             else if (type == IntType) EditorGUILayout.IntField(name, (int)value);
//             else if (type == UintType) EditorGUILayout.LongField(name, (uint)value);
//             else if (type == ShortType) EditorGUILayout.IntField(name, (short)value);
//             else if (type == UshortType) EditorGUILayout.IntField(name, (ushort)value);
//             else if (type == SbyteType) EditorGUILayout.IntField(name, (sbyte)value);
//             else if (type == ByteType) EditorGUILayout.IntField(name, (byte)value);
//             else if (type == LongType) EditorGUILayout.LongField(name, (long)value);
//             else if (type == ULongType) EditorGUILayout.LongField(name, (long)(ulong)value); //!
//             else if (type == BooleanType) EditorGUILayout.TextField(name, ((bool)value) ? "[True]" : "[False]");
//             else if (type == FloatType) EditorGUILayout.FloatField(name, (float)value);
//             else if (type == DoubleType) EditorGUILayout.DoubleField(name, (double)value);
//             else if (type == Vector2Type) EditorGUILayout.Vector2Field(name, (Vector2)value);
//             else if (type == Vector3Type) EditorGUILayout.Vector3Field(name, (Vector3)value);
//             else if (type == Vector4Type) EditorGUILayout.Vector4Field(name, (Vector4)value);
//             else if (type == Float2Type) EditorGUILayout.Vector2Field(name, (float2)value);
//             else if (type == Float3Type) EditorGUILayout.Vector3Field(name, (float3)value);
//             else if (type == Float4Type) EditorGUILayout.Vector4Field(name, (float4)value);
//             else if (type == Float4Type) EditorGUILayout.Vector4Field(name, (float4)value);
//             else if (type == ColorType) EditorGUILayout.ColorField(name, (Color)value);
//             else if (type == Int2CustomType)
//             {
//                 var v = (Int2)value;
//                 EditorGUILayout.Vector2IntField(name, new Vector2Int(v.x, v.y));
//             }
//             else if (type == Int2Type)
//             {
//                 var v = (int2)value;
//                 EditorGUILayout.Vector2IntField(name, new Vector2Int(v.x, v.y));
//             }
//             else if (type == Int3Type)
//             {
//                 var v = (int3)value;
//                 EditorGUILayout.Vector3IntField(name, new Vector3Int(v.x, v.y, v.z));
//             }
//             else if (type == Int4Type)
//             {
//                 var v = (int4)value;
//                 EditorGUILayout.Vector4Field(name, new Vector4(v.x, v.y, v.z, v.w));
//             }            
//             else if (type == QuaternionType)
//             {
//                 var v = (Quaternion)value;
//                 EditorGUILayout.Vector3Field(name, v.eulerAngles);
//             }
//             else if (type == GameObjectType || type == ScriptableObjectType || type == UnityObjectType || type == MonoBehaviourType || value is MonoBehaviour)
//             {
//                 if (value == null)
//                     EditorGUILayout.LabelField(name, "<null>");
//                 else
//                     EditorGUILayout.ObjectField(name, (UnityEngine.Object)value, value.GetType(), true);
//             }
//             else
//             {
//                 if (type.IsArray && value != null)
//                 {
//                     EditorGUILayout.LabelField(name, EditorExtensions.GetCleanGenericTypeName(type));
//                     EditorGUI.indentLevel++;
//                     var array = (Array)value;
//                     var index = 0;
//                     foreach (var item in array)
//                     {
//                         if (item == null) continue;
//                         DrawProperty(index.ToString(), item.GetType(), item);
//                         index++;
//                     }
//                     EditorGUI.indentLevel--;
//                 }
//                 else
//                 {
//                     EditorGUILayout.LabelField(EditorExtensions.GetCleanGenericTypeName(type), value?.ToString());
//                 }
//             }
//         }
//
//
//         private void DrawTitle(string sTitle, bool sep = false)
//         {
//             if (sep)
//             {
//                 EditorGUILayout.Space();
//                 EditorGUILayout.Separator();
//             }
//
//             EditorGUILayout.LabelField(sTitle, EditorStyles.boldLabel);
//         }
//     }
// }
// #endif