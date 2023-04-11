using System;
using System.ComponentModel;
using td.common;
using td.common.cells;
using UnityEngine;

namespace td.monoBehaviours
{
    public class CellInfo : MonoBehaviour
    {
#if UNITY_EDITOR
        [SerializeField] public ICell CellData { get; private set; }

        public void SetCell(ICell cell)
        {
            CellData = cell;
        }
#endif
    }
}