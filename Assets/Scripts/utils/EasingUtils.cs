// Source - https://github.com/ai/easings.net/blob/master/src/easings/easingsFunctions.ts
// Demo - https://easings.net/

using System;
using UnityEngine;

namespace td.utils
{
    public static class EasingUtils
    {
        private const float c1 = 1.70158f;
        private const float c2 = c1 * 1.525f;
        private const float c3 = c1 + 1f;
        private const float c4 = (2 * Mathf.PI) / 3f;
        private const float c5 = (2 * Mathf.PI) / 4.5f;
        
        private static float BounceOut(float x)
        {
            const float n1 = 7.5625f;
            const float d1 = 2.75f;

            return x switch
            {
                < 1 / d1 => n1 * x * x,
                < 2 / d1 => n1 * (x -= 1.5f / d1) * x + 0.75f,
                < 2.5f / d1 => n1 * (x -= 2.25f / d1) * x + 0.9375f,
                _ => n1 * (x -= 2.625f / d1) * x + 0.984375f
            };
        }

        public static float Linear(float x)
        {
            return x;
        }

        public static float EaseInQuad(float x)
        {
            return x * x;
        }

        public static float EaseOutQuad(float x)
        {
            return 1f - (1f - x) * (1f - x);
        }

        public static float EaseInOutQuad(float x)
        {
            return x < 0.5f ? 2f * x * x : 1f - Mathf.Pow(-2f * x + 2f, 2f) / 2f;
        }

        public static float EaseInCubic(float x)
        {
            return x * x * x;
        }

        public static float EaseOutCubic(float x)
        {
            return 1f - Mathf.Pow(1f - x, 3f);
        }

        public static float EaseInOutCubic(float x)
        {
            return x < 0.5f ? 4f * x * x * x : 1f - Mathf.Pow(-2f * x + 2f, 3f) / 2f;
        }
        
        public static float EaseInQuart(float x)
        {
            return x * x * x * x;
        }

        public static float EaseOutQuart(float x)
        {
            return 1f - Mathf.Pow(1f - x, 4f);
        }

        public static float EaseInOutQuart(float x)
        {
            return x < 0.5f ? 8f * x * x * x * x : 1f - Mathf.Pow(-2f * x + 2f, 4f) / 2f;
        }

        public static float EaseInQuint(float x)
        {
            return x * x * x * x * x;
        }

        public static float EaseOutQuint(float x)
        {
            return 1f - Mathf.Pow(1f - x, 5f);
        }
        
        public static float EaseInOutQuint(float x)
        {
            return x < 0.5f ? 16f * x * x * x * x * x : 1f - Mathf.Pow(-2f * x + 2f, 5f) / 2f;
        }
        
        public static float EaseInSine(float x)
        {
            return 1f - Mathf.Cos((x * Mathf.PI) / 2f);
        }
        
        public static float EaseOutSine(float x)
        {
            return Mathf.Sin((x * Mathf.PI) / 2f);
        }
        
        public static float EaseInOutSine(float x)
        {
            return -(Mathf.Cos(Mathf.PI * x) - 1f) / 2f;
        }
        
        public static float EaseInExpo(float x)
        {
            return FloatUtils.IsEquals(x, 0f) ? 0f : Mathf.Pow(2f, 10f * x - 10f);
        }
        
        public static float EaseOutExpo(float x)
        {
            return FloatUtils.IsEquals(x ,1f) ? 1f : 1f - Mathf.Pow(2, -10 * x);
        }
        
        public static float EaseInOutExpo(float x)
        {
            return FloatUtils.IsEquals(x, 0)
                ? 0f
                : FloatUtils.IsEquals(x, 1)
                    ? 1f
                    : x < 0.5f
                        ? Mathf.Pow(2, 20f * x - 10f) / 2f
                        : (2f - Mathf.Pow(2, -20f * x + 10f)) / 2f;
        }
        
        public static float EaseInCirc(float x)
        {
            return 1f - Mathf.Sqrt(1f - Mathf.Pow(x, 2));
        }
        
        public static float EaseOutCirc(float x)
        {
            return Mathf.Sqrt(1f - Mathf.Pow(x - 1f, 2));
        }
        
        public static float EaseInOutCirc(float x)
        {
            return x < 0.5f
                ? (1f - Mathf.Sqrt(1f - Mathf.Pow(2f * x, 2))) / 2f
                : (Mathf.Sqrt(1f - Mathf.Pow(-2f * x + 2f, 2)) + 1f) / 2f;
        }
        
        public static float EaseInBack(float x)
        {
            return c3 * x * x * x - c1 * x * x;
        }
        
        public static float EaseOutBack(float x)
        {
            return 1f + c3 * Mathf.Pow(x - 1f, 3) + c1 * Mathf.Pow(x - 1f, 2);
        }
        
        public static float EaseInOutBack(float x)
        {
            return x < 0.5f
                ? (Mathf.Pow(2f * x, 2) * ((c2 + 1f) * 2f * x - c2)) / 2f
                : (Mathf.Pow(2f * x - 2f, 2) * ((c2 + 1f) * (x * 2f - 2f) + c2) + 2f) / 2f;
        }
        
        public static float EaseInElastic(float x)
        {
            return FloatUtils.IsEquals(x, 0f)
                ? 0f
                : FloatUtils.IsEquals(x, 1f)
                    ? 1f
                    : -Mathf.Pow(2f, 10f * x - 10f) * Mathf.Sin((x * 10f - 10.75f) * c4);
        }
        
        public static float EaseOutElastic(float x)
        {
            return FloatUtils.IsEquals(x, 0f)
                ? 0f
                : FloatUtils.IsEquals(x, 1f)
                    ? 1f
                    : Mathf.Pow(2f, -10f * x) * Mathf.Sin((x * 10f - 0.75f) * c4) + 1f;
        }
        
        public static float EaseInOutElastic(float x)
        {
            return FloatUtils.IsEquals(x, 0f)
                ? 0f
                : FloatUtils.IsEquals(x, 1f)
                    ? 1f
                    : x < 0.5f
                        ? -(Mathf.Pow(2f, 20f * x - 10f) * Mathf.Sin((20f * x - 11.125f) * c5)) / 2f
                        : (Mathf.Pow(2f, -20f * x + 10f) * Mathf.Sin((20f * x - 11.125f) * c5)) / 2f + 1f;
        }
        
        public static float EaseInBounce(float x)
        {
            return 1f - BounceOut(1f - x);
        }

        public static float EaseOutBounce(float x) => BounceOut(x);
            
        public static float EaseInOutBounce(float x)
        {
            return x < 0.5f
                ? (1f - BounceOut(1f - 2f * x)) / 2f
                : (1f + BounceOut(2f * x - 1f)) / 2f;
        }

        public static float EaseMethod(EasingMethod method, float x) => method switch
        {
            EasingMethod.Linear => Linear(x),
            EasingMethod.BounceOut => BounceOut(x),
            EasingMethod.InQuad => EaseInQuad(x),
            EasingMethod.OutQuad => EaseOutQuad(x),
            EasingMethod.InOutQuad => EaseInOutQuad(x),
            EasingMethod.InCubic => EaseInCubic(x),
            EasingMethod.OutCubic => EaseOutCubic(x),
            EasingMethod.InOutCubic => EaseInOutCubic(x),
            EasingMethod.InQuart => EaseInQuart(x),
            EasingMethod.OutQuart => EaseOutQuart(x),
            EasingMethod.InOutQuart => EaseInOutQuart(x),
            EasingMethod.InQuint => EaseInQuint(x),
            EasingMethod.OutQuint => EaseOutQuint(x),
            EasingMethod.InOutQuint => EaseInOutQuint(x),
            EasingMethod.InSine => EaseInSine(x),
            EasingMethod.OutSine => EaseOutSine(x),
            EasingMethod.InOutSine => EaseInOutSine(x),
            EasingMethod.InExpo => EaseInExpo(x),
            EasingMethod.OutExpo => EaseOutExpo(x),
            EasingMethod.InOutExpo => EaseInOutExpo(x),
            EasingMethod.InCirc => EaseInCirc(x),
            EasingMethod.OutCirc => EaseOutCirc(x),
            EasingMethod.InOutCirc => EaseInOutCirc(x),
            EasingMethod.InBack => EaseInBack(x),
            EasingMethod.OutBack => EaseOutBack(x),
            EasingMethod.InOutBack => EaseInOutBack(x),
            EasingMethod.InElastic => EaseInElastic(x),
            EasingMethod.OutElastic => EaseOutElastic(x),
            EasingMethod.InOutElastic => EaseInOutElastic(x),
            EasingMethod.InBounce => EaseInBounce(x),
            EasingMethod.OutBounce => EaseOutBounce(x),
            EasingMethod.InOutBounce => EaseInOutBounce(x),
            _ => Linear(x)
        };
        
        public enum EasingMethod
        {
            Linear,
            BounceOut,
            InQuad,
            OutQuad,
            InOutQuad,
            InCubic,
            OutCubic,
            InOutCubic,
            InQuart,
            OutQuart,
            InOutQuart,
            InQuint,
            OutQuint,
            InOutQuint,
            InSine,
            OutSine,
            InOutSine,
            InExpo,
            OutExpo,
            InOutExpo,
            InCirc,
            OutCirc,
            InOutCirc,
            InBack,
            OutBack,
            InOutBack,
            InElastic,
            OutElastic,
            InOutElastic,
            InBounce,
            OutBounce,
            InOutBounce,
        }
    }
}