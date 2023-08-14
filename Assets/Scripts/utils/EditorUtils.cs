using System.Collections.Generic;
using td.features.enemy.components;
using td.features.shard;
using td.features.shard.components;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
namespace td.utils
{
    public static class EditorUtils
    {
        public static void DrawTitle(string sTitle, bool sep = false)
        {
            if (sep)
            {
                EditorGUILayout.Space();
                EditorGUILayout.Separator();
            }

            EditorGUILayout.LabelField(sTitle);//, boldStyle);
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

        public static void DrawProperty(Enemy? enemy)
        {
            if (!enemy.HasValue) return;
            DrawTitle("Enemy");
            EditorGUI.indentLevel++;
            //todo
            EditorGUI.indentLevel--;
        }

        public static Dictionary<string, bool> foldouts = new();
        public static GUIStyle boldStyle;

        public static bool FoldoutBegin(string key, string title, string titleHidden = null)
        {
            EditorGUI.indentLevel++;
            var value = foldouts.TryGetValue(key, out var f) && f;
            foldouts[key] = value;
            foldouts[key] = EditorGUILayout.Foldout(value,
                value ? title : (string.IsNullOrEmpty(titleHidden) ? title : titleHidden));
            return value;
        }

        public static void FoldoutEnd()
        {
            EditorGUI.indentLevel--;
            // EditorGUILayout.EndFoldoutHeaderGroup();
        }

        public static void DrawProperty(Shard? shard, Shard_Calculator calc)
        {
            if (!shard.HasValue) return;

            if (FoldoutBegin(shard.ToString(), "Shard", $"Shard {shard}"))
            {
                // DrawTitle("Shard");
                // EditorGUI.indentLevel++;
                DrawProperty("Level", calc.GetShardLevel(shard.Value.Quantity));
                DrawProperty("Quantity", shard.Value.Quantity);
                EditorGUILayout.Space();
                DrawProperty("red", shard.Value.red);
                DrawProperty("green", shard.Value.green);
                DrawProperty("blue", shard.Value.blue);
                DrawProperty("aquamarine", shard.Value.aquamarine);
                DrawProperty("yellow", shard.Value.yellow);
                DrawProperty("orange", shard.Value.orange);
                DrawProperty("pink", shard.Value.pink);
                DrawProperty("violet", shard.Value.violet);
                EditorGUILayout.Space();
                DrawProperty("Cost Buy", shard.Value.cost);
                DrawProperty("Cost Insert", shard.Value.costInsert);
                DrawProperty("Cost Remove", shard.Value.costRemove);
                DrawProperty("Cost Combine", shard.Value.costCombine);
                DrawProperty("Cost Drop", shard.Value.costDrop);
            }
            FoldoutEnd();

            // EditorGUI.indentLevel--;
        }
    }
}
#endif