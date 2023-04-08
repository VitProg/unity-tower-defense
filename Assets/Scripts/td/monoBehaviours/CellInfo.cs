using System;
using System.ComponentModel;
using td.common;
using UnityEngine;

namespace td.monoBehaviours
{
    public class CellInfo : MonoBehaviour
    {
#if UNITY_EDITOR
        [SerializeField] public Cell CellData { get; private set; }

        public void SetCell(Cell cell)
        {
            this.CellData = cell;
        }
#endif
    }
}