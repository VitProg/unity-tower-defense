#if UNITY_EDITOR
using Leopotam.EcsProto.QoL;
using UnityEditor;
#endif
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

#if UNITY_EDITOR
namespace td.utils
{
    public static class EditorUtils
    {
        private static readonly Color LineColor = new Color(0.35f, 0.35f, 0.35f, 0.65f);
        
        public static void DrawLine(int height = 2, int marginTop = 4, int marginBottom = 4, Color? color = null)
        {
            if (marginTop > 0) EditorGUILayout.Space(marginTop);
            
            var rect = EditorGUILayout.GetControlRect(false, height );
            rect.height = height;

            EditorGUI.DrawRect(rect, color ?? LineColor);
            
            if (marginBottom > 0) EditorGUILayout.Space(marginBottom);
        }
        
        public static void DrawTitle(string sTitle, bool sep = false)
        {
            if (sep)
            {
                EditorGUILayout.Space();
                EditorGUILayout.Separator();
            }

            EditorGUILayout.LabelField(sTitle, EditorStyles.boldLabel);
        }

        public static void DrawProperty(string sNname, bool value)
        {
            EditorGUILayout.TextField(sNname, value ? "True" : "False");
        }

        public static void DrawProperty(string sNname, float value)
        {
            EditorGUILayout.FloatField(sNname, value);
        }

        public static void DrawProperty(string sNname, ushort value)
        {
            EditorGUILayout.IntField(sNname, value);
        }

        public static void DrawProperty(string sNname, uint value)
        {
            EditorGUILayout.LongField(sNname, value);
        }

        public static void DrawProperty(string sNname, int value)
        {
            EditorGUILayout.IntField(sNname, value);
        }

        public static void DrawProperty(string sNname, string value)
        {
            EditorGUILayout.LabelField(sNname, value);
        }
        
        public static void DrawProperty(string sNname, Vector2 value)
        {
            EditorGUILayout.Vector2Field(sNname, value);
        }              
        
        public static void DrawProperty(string sNname, Color value)
        {
            EditorGUILayout.ColorField(sNname, value);
        }       
        
        public static void DrawEntity(string sNname, ProtoPackedEntityWithWorld packedEntity)
        {
            EditorGUILayout.TextField(sNname, $"{packedEntity.Id}:${packedEntity.Gen}");
        }
        
        public static void DrawInt2(string sNname, int2 value)
        {
            EditorGUILayout.Vector2IntField(sNname, new Vector2Int(value.x, value.y));
        }

        public static Dictionary<string, bool> foldouts = new();
        public static GUIStyle boldStyle;

        public static bool FoldoutBegin(string key, string title, string titleHidden = null)
        {
            var value = foldouts.TryGetValue(key, out var f) && f;
            foldouts[key] = value;
            foldouts[key] = EditorGUILayout.Foldout(value, value ? title : (string.IsNullOrEmpty(titleHidden) ? title : titleHidden), true);
            if (value) EditorGUI.indentLevel++;
            return value;
        }

        public static void FoldoutEnd()
        {
            EditorGUI.indentLevel--;
            // EditorGUILayout.EndFoldoutHeaderGroup();
        }

        public static bool PrimaryFoldoutBegin(string key, string title, string titleHidden = null)
        {
            var value = foldouts.TryGetValue(key, out var f) && f;
            foldouts[key] = value;
            
            // EditorGUILayout.BeginVertical(Styles.BorderStyle);
            foldouts[key] = EditorGUILayout.BeginFoldoutHeaderGroup(value, value ? title : (string.IsNullOrEmpty(titleHidden) ? title : titleHidden));//, Styles.PrimaryFoldoutHeaderStyle);
            if (value)
            {
                EditorGUILayout.BeginVertical(Styles.PrimiryFoldoutContentStyle);
                EditorGUILayout.Space(-2);
            }
            return value;
        }

        // public static void PrimaryFoldoutEnd()
        // {
        //     // EditorGUILayout.EndVertical(); // content
        //     EditorGUILayout.EndFoldoutHeaderGroup();
        //     // EditorGUILayout.EndVertical(); // border
        // }

        public static void LabelField(string key, string value, GUIStyle style)
        {
            EditorGUILayout.BeginHorizontal();
            try {
                EditorGUILayout.LabelField(key, style);
                EditorGUILayout.LabelField(value, style);
            } finally {
                EditorGUILayout.EndHorizontal();
            }
        }
        
        public static class Styles
        {
            public static Color HeaderColor;
            public static Color ContentColor;
            public static Color BorderColor;
            public static GUIStyle PrimaryFoldoutHeaderStyle;
            public static GUIStyle PrimiryFoldoutContentStyle;
            public static GUIStyle BorderStyle;

            static Styles()
            {
                Init();
            }

            public static void Init()
            {
                // Define colors for the foldout
                HeaderColor = true ? new Color(0.25f, 0.25f, 0.25f) : new Color(0.76f, 0.76f, 0.76f);
                ContentColor = true ? new Color(0.25f, 0.25f, 0.25f) : new Color(0.8f, 0.8f, 0.8f);
                BorderColor = true ? new Color(0.1f, 0.1f, 0.1f) : new Color(0.86f, 0.86f, 0.86f);

                PrimaryFoldoutHeaderStyle = new GUIStyle(EditorStyles.foldout);
                // PrimaryFoldoutHeaderStyle.normal.textColor = HeaderColor;
                // PrimaryFoldoutHeaderStyle.onNormal.textColor = HeaderColor;
                // PrimaryFoldoutHeaderStyle.active.textColor = HeaderColor;
                // PrimaryFoldoutHeaderStyle.onActive.textColor = HeaderColor;
                // PrimaryFoldoutHeaderStyle.focused.textColor = HeaderColor;
                // PrimaryFoldoutHeaderStyle.onFocused.textColor = HeaderColor;
                PrimaryFoldoutHeaderStyle.normal.background = MakeTex(1, 1, HeaderColor);

                PrimiryFoldoutContentStyle = new GUIStyle(GUI.skin.box);
                PrimiryFoldoutContentStyle.normal.background = MakeTex(1, 1, ContentColor);
                PrimiryFoldoutContentStyle.padding.left = 10;
                PrimiryFoldoutContentStyle.padding.right = 10;
                PrimiryFoldoutContentStyle.padding.top = 5;
                PrimiryFoldoutContentStyle.padding.bottom = 5;
                PrimiryFoldoutContentStyle.margin.bottom = 15;
                // PrimiryFoldoutContentStyle.normal.background = EditorGUIUtility.whiteTexture;
                // PrimiryFoldoutContentStyle.normal.textColor = ContentColor;
                // PrimiryFoldoutContentStyle.border = new RectOffset(1, 1, 1, 1);
                
                BorderStyle = new GUIStyle(GUI.skin.box);
                BorderStyle.normal.background = MakeTex(1, 1, BorderColor);
            }

            public static Texture2D MakeTex(int width, int height, Color color)
            {
                Color[] pix = new Color[width * height];
                for (int i = 0; i < pix.Length; i++)
                {
                    pix[i] = color;
                }
                Texture2D result = new Texture2D(width, height);
                result.SetPixels(pix);
                result.Apply();
                return result;
            }
        }
    }

    
}
#endif