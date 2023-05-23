using td.features.shards;
using td.features.shards.mb;
using UnityEditor;
using UnityEngine;

namespace td.editor
{
    public class ShardEditor : ScriptableObject
    {
        [MenuItem("TD/Update All Shard Meshes", false, -200)]
        public static void UpdateShardMeshes()
        {
            foreach (var shardMeshGenerator in FindObjectsOfType<ShardMeshGenerator>())
            {
                shardMeshGenerator.Refresh();
            }
            
        }
    }
}