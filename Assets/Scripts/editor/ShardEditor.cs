using td.features.shard.mb;
using UnityEditor;
using UnityEngine;

namespace td.editor
{
    public class ShardEditor : ScriptableObject
    {
        [MenuItem("TD/Update All Shard Meshes", false, -200)]
        public static void UpdateShardMeshes()
        {
            foreach (var s in FindObjectsOfType<UI_Shard_Button>())
            {
                s.Refresh();
            }       
            foreach (var s in FindObjectsOfType<UI_Shard>())
            {
                s.FullRefresh();
            }
        }
    }
}