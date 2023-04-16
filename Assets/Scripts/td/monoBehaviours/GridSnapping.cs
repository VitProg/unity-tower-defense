using td.utils;
using UnityEngine;

namespace td.monoBehaviours
{
#if UNITY_EDITOR
    [ExecuteAlways] // Код ниже должен исполняться всегда
    [SelectionBase] // Если вы кликнете на внутреннюю запчасть префаба, то выделится именно этот объект
#endif
    public sealed class GridSnapping : MonoBehaviour
    {
#if UNITY_EDITOR
        public float CellSize = 1f;
        
        private void Update()
        {
            if (!Application.isPlaying && transform.hasChanged)
            {
                var pos = transform.localPosition;
                var newPos = SquareGridUtils.SnapToGrid(pos, CellSize);

                if (((Vector2)pos - newPos).sqrMagnitude > 0.0001f)
                {
                    transform.position = newPos; // Магнитим клетку к сетке.
                }
            }
        }
#endif
    }
}
