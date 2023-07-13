using UnityEngine;

namespace td.utils
{
    public static class CameraUtils
    {
        // private static Camera Camera;
        
        public static Vector3 ToWorldPoint(Camera camera, Vector2 inputPos, float z = 0f) {
            // if (Camera == null)
            // {
                // Camera = Camera.main;
            // }
            // var pos = new Vector3(inputPos.x, inputPos.y, -camera.transform.position.z);
            var pos = new Vector3(inputPos.x, inputPos.y, z);
            return camera.ScreenToWorldPoint(pos);
        }
    }
}