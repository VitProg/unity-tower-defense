#if UNITY_EDITOR
using td.features.enemy.components;
using td.utils;
using UnityEditor;

namespace td.features.enemy
{
    public static class Enemy_EditorUtils
    {
        public static void DrawEnemy(Enemy? enemy)
        {
            if (!enemy.HasValue) return;
            EditorUtils.DrawTitle("Enemy");
            EditorGUI.indentLevel++;
            //todo
            EditorGUI.indentLevel--;
        }
    }
}
#endif