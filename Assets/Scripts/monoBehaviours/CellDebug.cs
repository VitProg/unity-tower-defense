using System;
using Leopotam.EcsLite;
using NaughtyAttributes;
using td.services;
using td.common;
using td.utils;
using TMPro;
using UnityEngine;


namespace td.monoBehaviours
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

        public Cell cell;

        private void Start()
        {
            
        }

        private void Update()
        {
            if (cell == null)
            {
                if (transform.parent)
                {
                    cell = transform.parent.GetComponent<Cell>();
                }
            }

            if (cell == null) return;
            
            if (labelX) labelX.text = cell.Coords.x.ToString();
            if (labelY) labelY.text = cell.Coords.y.ToString();

            if (cell.type == CellTypes.CanWalk)
            {
                if (switcherIcon) switcherIcon.SetActive(cell.isSwitcher && !cell.isKernel);
                if (kernelLabel) kernelLabel.SetActive(cell.isKernel);
                if (spawnLabel) spawnLabel.SetActive(cell.isSpawn);
                if (manualLabel) manualLabel.SetActive(!cell.isAutoNextSearching);
                RotateArrow(arrow1, cell.directionToNext, !cell.isKernel && cell.HasDirectionToNext);
                RotateArrow(arrow2, cell.directionToAltNext, !cell.isKernel && cell.isSwitcher && cell.HasAltSirectionToNext);
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