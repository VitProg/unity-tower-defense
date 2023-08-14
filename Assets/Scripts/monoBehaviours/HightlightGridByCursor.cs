using NaughtyAttributes;
using td.features.camera;
using td.utils.di;
using UnityEngine;
using UnityEngine.Serialization;

namespace td.monoBehaviours
{
    [RequireComponent(typeof(Renderer))]
    public class HightlightGridByCursor : MonoBehaviour
    {
        private new Renderer renderer;

        private static readonly int SGridColor = Shader.PropertyToID("_GridColor");
        private static readonly int SBgColor = Shader.PropertyToID("_BgColor");
        private static readonly int SCellSize = Shader.PropertyToID("_CellSize");
        private static readonly int SGridWidth = Shader.PropertyToID("_GridWidth");
        private static readonly int SShift = Shader.PropertyToID("_Shift");
        private static readonly int SLightPosition = Shader.PropertyToID("_LightPosition");
        private static readonly int SLightPower = Shader.PropertyToID("_LightPower");
        private static readonly int SLightRadius = Shader.PropertyToID("_LightRadius");

        [SerializeField] private GameObject DebugMarker;

        [Header("For Perspective Camera")]
        [SerializeField] private LayerMask layerMask;

        [Header("Fine Status")]
        [SerializeField] private Color fineColor; // SGridColor
        [SerializeField] private Color fineColorBackground; // SBgColor
        
        [Header("Error Status")]
        [SerializeField] private Color errorColor; // SGridColor
        [SerializeField] private Color errorColorBackground; // SBgColor
        
        [Header("Light Config")]
        [SerializeField] private float lightPower; // SLightPower
        [SerializeField] private float lightRadius; // SLightRadius

        [Space(15)]
        [SerializeField] private float gridWight;
        [SerializeField] private Vector2 shift;
        
        [FormerlySerializedAs("State")] [Space(15)]
        public GridHightlightState state = GridHightlightState.Fine;

        private Plane plane;
        
        //
        private bool diResolved;
        //

        // public async void Awake()
        // {
            // await DI.Resolve(this);
            // diResolved = true;
        // }

        // Start is called before the first frame update
        private void Start()
        {
            renderer = GetComponent<Renderer>();
            renderer.material.SetColor(SGridColor, fineColor);
            renderer.material.SetColor(SBgColor, fineColorBackground);
            plane = new Plane(Vector3.forward, transform.position);
        }

        // Update is called once per frame
        void Update()
        {
            if (!diResolved) return;
            
            var mousePressed = Input.GetMouseButton(0);
            
            var mousePosition = Input.mousePosition;
            var worldPosition = Vector3.zero;

            var cameraService = ServiceContainer.Get<Camera_Service>();
            
            var mainCamera = cameraService.GetMainCamera();
            
            if (cameraService.IsPerspectiveCameraMode())
            {
                var ray = mainCamera.ScreenPointToRay(mousePosition);

                if (plane.Raycast(ray, out var distance))
                {
                    worldPosition = ray.GetPoint(distance);
                }
            }
            else
            {
                worldPosition = mainCamera.ScreenToWorldPoint(mousePosition);
            }

            if (DebugMarker)
            {
                DebugMarker.transform.position = worldPosition;
            }

            var (color, bgColor, lr, lp) = GetByStatus(lightRadius, lightPower);
            
            renderer.material.SetVector(SLightPosition, worldPosition);
            renderer.material.SetColor(SGridColor, color);
            renderer.material.SetColor(SBgColor, bgColor);
            renderer.material.SetFloat(SLightRadius, lr * (mousePressed ? 0.9f : 1f));
            renderer.material.SetFloat(SLightPower, lp * (mousePressed ? 1.1f : 1f));
        }

        private (Color, Color, float, float) GetByStatus(float lr, float lp)
        {
            return state switch
            {
                GridHightlightState.Fine => (fineColor, fineColorBackground, lr, lp),
                GridHightlightState.Error => (errorColor, errorColorBackground, lr * 0.9f, lp * 1.2f),
                _ => (fineColor, fineColorBackground, lr, lp)
            };
        }

#if UNITY_EDITOR
        [Button]
        public void UpdateShaderParameters()
        {
            var (color, bgColor, lr, lp) = GetByStatus(lightRadius, lightPower);
            
            // Debug.Log($"color: {color}");
            // Debug.Log($"bgColor: {bgColor}");
            // Debug.Log($"lightRadius: {lr}");
            // Debug.Log($"lightPower: {lightPower}");
            // Debug.Log($"gridWight: {gridWight}");
            // Debug.Log($"shift: {shift}");
            
            var renderer = GetComponent<Renderer>();
            renderer.material.SetColor(SGridColor, color);
            renderer.material.SetColor(SBgColor, bgColor);
            renderer.material.SetFloat(SLightRadius, lr);
            renderer.material.SetFloat(SLightPower, lightPower);
            renderer.material.SetFloat(SGridWidth, gridWight);
            renderer.material.SetVector(SShift, shift);
        }
#endif
    }

    public enum GridHightlightState
    {
        Fine,
        Error,
    }

}