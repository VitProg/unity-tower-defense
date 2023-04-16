using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace td.monoBehaviours
{
    public class FollowCursor : MonoBehaviour
    {
        private void Update()
        {
            var mousePosition = Input.mousePosition;
            var worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            worldPosition.z = 1f;
            transform.position = worldPosition;
        }
    }
}