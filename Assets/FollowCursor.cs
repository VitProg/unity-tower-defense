using System.Collections;
using System.Collections.Generic;
using td.common;
using td.utils.ecs;
using UnityEditor;
using UnityEngine;

namespace td
{
    public class FollowCursor : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            if (!DI.IsReady) return;

            var shared = DI.GetShared<SharedData>()!;

            var cursorScreenPos = Input.mousePosition;
            Debug.Log("# cursorScreenPos = " + cursorScreenPos);
            var cursorPosInCanvasCamera = shared.canvasCamera.ScreenToViewportPoint(cursorScreenPos);
            Debug.Log("# cursorPosInCanvasCamera = " + cursorPosInCanvasCamera);
            var cursorPosInMainCamera = shared.mainCamera.ViewportToWorldPoint(cursorPosInCanvasCamera);
            Debug.Log("# cursorPosInMainCamera = " + cursorPosInMainCamera);
            cursorPosInMainCamera.z = 0f;
            // Debug.Log("# cursorPosInMainCamera = " + cursorPosInMainCamera);

            transform.localPosition = cursorPosInMainCamera;
        }
    }
}
