#if UNITY_EDITOR
using System;
using td.features.shard.components;
using td.utils;
using UnityEditor;

namespace td.features.shard
{
    public static class Shard_EditorUtils
    {
        public static void DrawShard(ref Shard shard, Shard_Calculator calc, string foldoutIndex = null) => DrawShard("Shard", ref shard, calc, foldoutIndex);
        public static void DrawShard(string title, ref Shard shard, Shard_Calculator calc, string foldoutIndex = null)
        {
            if (EditorUtils.FoldoutBegin(string.IsNullOrEmpty(foldoutIndex) ? shard.ToString() : foldoutIndex, "Shard", $"Shard {shard}"))
            {
                // DrawTitle("Shard");
                // EditorGUI.indentLevel++;
                EditorUtils.DrawProperty("Level", calc.GetShardLevel(shard.Quantity));
                EditorUtils.DrawProperty("Quantity", shard.Quantity);
                EditorGUILayout.Space();
                EditorUtils.DrawProperty("red", shard.red);
                EditorUtils.DrawProperty("green", shard.green);
                EditorUtils.DrawProperty("blue", shard.blue);
                EditorUtils.DrawProperty("aquamarine", shard.aquamarine);
                EditorUtils.DrawProperty("yellow", shard.yellow);
                EditorUtils.DrawProperty("orange", shard.orange);
                EditorUtils.DrawProperty("pink", shard.pink);
                EditorUtils.DrawProperty("violet", shard.violet);
                EditorGUILayout.Space();
                EditorUtils.DrawProperty("Price Buy", shard.price);
                EditorUtils.DrawProperty("Price Insert", shard.priceInsert);
                EditorUtils.DrawProperty("Time Insert", shard.timeInsert);
                EditorUtils.DrawProperty("Price Remove", shard.priceRemove);
                EditorUtils.DrawProperty("Time Remove", shard.timeRemove);
                EditorUtils.DrawProperty("Price Combine", shard.priceCombine);
                EditorUtils.DrawProperty("Time Combine", shard.timeCombine);
                EditorUtils.DrawProperty("Price Drop", shard.priceDrop);
                EditorGUILayout.Space();
                EditorUtils.DrawProperty("Radius", shard.radius);
                EditorUtils.DrawProperty("Fire Rate", shard.fireRate);
                EditorUtils.DrawProperty("Fire Countdown", shard.fireCountdown);
                EditorUtils.DrawProperty("Projectile Speed", shard.projectileSpeed);
                EditorUtils.DrawProperty("Current Color", shard.currentColor);
                EditorUtils.FoldoutEnd();
            }
        }
    }
}
#endif