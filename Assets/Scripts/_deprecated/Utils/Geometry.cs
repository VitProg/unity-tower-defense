using UnityEngine;

namespace Utils
{
    public static class GeometryUtils
    {
        /**
         * ToDo add unti tests
         */
        public static float NormalizeAngle(float angle)
        {
            var normalizedAngle = angle is < 0 or > 360f ? angle % 360f : angle;
            if (normalizedAngle < 0) normalizedAngle = 360f + normalizedAngle;

            return normalizedAngle;
        }

        public static float AngleFromVelocity(Vector2 velocity)
        {
            return Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
        }

        public static float DeltaAngle(float angleA, float angleB)
        {
            return angleA - angleB < 0 ? angleA + 360f - angleB : angleA - angleB;
        }
    }
}