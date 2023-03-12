using td.utils;
using UnityEngine;

namespace esc
{
    public class SceneData : MonoBehaviour
    {
        public GridSnapping[] Cells;
        
#if UNITY_EDITOR
        [ContextMenu ("Find Cells")]

        void FindCells () {
            Cells = FindObjectsOfType<GridSnapping> ();
            Debug.Log ($"Successfully found {Cells.Length} cells!");
        }
#endif
    }
}