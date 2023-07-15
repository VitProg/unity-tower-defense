using System.Threading.Tasks;
using NaughtyAttributes;
using td.features.menu;
using td.utils;
using UnityEngine;

namespace td.features.windows
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CanvasGroup))]
    public class FadeInOut : MonoBehaviour
    {
        [Required] [SerializeField] private CanvasGroup group;

        [Range(0f, 1f), OnValueChanged("UpdateAlpha")]
        [SerializeField] private float alpha = 0.0f;

        public EasingUtils.EasingMethod fateInMethod = EasingUtils.EasingMethod.InQuad;
        public EasingUtils.EasingMethod fateOutMehthod = EasingUtils.EasingMethod.OutQuad;
        
        private float progress = 0.0f;

        public float speed = 0.75f;

        [Range(0f, 1f), OnValueChanged("UpdateAlpha")]
        public float min = 0f;
        
        [Range(0f, 1f), OnValueChanged("UpdateAlpha")]
        public float max = 1f;

        public MenuState state { get; private set; } = MenuState.Hidden;
        public float currentAlpha => alpha;

        private void UpdateAlpha()
        {
            alpha = Mathf.Clamp(alpha, min, max);
            Debug.Log("alpha = " + alpha);
            group.alpha = alpha;
        }
        
        private void SetAlpha(float a)
        {
            alpha = a;
            group.alpha = alpha;
        }

        [Button]
        public void ToMin()
        {
            alpha = min;
            UpdateAlpha();
        }

        [Button]
        public void ToMax()
        {
            alpha = max;
            UpdateAlpha();
        }
        
        [Button]
        public async Task FadeIn(bool immediately = false)
        {
            gameObject.SetActive(true);
            state = MenuState.FadeIn;
            
            if (immediately)
            {
                SetAlpha(max);
                progress = 0f;
                state = MenuState.Normal;
                return;
            }

            while (true)
            {
                progress += Time.deltaTime * speed;
                var t = EasingUtils.EaseMethod(fateInMethod, progress);
                alpha = Mathf.Lerp(min, max, t);
                UpdateAlpha();
                if (state != MenuState.FadeIn || progress > 1f) break;
                await Task.Yield();
            }
            
            if (state != MenuState.FadeIn) return;
            
            SetAlpha(max);
            progress = 0f;
            state = MenuState.Normal;
        }

        [Button]
        public async Task FadeOut(bool immediately = false)
        {
            gameObject.SetActive(true);
            state = MenuState.FadeOut;
            
            if (immediately)
            {
                SetAlpha(min);
                progress = 0f;
                state = MenuState.Hidden;
                if (min < 0.0001f) gameObject.SetActive(false);
                return;
            }
            
            while (true)
            {
                progress += Time.deltaTime * speed;
                var t = EasingUtils.EaseMethod(fateOutMehthod, progress);
                alpha = Mathf.Lerp(max, min, t);
                UpdateAlpha();
                if (state != MenuState.FadeOut || progress > 1f) break;
                await Task.Yield();
            }

            if (state != MenuState.FadeOut) return;
            
            SetAlpha(min);
            progress = 0f;
            state = MenuState.Hidden;
            if (min < 0.0001f) gameObject.SetActive(false);
        }
    }
}