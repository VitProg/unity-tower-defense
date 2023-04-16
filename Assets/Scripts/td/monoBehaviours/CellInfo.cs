using System;
using System.ComponentModel;
using td.common;
using td.common.cells;
using td.common.cells.interfaces;
using td.services;
using td.utils;
using TMPro;
using UnityEngine;

namespace td.monoBehaviours
{
    public class CellInfo : MonoBehaviour
    {
        private Transform arrow;
        private TextMeshPro labelX;
        private TextMeshPro labelY;
        private LevelMap levelMap;

#if UNITY_EDITOR
        [SerializeField] public ICell cell { get; private set; }

        public void Init(ICell cell, LevelMap levelMap)
        {
            this.cell = cell;
            this.levelMap = levelMap;
        }

        public void Start()
        {
            arrow = transform.Find("arrow");
            labelX = transform.Find("x")?.GetComponent<TextMeshPro>();
            labelY = transform.Find("y")?.GetComponent<TextMeshPro>();
        }

        public void Update()
        {
            if (labelX)
            {
                labelX.text = cell.Coordinates.x.ToString();
            }
            if (labelY)
            {
                labelY.text = cell.Coordinates.y.ToString();
            }

            var arrowVisible = false;
            if (arrow && cell is ICellCanWalk { IsKernel: false, NextCellCoordinates: { empty: false } } walkCell)
            {
                var toNextCellVector = GridUtils.CellToCoords(walkCell.NextCellCoordinates, levelMap.CellType, levelMap.CellSize) -
                                       GridUtils.CellToCoords(walkCell.Coordinates, levelMap.CellType, levelMap.CellSize);
                    
                if (toNextCellVector.sqrMagnitude > Constants.ZeroFloat)
                {
                    arrowVisible = true;
                        
                    toNextCellVector.Normalize();

                    arrow.transform.rotation = Quaternion.LookRotation(Vector3.back, toNextCellVector);
                }
            }
            
            arrow.gameObject.SetActive(arrowVisible);
        }
#endif
    }
}