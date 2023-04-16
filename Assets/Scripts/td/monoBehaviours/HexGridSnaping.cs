using System;
using td.common;
using td.utils;
using UnityEngine;
using UnityEngine.Serialization;

namespace td.monoBehaviours
{
#if UNITY_EDITOR
    [ExecuteAlways] // Код ниже должен исполняться всегда
    [SelectionBase] // Если вы кликнете на внутреннюю запчасть префаба, то выделится именно этот объект
#endif
    public sealed class HexGridSnaping : MonoBehaviour
    {
#if UNITY_EDITOR
        public float CellSize = 1f;
        
        private void Update()
        {
            if (!Application.isPlaying && transform.hasChanged)
            {
                var pos = transform.position;
                var newPos = HexGridUtils.SnapToGrid(pos, CellSize);

                if (((Vector2)pos - newPos).sqrMagnitude > 0.0001f)
                {
                    transform.position = newPos;
                }
            }
        }

        // private const float HexRadius = 2f;
        // private const float HexWidth = HexRadius * 2;
        // private float HexHeight => Mathf.Sqrt(3) * HexRadius;
        // private float HexHorizontalSpacing => HexWidth * 0.75f;
        // private float HexVerticalSpacing => HexHeight;
        //
        // // Функция для перевода индексов шестиугольника в позицию в сетке
        // private Vector2 HexIndexToGridPosition(int xIndex, int yIndex)
        // {
        //     int x = xIndex;
        //     int y = yIndex;
        //
        //     if (yIndex % 2 == 1) // Нечетные ряды смещены на половину расстояния между соседними шестиугольниками по оси X
        //     {
        //         x += 1;
        //     }
        //
        //     return new Vector2Int(x, y);
        // }
        //
        // // Функция для перевода координат в системе координат шестиугольника в позицию в сетке
        // private Vector2Int HexCoordinatesToGridPosition(float xCoord, float yCoord)
        // {
        //     int xIndex = Mathf.RoundToInt(xCoord / (HexWidth * 0.75f));
        //     int yIndex = Mathf.RoundToInt(yCoord / HexVerticalSpacing);
        //
        //     if (yIndex % 2 == 1) // Нечетные ряды смещены на половину расстояния между соседними шестиугольниками по оси X
        //     {
        //         xIndex -= 1;
        //     }
        //
        //     return new Vector2Int(xIndex, yIndex);
        // }
        // private void Update()
        // {
        //     if (!Application.isPlaying && transform.hasChanged)
        //     {
        //         var pos = transform.position;
        //         // var hexCoord = HexCoordinatesToGridPosition(pos.x, pos.y);
        //         // var newPos = HexIndexToGridPosition(hexCoord.x, hexCoord.y);
        //         
        //         transform.position = new Vector2(
        //             // (pos/.x + pos.y * 0.5f - pos.y / 2),
        //             // pos.y
        //             Mathf.RoundToInt(pos.x / HexHorizontalSpacing) * HexHorizontalSpacing / 2,
        //             pos.y
        //             // Mathf.RoundToInt(pos.y / (2 * 0.75f)) * 0.75f
        //         );
        //
        //         // var gridCoordinates = HexGridUtils.GetGridCoordinate(curPos);
        //         // var snappedPos = HexGridUtils.GetVector(gridCoordinates);
        //
        //         // var gx = Mathf.RoundToInt(Mathf.Floor(curPos.x / (HexWidth)));
        //         // var px = gx * HexWidth;
        //         //
        //         // Debug.Log(gx);
        //         //
        //         // transform.position = new Vector2(
        //         //     px,
        //         //     curPos.y
        //         // );
        //
        //         // Debug.Log("------------------");
        //         // Debug.Log(curPos);
        //         // Debug.Log(gridCoordinates);
        //         // Debug.Log(snappedPos);
        //         // Debug.Log("------------------");
        //         // transform.localPosition = newPos; // Магнитим клетку к сетке.
        //     }
        // }

#endif
    }
}