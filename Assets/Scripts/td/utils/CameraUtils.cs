using UnityEngine;

namespace td.utils
{
    public static class CameraUtils
    {
        public static Vector3 ToWorldPoint(Vector2 inputPos) {
            var pos = new Vector3(inputPos.x, inputPos.y, - Camera.main.transform.position.z);
            return Camera.main.ScreenToWorldPoint(pos);
        }
    }
}