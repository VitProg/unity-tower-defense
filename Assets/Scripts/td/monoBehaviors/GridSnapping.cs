using UnityEngine;

namespace td.utils
{
#if UNITY_EDITOR
    [ExecuteAlways] // Код ниже должен исполняться всегда
    [SelectionBase] // Если вы кликнете на внутреннюю запчасть префаба, то выделится именно этот объект
#endif
    public sealed class GridSnapping : MonoBehaviour
    {
        public float step = 2f;

#if UNITY_EDITOR
        private void Update()
        {
            if (!Application.isPlaying && transform.hasChanged)
            {
                var newPos = Vector3.zero;
                var curPos = transform.localPosition;
                newPos.x = Mathf.RoundToInt(curPos.x / step) * step;
                newPos.y = Mathf.RoundToInt(curPos.y / step) * step;
                transform.localPosition = newPos; // Магнитим клетку к сетке.
            }
        }
#endif
    }
}
