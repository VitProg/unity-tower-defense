using UnityEngine;
using td.utils.ecs;
using td.features.state;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace td.features.gameSpeed
{
    public class UITimeFlowButton : MonoBehaviour
    {
        [Inject] private State state;
        [SerializeField] private float gameSpeed;
        
        public void ChangeTimeFlow()
        {
            if (state == null)
            {
                DI.Resolve(this);
            }
            
            state!.GameSpeed = gameSpeed; 

            // todo
            var aColorSelected = 1;
            var aColorNotSelected = 0.6f;
            float aColorNewClick;
            
            foreach (var timeButtonGO in transform.parent.GetComponentsInChildren<Button>())
            {
                aColorNewClick = timeButtonGO.gameObject == gameObject ? aColorSelected : aColorNotSelected;

                if (timeButtonGO.transform.GetChild(0).TryGetComponent<Image>(out var imageUnderTimeButton))
                {
                    imageUnderTimeButton.color = new Color(
                        imageUnderTimeButton.color.r,
                        imageUnderTimeButton.color.g,
                        imageUnderTimeButton.color.b,
                        aColorNewClick);
                }
            }
        }
    }
}