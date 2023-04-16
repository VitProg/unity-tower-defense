using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace td.monoBehaviours
{
    public class HightlightGridByCursor : MonoBehaviour
    {
        private Renderer renderer;
        
        private static readonly int SLightPosition = Shader.PropertyToID("_LightPosition");
        private static readonly int SLightPower = Shader.PropertyToID("_LightPower");
        private static readonly int SLightRadius = Shader.PropertyToID("_LightRadius");
        private static readonly int SGridColor = Shader.PropertyToID("_GridColor");
        private static readonly int SBgColor = Shader.PropertyToID("_BgColor");
        
        private Vector3 shift;
        private float fineLightRadius;
        private float fineLightPower;

        [SerializeField] private Color fineColor;
        [SerializeField] private Color errorColor;

        public GridHightlightState State = GridHightlightState.Fine;

        // Start is called before the first frame update
        void Start()
        {
            renderer = GetComponent<Renderer>();
            fineLightRadius = renderer.material.GetFloat(SLightRadius);
            fineLightPower = renderer.material.GetFloat(SLightPower);
            renderer.material.SetColor(SGridColor, fineColor);
        }

        // Update is called once per frame
        void Update()
        {
            var mousePressed = Input.GetMouseButton(0);
            
            var mousePosition = Input.mousePosition;
            var worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);

            Color color;
            float lightRadius;
            float lightPower;
            
            switch (State)
            {
                case GridHightlightState.Fine:
                    color = fineColor;
                    lightRadius = fineLightRadius;
                    lightPower = fineLightPower;
                    break;
                    
                case GridHightlightState.Error:
                    color = errorColor;
                    lightRadius = fineLightRadius * 0.9f;
                    lightPower = fineLightPower * 1.2f;
                    break;
                    
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            renderer.material.SetVector(SLightPosition, worldPosition);
            renderer.material.SetColor(SGridColor, color);
            renderer.material.SetFloat(SLightRadius, lightRadius * (mousePressed ? 0.9f : 1f));
            renderer.material.SetFloat(SLightPower, lightPower * (mousePressed ? 1.1f : 1f));
        }
    }

    public enum GridHightlightState
    {
        Fine,
        Error,
    }
}