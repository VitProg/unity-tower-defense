using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

namespace td.utils
{
    public class RelativePosition : MonoBehaviour
    {
        [OnValueChanged("Refresh")][Range(0f, 100f)] public float left = 0f;
        [OnValueChanged("Refresh")][Range(0f, 100f)] public float right = 0f;
        [OnValueChanged("Refresh")][Range(0f, 100f)] public float top = 0f;
        [OnValueChanged("Refresh")][Range(0f, 100f)] public float bottom = 0f;
        
        [Space][OnValueChanged("Refresh")][Range(-100f, 100f)] public float shiftX = 0f;
        [OnValueChanged("Refresh")][Range(-100f, 100f)] public float shiftY = 0f ;

        private bool initialized = false;
        private RectTransform rectTransform;
        
        private void Awake()
        {
            initialized = true;
            rectTransform = (RectTransform)transform;
        }

        [Button("Set from Rect Transform")]
        private void Setup()
        {
            if (!initialized) Awake();
            left = rectTransform.anchorMin.x * 100f;
            right = rectTransform.anchorMax.x * 100f;
            top = rectTransform.anchorMax.y * 100f;
            bottom = rectTransform.anchorMin.y * 100f;
        }

        [Button]
        private void Refresh()
        {
            if (!initialized) Awake();
            rectTransform.anchorMin = new Vector2(left / 100f - shiftX / 100f, bottom / 100f - shiftY / 100f);
            rectTransform.anchorMax = new Vector2(right / 100f - shiftX / 100f, top / 100f - shiftY / 100f);
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Refresh();
        }
#endif
    }
}