using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace td.monoBehaviours
{
    public class HightlightGridByCursor : MonoBehaviour
    {
        private Renderer renderer;
        private static readonly int LightPosition = Shader.PropertyToID("_LightPosition");
        private static readonly int LightPower = Shader.PropertyToID("_LightPower");
        private static readonly int LightRadius = Shader.PropertyToID("_LightRadius");
        private Vector3 shift;
        private float lightRadius;
        private float lightPower;

        // Start is called before the first frame update
        void Start()
        {
            renderer = GetComponent<Renderer>();
            lightRadius = renderer.material.GetFloat(LightRadius);
            lightPower = renderer.material.GetFloat(LightPower);
        }

        // Update is called once per frame
        void Update()
        {
            var mousePressed = Input.GetMouseButton(0);
            
            var mousePosition = Input.mousePosition;
            var worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            
            renderer.material.SetVector(LightPosition, worldPosition);
            renderer.material.SetFloat(LightPower, lightPower * (mousePressed ? 1.1f : 1f));
            renderer.material.SetFloat(LightRadius, lightRadius * (mousePressed ? 0.9f : 1f));
        }
    }
}