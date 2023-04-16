using td.utils;
using UnityEngine;

namespace td.monoBehaviours
{
#if UNITY_EDITOR
    [ExecuteAlways] // Код ниже должен исполняться всегда
    [SelectionBase] // Если вы кликнете на внутреннюю запчасть префаба, то выделится именно этот объект
#endif
    public sealed class GridSnappingAuto : MonoBehaviour
    {
#if UNITY_EDITOR
        [SerializeField] private float cellSize;
        
        private void Update()
        {
            if (!Application.isPlaying && transform.hasChanged)
            {
                cellSize = Mathf.Round(transform.localScale.x * 10f) / 10f;
                
                var pos = transform.position;
                var newPos = SquareGridUtils.SnapToGrid(pos, cellSize);

                if (((Vector2)pos - newPos).sqrMagnitude > 0.0001f)
                {
                    transform.position = newPos;
                }
            }
        }
#endif
    }
}
