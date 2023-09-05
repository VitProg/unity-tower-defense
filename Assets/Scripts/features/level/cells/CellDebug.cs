using NaughtyAttributes;
using td.utils;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace td.features.level.cells
{
#if UNITY_EDITOR
    [ExecuteAlways] // Код ниже должен исполняться всегда
#endif
    public class CellDebug : MonoBehaviour
    {
#if UNITY_EDITOR
        [BoxGroup("Links")] [SerializeField] private GameObject arrow1;
        [BoxGroup("Links")] [SerializeField] private GameObject arrow2;
        [BoxGroup("Links")] [SerializeField] private GameObject switcherIcon;
        [BoxGroup("Links")] [SerializeField] private TextMeshPro labelX;
        [BoxGroup("Links")] [SerializeField] private TextMeshPro labelY;
        [BoxGroup("Links")] [SerializeField] private GameObject kernelLabel;
        [BoxGroup("Links")] [SerializeField] private GameObject spawnLabel;
        [BoxGroup("Links")] [SerializeField] private GameObject manualLabel;

        [FormerlySerializedAs("cell")] public CellMonoBehaviour cellMB;
        private bool isCellMBNull;

        private void Start()
        {
            isCellMBNull = cellMB == null;
        }
        
        private void Update()
        {
            if (Application.isPlaying) return;
            if (isCellMBNull)
            {
                if (transform.parent)
                {
                    cellMB = transform.parent.GetComponent<CellMonoBehaviour>();
                    isCellMBNull = cellMB == null;
                }
            }

            if (isCellMBNull) return;

            var coords = HexGridUtils.PositionToCell(cellMB.transform.position);
            
            if (labelX) labelX.text = coords.x.ToString();
            if (labelY) labelY.text = coords.y.ToString();

            if (cellMB.type == CellTypes.CanWalk)
            {
                if (switcherIcon) switcherIcon.SetActive(cellMB.isSwitcher && !cellMB.isKernel);
                if (kernelLabel) kernelLabel.SetActive(cellMB.isKernel);
                if (spawnLabel) spawnLabel.SetActive(cellMB.isSpawn);
                if (manualLabel) manualLabel.SetActive(!cellMB.isAutoNextSearching);
                RotateArrow(arrow1, cellMB.directionToNext, !cellMB.isKernel && cellMB.directionToNext != HexDirections.NONE);
                RotateArrow(arrow2, cellMB.directionToAltNext, !cellMB.isKernel && cellMB.isSwitcher && cellMB.directionToAltNext != HexDirections.NONE);
            }
            else
            {
                if (arrow1) arrow1.SetActive(false);
                if (arrow2) arrow2.SetActive(false);
                if (switcherIcon) switcherIcon.SetActive(false);
                if (manualLabel) manualLabel.SetActive(false);
            }
        }

        private void RotateArrow(GameObject arrow, HexDirections direction, bool visible)
        {
            if (!arrow) return;

            if (!visible || direction == HexDirections.NONE || direction == HexDirections.NONE)
            {
                arrow.SetActive(false);
                return;
            }

            arrow.SetActive(true);
            
            var angle = 0f;

            switch (direction)
            {
                case HexDirections.NorthWest:
                    angle = 60f;
                    break;
                case HexDirections.North:
                    angle = 0f;
                    break;
                case HexDirections.NorthEast:
                    angle = -60f;
                    break;
                case HexDirections.SouthEast:
                    angle = -120f;
                    break;
                case HexDirections.South:
                    angle = 180f;
                    break;
                case HexDirections.SouthWest:
                    angle = 120f;
                    break;
            }

            arrow.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
#endif
    }
}