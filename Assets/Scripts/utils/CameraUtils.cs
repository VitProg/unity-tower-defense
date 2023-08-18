using UnityEngine;

namespace td.utils
{
    public static class CameraUtils
    {
        // private static Camera Camera;
        
        public static Vector3 TransformPointToCameraSpace(Camera camera, Vector2 inputPos, float z = 0f) {
            // if (Camera == null)
            // {
                // Camera = Camera.main;
            // }
            // var pos = new Vector3(inputPos.x, inputPos.y, -camera.transform.position.z);
            var pos = new Vector3(inputPos.x, inputPos.y, z);
            var inCameraPos = camera.ScreenToWorldPoint(pos);

            return inCameraPos;
        }

        public static void FixAnchoeredPosition(this Transform transform)
        {
            var rectTransform = ((RectTransform)transform);
            var ap = rectTransform.anchoredPosition3D;
            rectTransform.anchoredPosition3D = new Vector3(ap.x, ap.y, 0.0f);
        }
        
        public static void FixAnchoeredPosition(this RectTransform rectTransform)
        {
            var ap = rectTransform.anchoredPosition3D;
            rectTransform.anchoredPosition3D = new Vector3(ap.x, ap.y, 0.0f);
        }
    }
}