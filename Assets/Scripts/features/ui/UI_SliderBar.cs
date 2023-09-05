using System;
using System.Text;
using NaughtyAttributes;
using td.features._common;
using td.utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace td.features.ui
{
    public class UI_SliderBar : MonoBehaviour
    {
        [Required][SerializeField] private TMP_Text text;
        [Required][SerializeField] private Image slider;

        [OnValueChanged("Refresh")]
        public bool withEnergySymbol = false;
        
        [OnValueChanged("Refresh")]
        public bool withIcon = false;
        
        [OnValueChanged("Refresh")][Range(0, 32)][SerializeField][ShowIf("withIcon")]
        public int icon = 0;
        
        [OnValueChanged("Refresh")][SerializeField] public uint value = 5; 
        [OnValueChanged("Refresh")][SerializeField] public uint minValue = 0; 
        [OnValueChanged("Refresh")][SerializeField] public uint maxValue = 10; 
        [OnValueChanged("Refresh")][SerializeField] public Color color = Color.gray;
        
        private StringBuilder sb = new ();

        public void Refresh()
        {
            value = Math.Clamp(value, minValue, maxValue);
            var percent = (value - minValue) / (float)(maxValue - minValue);

            sb.Clear();

            if (withIcon) {
                sb.Append($"<sprite={icon} tint> ");
            }

            if (withEnergySymbol && minValue == 0)
            {
                sb.Append($"<size=90%>{Constants.UI.CurrencySign}</size> ");
            }

            if (minValue != 0)
            {
                sb.Append(CommonUtils.IntegerFormat(value));
            }
            else
            {
                sb.Append($"{CommonUtils.IntegerFormat(value, true)} <alpha=#88>/ {CommonUtils.IntegerFormat(maxValue, true)}</color>");
            }

            text.text = sb.ToString();
            
            slider.rectTransform.anchorMin = new Vector2(0f, 0f);
            slider.rectTransform.anchorMax = new Vector2(percent, 1f);
            slider.color = color;
        }
    }
}