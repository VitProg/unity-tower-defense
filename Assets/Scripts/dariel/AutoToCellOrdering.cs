using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class AutoToCellOrdering : MonoBehaviour
{
    [SerializeField] float gridSize = 0.5f;

    void Update()
    {
        Vector3 snap;
        snap.x = Mathf.RoundToInt(transform.position.x / gridSize) * gridSize;
        snap.y = Mathf.RoundToInt(transform.position.y / gridSize) * gridSize;
        transform.position = new Vector3(snap.x, snap.y, 0);
    }
}
