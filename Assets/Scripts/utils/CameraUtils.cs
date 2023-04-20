using UnityEngine;

namespace td.utils
{
    public static class CameraUtils
    {
        private static Camera Camera;
        
        public static Vector3 ToWorldPoint(Vector2 inputPos) {
            if (Camera == null)
            {
                Camera = Camera.main;
            }
            var pos = new Vector3(inputPos.x, inputPos.y, -Camera.transform.position.z);
            return Camera.ScreenToWorldPoint(pos);
        }
    }
}