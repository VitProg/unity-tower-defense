using td.utils;
using UnityEngine;

namespace td.monoBehaviours
{
#if UNITY_EDITOR
    [ExecuteAlways] // Код ниже должен исполняться всегда
    [SelectionBase] // Если вы кликнете на внутреннюю запчасть префаба, то выделится именно этот объект
#endif
    public sealed class HexGridSnaping : MonoBehaviour
    {
#if UNITY_EDITOR
        private void Update()
        {
            if (!Application.isPlaying && transform.hasChanged)
            {
                var pos = transform.position;
                var newPos = HexGridUtils.SnapToGrid(pos);

                if (((Vector2)pos - newPos).sqrMagnitude > 0.0001f)
                {
                    transform.position = newPos;
                }
            }
        }
#endif
    }
}